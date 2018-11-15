using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Entity_Framework.Util;

namespace Entity_Framework.SQLite.EF
{
    [DbConfigurationType(typeof(SqLitePackageLoader))]
    public class ExternallyMappedContext<T> : DbContext where T : class, IRepository
    {
        private readonly Dictionary<Type, string> _mappings;

        public ExternallyMappedContext(string dbFullFileName, bool debug, params (Type,string)[] mappings) : base(
            new SQLiteConnection
            {
                ConnectionString = new SQLiteConnectionStringBuilder()
                {
                    DataSource = dbFullFileName
                }.ConnectionString
            }, debug)
        {
            _mappings = new Dictionary<Type, string>();
            foreach (var mapping in mappings)
                _mappings.Add(mapping.Item1, mapping.Item2);

            Database.SetInitializer<ExternallyMappedContext<T>>(null);

            if (debug)
                Database.Log = s => { Debug.WriteLine(s); };
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            MapUsingReflection(modelBuilder, _mappings);
        }

        private void MapUsingReflection(DbModelBuilder modelBuilder, Dictionary<Type, string> mappings)
        {
            foreach (var type in typeof(T).GetNestedTypes())
            {
                var entityMethod = modelBuilder?.GetType()?.GetMethod(nameof(modelBuilder.Entity));
                var genericEntityMethod = entityMethod?.MakeGenericMethod(type);
                var entityObject = genericEntityMethod?.Invoke(modelBuilder, null);

                var casterMethod = typeof(Caster).GetMethod(nameof(Caster.Cast), BindingFlags.Static | BindingFlags.Public);
                var genericCasterMethod = casterMethod?.MakeGenericMethod(typeof(EntityTypeConfiguration<>).MakeGenericType(type));
                var castedEntityObject = genericCasterMethod?.Invoke(null, new[] { entityObject });

                if (mappings != null && mappings.ContainsKey(type))
                {
                    var mapping = mappings[type];

                    var lines = mapping.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                    for (var i = 1; i < lines.Length; i++)
                    {
                        var line = lines[i];
                        var command = line.Substring(0, line.IndexOf("]", StringComparison.Ordinal) + 1);
                        var parameter = line.Substring(line.IndexOf("]", StringComparison.Ordinal) + 1);
                        
                        switch (command)
                        {
                            case "[TABLE]":
                                var castedEntityObjectToTableMethod = castedEntityObject?.GetType().GetMethod("ToTable", new[] { typeof(string) });
                                castedEntityObjectToTableMethod?.Invoke(castedEntityObject, new object[] { parameter });
                                break;
                            case "[KEY]":
                                var lambdaParameter = Expression.Parameter(type, "x");
                                Expression lambdaProperty = Expression.Property(lambdaParameter, parameter);
                                var lambda = Expression.Lambda(lambdaProperty, lambdaParameter);

                                // Set Key A: get HasKey method
                                var castedEntityObjectHasKeyMethodCandidatesArguments = "System.Linq.Expressions.Expression`1[System.Func`2["
                                                                                        + type + ",TKey]]";
                                var castedEntityObjectHasKeyMethodCandidates = castedEntityObject?.GetType()
                                    .GetMethods()
                                    .Where(m => m.Name == "HasKey")
                                    .Where(m => m.ToString().Contains(castedEntityObjectHasKeyMethodCandidatesArguments))
                                    .Where(p => p.GetParameters().Length == 1)
                                    .Select(p => p);

                                // Set Key B: invoke HasKey method
                                var castedEntityObjectHasKeyMethod = castedEntityObjectHasKeyMethodCandidates?.First();
                                castedEntityObjectHasKeyMethod?.MakeGenericMethod(typeof(string)).Invoke(castedEntityObject, new object[] { lambda });
                                break;
                                
                            case "[COLUMN]":
                                var parameterFirstPart = parameter.Substring(0, parameter.IndexOf("[", StringComparison.Ordinal)); // T: null check on [
                                var parameterSecondPart = parameter.Substring(parameter.IndexOf("]", StringComparison.Ordinal) + 1); // T: null check on ]

                                var lambdaParameterX = Expression.Parameter(type, "x");
                                Expression lambdaPropertyX = Expression.Property(lambdaParameterX, parameterSecondPart);
                                var lambdaX = Expression.Lambda(lambdaPropertyX, lambdaParameterX);

                                // Set Column & Name A: get Property method
                                var castedEntityObjectPropertyMethodCandidatesArguments = "System.Linq.Expressions.Expression`1[System.Func`2["
                                                                                          + type + ",System.String]]";
                                var castedEntityObjectPropertyMethodCandidates = castedEntityObject?.GetType()
                                    .GetMethods()
                                    .Where(m => m.Name == "Property")
                                    .Where(m => m.ToString().Contains(castedEntityObjectPropertyMethodCandidatesArguments))
                                    .Where(p => p.GetParameters().Length == 1)
                                    .Select(p => p);

                                // Set Column & Name B: invoke Property method
                                var castedEntityObjectPropertyMethod = castedEntityObjectPropertyMethodCandidates?.First();
                                var castedEntityObjectProperty = (StringPropertyConfiguration)castedEntityObjectPropertyMethod?.Invoke(castedEntityObject, new object[] { lambdaX });
                                var castedEntityObjectPropertyHasColumnNameMethod = castedEntityObjectProperty?.GetType()?.GetMethod("HasColumnName");
                                castedEntityObjectPropertyHasColumnNameMethod?.Invoke(castedEntityObjectProperty, new object[] { parameterFirstPart });
                                break;
                        }
                    }
                }
            }
        }
    }
}
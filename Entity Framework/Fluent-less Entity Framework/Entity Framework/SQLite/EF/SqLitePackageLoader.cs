using System;
using System.Data.Entity;
using System.Data.Entity.Core.Common;
using System.Data.SQLite;
using System.Data.SQLite.EF6;
using System.Reflection;

namespace Entity_Framework.SQLite.EF
{
    public class SqLitePackageLoader : DbConfiguration
    {
        public SqLitePackageLoader()
        {
            SetProviderFactory("System.Data.SQLite", SQLiteFactory.Instance);
            SetProviderFactory("System.Data.SQLite.EF6", SQLiteProviderFactory.Instance);
            var sqLiteType = Type.GetType("System.Data.SQLite.EF6.SQLiteProviderServices, System.Data.SQLite.EF6");
            var sqLiteFieldinfo = sqLiteType?.GetField("Instance", BindingFlags.NonPublic | BindingFlags.Static);
            SetProviderServices("System.Data.SQLite", (DbProviderServices)sqLiteFieldinfo?.GetValue(null));
        }
    }
}
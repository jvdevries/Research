using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using DevUtil.Aspects.Extensions;

namespace DevUtil.Aspects.Contracts
{
    // ReSharper disable once ConvertToStaticClass
    public sealed class StateContractManager
    {
        private const string StateContractPropertyName = "StateContract";

        private StateContractManager()
        {
        }

        /// <summary>
        /// Gathers classes relying on a StateContract via <see cref="ReliesOnStateContractAttribute"/>, and checks that the reliance is valid.
        /// </summary>
        /// <param name="assemblyToUse">The <see cref="Assembly"/> which is searched.</param>
        /// <returns>A <see cref="List{T}"/>List of <see cref="Tuple"/> with the classes relying on a StateContract, and the HashCode of the contract.</returns>
        public static IReadOnlyList<(Type reliantClass, string contractHash)> GetBrokenReliances(Assembly assemblyToUse)
        {
            // A reliantClass (RC) relies on a contractClass (CC): RC uses ReliesOnContractAttribute pointing to CC.
            var brokenReliances = new List<(Type reliantClass, string contractHash)>();
            var contractsCache = new Dictionary<Type, string>();

            var reliances =
                from type in assemblyToUse.GetTypes().AsParallel()
                let typeAttributes = type.GetCustomAttributes(typeof(ReliesOnStateContractAttribute), true)
                where typeAttributes != null && typeAttributes.Length > 0
                select new {reliantClass = type, classReliances = typeAttributes};

            foreach (var reliance in reliances)
            {
                var reliantClass = reliance.reliantClass;
                foreach (ReliesOnStateContractAttribute relyAttribute in reliance.classReliances)
                {
                    var contractClass = relyAttribute.ContractClass;

                    // Use Reflection to get the contract property.
                    var stateContractProperty = contractClass.GetProperty(StateContractPropertyName);
                    if (stateContractProperty == null || !stateContractProperty.GetGetMethod().IsStatic)
                    {
                        brokenReliances.Add((reliantClass, ""));
                        break;
                    }

                    var contract = stateContractProperty.GetValue(null, null);

                    // Check that the contract has the correct type.
                    var contractType = contract.GetType();
                    var expectedType =
                        typeof(Expression<>).MakeGenericType(
                            typeof(Func<,>).MakeGenericType(contractClass, typeof(bool)));
                    if (contractType != expectedType)
                    {
                        brokenReliances.Add((reliantClass, ""));
                        break;
                    }

                    // Get the contract HashCode.
                    var contractHashCode = contractsCache.ContainsKey(contractClass)
                        ? contractsCache[contractClass]
                        : (contract as Expression).GetViewHashCode();

                    // Check that the contract HashCode matches the HashCode of the reliantClass.
                    if (contractHashCode.Equals(relyAttribute.HashCode))
                        contractsCache.Add(reliantClass, contractHashCode);
                    else
                        brokenReliances.Add((contractClass, contractHashCode));
                }
            }

            return brokenReliances;
        }
    }
}
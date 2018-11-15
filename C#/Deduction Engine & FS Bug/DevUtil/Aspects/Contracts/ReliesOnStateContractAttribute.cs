using System;

namespace DevUtil.Aspects.Contracts
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class ReliesOnStateContractAttribute : Attribute
    {
        public ReliesOnStateContractAttribute(Type contractClass, string hashCode)
        {
            ContractClass = contractClass;
            HashCode = hashCode;
        }

        public Type ContractClass { get; }
        public string HashCode { get; }
    }
}
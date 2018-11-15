namespace DevUtil.Aspects.Extensions
{
    public static class GenericExt
    {
        public static T[] InArray<T>(this T o, params T[] mergeWith)
        {
            if (mergeWith == null || mergeWith.Length == 0)
            {
                return new[] {o};
            }

            var newArray = new T[mergeWith.Length + 1];

            newArray[0] = o;
            mergeWith.CopyTo(newArray, 1);

            return newArray;
        }
    }
}
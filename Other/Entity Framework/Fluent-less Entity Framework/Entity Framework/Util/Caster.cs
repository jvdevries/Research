namespace Entity_Framework.Util
{
    public static class Caster
    {
        public static T Cast<T>(object o) => (T)o;
    }
}
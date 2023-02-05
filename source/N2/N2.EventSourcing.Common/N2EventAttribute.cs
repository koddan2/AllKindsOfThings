namespace N2.EventSourcing.Common
{
	[AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
    public sealed class N2EventAttribute : Attribute
    {
        public N2EventAttribute()
        {
        }
    }
}
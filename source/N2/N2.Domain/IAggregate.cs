namespace N2.Domain
{
	public interface IAggregate
	{
		string Identity { get; }
		ulong Revision { get; }
	}
}
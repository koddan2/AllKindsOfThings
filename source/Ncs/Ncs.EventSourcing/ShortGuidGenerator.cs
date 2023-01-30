using CSharpVitamins;

namespace Ncs.EventSourcing
{
	public class ShortGuidGenerator : IUniqueIdGenerator
	{
		string IUniqueIdGenerator.MakeOne()
		{
			var guid = Guid.NewGuid();
			return ShortGuid.Encode(guid);
		}
	}
}
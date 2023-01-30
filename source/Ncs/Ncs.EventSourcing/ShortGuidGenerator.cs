
using Base62;
using System.Text;

namespace Ncs.EventSourcing
{
	public class ShortGuidGenerator : IUniqueIdGenerator
	{
		////private static readonly Base62Converter _Base62Converter = new();
		string IUniqueIdGenerator.MakeOne()
		{
			return Guid.NewGuid().ToBase62();
		}

		internal static string GetGuidBase36Encoded()
		{
			var guidBytes = Guid.NewGuid().ToByteArray();
			var fst = new byte[8];
			var snd = new byte[8];
			for (var i = 0; i < 8; ++i)
			{
				fst[i] = guidBytes[i];
			}
			for (var i = 0; i < 8; ++i)
			{
				snd[i] = guidBytes[i + 8];
			}
			var left = BitConverter.ToInt64(fst);
			var right = BitConverter.ToInt64(snd);

			var s1 = Base36.NumberToBase36(left);
			var s2 = Base36.NumberToBase36(right);

			return $"{s1}{s2}";
		}
	}
}
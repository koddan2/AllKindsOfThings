using Urdep.Extensions.Augmentation;
using Urdep.Extensions.Data;

namespace DZT.Lib.Helpers;

internal class LolClass
{
    private readonly int _max = 0;

    public void Test()
    {
        var erp = Augment.Ref(new AiLoadoutRoot());
        var dic = erp.AsDictionary();
        dic["Whatever"] = _max;
    }
}

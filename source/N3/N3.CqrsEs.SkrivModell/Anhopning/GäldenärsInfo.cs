
using N3.Modell;

namespace N3.CqrsEs.SkrivModell.Anhopning
{
    public record GäldenärsInfo(
        PersonNummer PersonNummer,
        string FullkomligtNamn,
        PostAdress HuvudsakligAdress,
        string? EpostAdress = null);
}

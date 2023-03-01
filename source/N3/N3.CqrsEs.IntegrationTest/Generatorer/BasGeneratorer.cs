using Bogus;
using N3.Modell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N3.CqrsEs.IntegrationTest.Generatorer
{
    internal static class BasGeneratorer
    {
        public static readonly DateOnly FakturaBasDatumStart = new(2020, 1, 1);
        public static readonly DateOnly FakturaBasDatumSlut = new(2023, 1, 1);
        public static readonly Faker<Faktura> TestFakturor = new Faker<Faktura>()
            //Ensure all properties have rules. By default, StrictMode is false
            //Set a global policy by using Faker.DefaultStrictMode
            .StrictMode(true)
            //OrderId is deterministic
            .RuleFor(
                o => o.FakturaDatum,
                f => f.Date.BetweenDateOnly(FakturaBasDatumStart, FakturaBasDatumSlut)
            )
            .RuleFor(o => o.FörfalloDatum, (f, o) => o.FakturaDatum.AddDays(30))
            .RuleFor(o => o.FakturaNummer, f => f.Random.Int(0, 999_999).ToString())
            .RuleFor(
                o => o.RänteStoppsDatum,
                (f, o) =>
                    f.Random.Int(0, 5) > 4
                        ? f.Date.BetweenDateOnly(o.FörfalloDatum, DateTime.Now.ToDateOnly())
                        : null
            )
            .RuleFor(o => o.RänteSats, f => new Procent(8))
            .RuleFor(
                o => o.UrsprungligBetalReferens,
                f => f.Random.Long(min: 10_000_000).ToString()
            )
            .RuleFor(
                o => o.UrsprungligtKapitalBelopp,
                f => new SvenskaKronor(Math.Round(f.Random.Decimal(400, 999_999), 2))
            )
            .RuleFor(
                o => o.KvarvarandeKapitalBelopp,
                (f, o) =>
                    new SvenskaKronor(
                        Math.Round(
                            f.Random.Int(0, 9) > 8
                                ? f.Random.Decimal(
                                    o.UrsprungligtKapitalBelopp.Belopp / 2,
                                    o.UrsprungligtKapitalBelopp.Belopp
                                )
                                : o.UrsprungligtKapitalBelopp.Belopp,
                            2
                        )
                    )
            )
            .RuleFor(o => o.KvarvarandePåminnelseKostnad, f => new SvenskaKronor(60))
            .RuleFor(o => o.RänteSatsTyp, f => RänteSatsTyp.ÖverGällandeReferensRänta)
            .RuleFor(
                o => o.RänteUträkningsSätt,
                f => RänteUträkningsSätt.DagligUträkningPåÅrsbasis
            );
    }
}

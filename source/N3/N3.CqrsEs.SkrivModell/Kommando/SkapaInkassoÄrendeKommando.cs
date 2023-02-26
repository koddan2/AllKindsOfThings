﻿using N3.CqrsEs.Ramverk;
using N3.Modell;
using SmartAnalyzers.CSharpExtensions.Annotations;

namespace N3.CqrsEs.SkrivModell.Kommando
{
    [InitRequired]
    public class SkapaInkassoÄrendeKommando : IKommando
    {
        public SkapaInkassoÄrendeKommando(
            UnikIdentifierare aggregatIdentifierare,
            UnikIdentifierare klientReferens,
            UnikIdentifierare[] gäldenärsReferenser,
            Faktura[] fakturor
        )
        {
            AggregatIdentifierare = aggregatIdentifierare;
            KlientReferens = klientReferens;
            GäldenärsReferenser = gäldenärsReferenser;
            Fakturor = fakturor;
        }

        public UnikIdentifierare KorrelationsIdentifierare { get; init; }
        public IEnumerable<string> Historia { get; } = new List<string>();

        public string Auktorisering { get; init; }
        public long FörväntadRevision { get; init; }

        public UnikIdentifierare AggregatIdentifierare { get; }
        public UnikIdentifierare KlientReferens { get; }
        public UnikIdentifierare[] GäldenärsReferenser { get; }
        public Faktura[] Fakturor { get; }
    }
}

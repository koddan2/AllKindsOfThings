using N3.Modell;
using System.Text.Json;

namespace N3.Modell.Test
{
	public class N3ModelTests
	{
		[SetUp]
		public void Setup()
		{
		}

		[Test]
		[TestCase(0, 0)]
		[TestCase(1, 1.25)]
		[TestCase(400, 500)]
		[TestCase(400.04, 500.05)]
		public void TestaSvenskaKronorOchMoms1(decimal ingåendeBelopp, decimal förväntatBelopp)
		{
			SvenskaKronor sek = ingåendeBelopp;
			SvenskaKronorOchMoms sekMoms = new(sek, Moms._25);
			Assert.That(sekMoms.TotalBelopp, Is.EqualTo((SvenskaKronor)förväntatBelopp));

			TestaSerialiseringTurOchRetur(sekMoms);
		}

		[Test]
		[TestCase(0, 0)]
		[TestCase(1, 0.25)]
		[TestCase(400, 100)]
		public void TestaMoms1(decimal ingåendeBelopp, decimal förväntatBelopp)
		{
			SvenskaKronor sek = ingåendeBelopp;
			Assert.That(sek.RäknaUtMomsDel(Moms._25), Is.EqualTo((SvenskaKronor)förväntatBelopp));

			TestaSerialiseringTurOchRetur(sek);
		}

		[Test]
		[TestCase(0, 0)]
		[TestCase(1, 0.8)]
		[TestCase(400, 320)]
		public void TestaMoms3(decimal ingåendeBelopp, decimal förväntatBelopp)
		{
			SvenskaKronor sek = ingåendeBelopp;
			Assert.That(sek.RäknaUtMomsBas(Moms._25), Is.EqualTo((SvenskaKronor)förväntatBelopp));

			TestaSerialiseringTurOchRetur(sek);
		}

		[Test]
		[TestCase(0, 0)]
		[TestCase(1, 1.25)]
		[TestCase(400, 500)]
		public void TestaMoms2(decimal ingåendeBelopp, decimal förväntatBelopp)
		{
			SvenskaKronor sek = ingåendeBelopp;
			Assert.That(sek.LäggPå(Moms._25), Is.EqualTo((SvenskaKronor)förväntatBelopp));

			TestaSerialiseringTurOchRetur(sek);
		}

		[Test]
		[TestCase(0)]
		[TestCase(99)]
		[TestCase(100_000.9999)]
		public void TestaSvenskaKronor1(decimal ingåendeBelopp)
		{
			SvenskaKronor sek1 = ingåendeBelopp;
			SvenskaKronor sek2 = ingåendeBelopp;
			Assert.Multiple(() =>
			{
				Assert.That(sek1, Is.EqualTo(sek2));
				Assert.That(sek1, Is.EqualTo(sek2));
				Assert.That(sek1, Is.EqualTo((SvenskaKronor)ingåendeBelopp));
				Assert.That(sek2, Is.EqualTo((SvenskaKronor)ingåendeBelopp));
			});
		}

		[Test]
		[TestCase(0.123, 0, 0.12)]
		[TestCase(0.5544, 1, 0.55)]
		[TestCase(0.9, 1, 0.9)]
		[TestCase(1.001, 1, 1.00)]
		[TestCase(1.005, 1, 1.01)]
		public void TestaSvenskaKronor2(decimal ingåendeVärde, decimal förväntadeHelaKronor, decimal förväntadeHelaKronorOchÖren)
		{
			static void Test(SvenskaKronor? p)
			{
				Assert.That(p, Is.Not.Null);
			}

			SvenskaKronor ingåendeSek = ingåendeVärde;
			SvenskaKronor sek1 = ingåendeSek.AvrundaHelaKronor;
			SvenskaKronor sek2 = ingåendeSek.AvrundaHelaKronorOchÖren;
			Assert.Multiple(() =>
			{
				Test(ingåendeVärde);
				Assert.That(sek1, Is.EqualTo((SvenskaKronor)förväntadeHelaKronor));
				Assert.That(sek2, Is.EqualTo((SvenskaKronor)förväntadeHelaKronorOchÖren));
			});
		}

		[Test]
		[TestCase("4QDKqvHAPEldy3ijc1HX95")]
		[TestCase("xBIimss7CkAszsCokUzHv")]
		public void TestaUnikIdentifierare1(string strängVärde)
		{
			string s = strängVärde;
			UnikIdentifierare unikId1 = s;
			string t = unikId1;
			UnikIdentifierare unikId2 = t;

			Assert.Multiple(() =>
			{
				Assert.That(s, Is.EqualTo(t));
				Assert.That(unikId1, Is.EqualTo(unikId2));
				Assert.That(s, Is.EqualTo((string)unikId1));
				Assert.That(t, Is.EqualTo((string)unikId1));
				Assert.That(s, Is.EqualTo((string)unikId2));
				Assert.That(t, Is.EqualTo((string)unikId2));

				Assert.That(unikId1, Is.EqualTo(unikId2));
				Assert.That(s, Is.EqualTo((string)unikId2));
				Assert.That(unikId1, Is.EqualTo((UnikIdentifierare)t));

				TestaSerialiseringTurOchRetur(unikId1);
				TestaSerialiseringTurOchRetur(unikId2);
			});
		}

		[TestCase("4QDKqvHAPEldy3ijc1HX95")]
		[TestCase("xBIimss7CkAszsCokUzHv")]
		public void TestaUnikIdentifierare2(string strängVärde)
		{
			UnikIdentifierare ui1 = strängVärde;
			Guid g1 = ui1;
			Assert.That(strängVärde, Is.EqualTo((string)ui1));	
			Assert.That(ui1, Is.EqualTo((UnikIdentifierare)g1));	
			Assert.That(g1, Is.EqualTo((Guid)ui1));	
		}

		static void TestaSerialiseringTurOchRetur<T>(T ingåendeVärde)
		{
			var json = JsonSerializer.Serialize(ingåendeVärde);
			var tillbaka = JsonSerializer.Deserialize<T>(json);
			Assert.That(tillbaka, Is.EqualTo(ingåendeVärde));
		}
	}
}
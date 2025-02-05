using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using DapperBeer.DTO;
using DapperBeer.Model;
using FluentAssertions;
using TUnit.Core;

namespace DapperBeer;

using Dapper;

public class Assignments1 : TestHelper
{
    // 1 Question
    // Geef een overzicht van alle brouwers, gesorteerd op naam (alfabetisch).
    // Gebruik hiervoor de class Brewer. En de Query<Brewer>(sql) methode van Dapper.
    // Het is ook altijd een goed idee om het resultaat van de Query om te zetten naar een `List<T>`, 
    // Dit kan met de methode ToList() die je aan het einde van de Query() kan toevoegen (connection.Query(sql).ToList()).
    // Onder deze methode staat een test. Zodat je kan controleren of je query en C# code correct werkt.
    // Tip: mocht je een foutmelding krijgen, kijk dan goed naar de foutmelding en de query die je hebt geschreven.
    // Test eerst je SQL-query en kijk dan of je C# code correct werkt met de debugger.
    // Kom je er niet uit na zelf proberen,
    // vraag dan hulp (medestudent, docent). Het kan natuurlijk zijn dat de test niet correct is.
    // Dit is natuurlijk niet de bedoeling (mocht dit zo zijn laat het mij (joris) dan s.v.p. weten). 
    //
    // De test zijn gemaakt met TUnit, dit testframework is relatief nieuw.
    // Het kan zijn dat je even een setting moet aanpassen in Rider/Visual Studio. Dit staat beschreven in de README.md.
    // op https://github.com/thomhurst/TUnit
    // LET OP: je kan de testen per stuk runnen, maar ook allemaal tegelijk per Assignment.
    // Maar niet alle assignments (Assigment1.cs, Assignment2.cs, Assignment3.cs) tegelijk!
    // Deze testen zijn niet geschreven om parallel te runnen. Dit zorgt er wel voor dat de testen sneller runnen.
    // 
    // In de directory Model staan de classes die overeenkomen met de database tabellen.
    // In de directory DTO (Data Transfer Object) staan de classes die worden gebruikt als resultaat indien deze
    // niet overeenkomt met een database tabel (Model).
    public static List<Brewer> GetAllBrewers()
    {
        var connection = DbHelper.GetConnection();
        return connection.Query<Brewer>("SELECT * FROM Brewer").ToList();
    }
    
    // 1 Test
    [Test]
    public  async Task GetAllBrewersTest()
    {
        List<Brewer> brewers = GetAllBrewers();
        
        brewers.Should().HaveCount(677);

        await Verify(brewers.Take(3));
    }
}
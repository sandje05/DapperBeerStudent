using System.Data;
using Dapper;
using DapperBeer.DTO;
using DapperBeer.Model;
using FluentAssertions;
using SqlKata;

namespace DapperBeer;

public class Assignments2
{
    [Before(Class)]
    public static void CreateAndPopulateDatabase()
    {
        DbHelper.CreateTablesAndInsertData();
    }
    
    // !!!Doe dit nooit!!!!!
    // Wees voorzichtig en doe dit in de praktijk nooit!!!!!!
    // We gaan SQL-injectie gebruiken om te laten zien hoe het werkt.
    // Maak de methode die alle bieren teruggeeft, gegeven het land.
    // Normaal gesproken zouden we hiervoor een WHERE Country = @Country gebruiken in de sql.
    // Dit is veilig omdat er een query parameter placeholder wordt gebruikt.
    // Echter, we gaan nu het volgende doen:
    //  string sql = $"SELECT Name FROM ... WHERE Country = '{country}'";
    // Dit is onveilig en zal SQL-injectie veroorzaken!
    // Hetzelfde geldt overigens ook voor:
    //  string sql = "SELECT Name FROM ... WHERE Country = '" + country + "'";
    // Kijk goed naar de test, het tweede deel van de test doet een SQL-injectie en we krijgen dus toch alle bieren terug.
    // Ook al zou normaal gesproken dit niet de intentie zijn.
    // Dit is een voorbeeld van een SQL-injectie, dit geval is niet zo gevaarlijk, maar het kan veel gevaarlijker zijn
    // omdat we op de plek waar de SQL-injectie plaatsvindt, alles kunnen doen wat we willen
    // (DELETE, DROP, SELECT van andere tabellen, etc.)!
    // Met andere woorden gebruik altijd query parameters en nooit string concatenatie om SQL-queries te maken.
    // !!!DOE DIT NOOIT MEER SVP!!!!
    public static List<string> GetBeersByCountryWithSqlInjection(string country)
    {
        throw new NotImplementedException();
    }
    
    [Test]
    public void GetBeersByCountryWithSqlInjectionTest()
    {
        List<string> beers = GetBeersByCountryWithSqlInjection("BEL");
        beers.Should().HaveCount(296);
        
        //sql injection test, we krijgen nu alle bieren terug 86 stuks ipv 7
        List<string> allBeersSQlInjection = GetBeersByCountryWithSqlInjection("BEL' OR '1' = '1");
        allBeersSQlInjection.Should().HaveCount(1617);
    }
    
    // Maak een methode die alle bieren teruggeeft, gegeven het land, echter het land kan ook leeg gelaten worden.
    // Sorteer de bieren op naam.
    // Als het land leeg is, dan moeten alle bieren teruggegeven worden.
    // Hiervoor kan je NULL-truc gebruiken in je SQL (zie de sheets).
    //      WHERE @Country IS NULL OR Country = @Country 
    // Het vraagteken bij 'GetAllBeersByCountry(string? country)' geeft aan dat het argument optioneel is (string? country).
    // Dit betekent dus dat country null kan zijn.
    public static List<string> GetAllBeersByCountry(string? country)
    {
        throw new NotImplementedException();
    }
    
    [Test]
    public async Task GetAllBeersByCountryTest()
    {
        List<string> beers = GetAllBeersByCountry("BEL");
        beers.Should().HaveCount(296);
        
        List<string> allBeers = GetAllBeersByCountry(null);
        allBeers.Should().HaveCount(1617);

        await Verify(allBeers.Take(3));
    }
    
    // Nu doen we hetzelfde als in de vorige opdracht GetAllBeersByCountry, echter voegen we een extra parameter toe,
    // het minimal alcoholpercentage.
    // Ook het minAlcohol kan leeg gelaten worden (decimal? minAlcohol).
    // Gebruikt >= (groter of gelijk aan) voor de vergelijking van het minAlcohol.
    public static List<string> GetAllBeersByCountryAndMinAlcohol(string? country = null, decimal? minAlcohol = null)
    {
        throw new NotImplementedException(); 
    }
    
    [Test]
    public void GetAllBeersByCountryAndMinAlcoholTest()
    {
        GetAllBeersByCountryAndMinAlcohol("BEL", 5.5m).Should().HaveCount(213);
        GetAllBeersByCountryAndMinAlcohol(minAlcohol: 5.5m).Should().HaveCount(626);
        GetAllBeersByCountryAndMinAlcohol(country: "BEL").Should().HaveCount(296);
        GetAllBeersByCountryAndMinAlcohol().Should().HaveCount(1617);
    }
    
    // Helaas kan je in SQL bijv. geen parameter gebruiken voor de ORDER BY.
    // Dit kan je oplossen door de SQL te bouwen met een StringBuilder of een SqlBuilder.
    // De SqlBuilder is een handige tool om SQL-queries te bouwen.
    //  Voor uitleg zie: https://github.com/DapperLib/Dapper/blob/main/Dapper.SqlBuilder/Readme.md
    
    // Maak onderstaande methode die bieren teruggeeft (gebruik onderstaande query).
    // SELECT beer.Name
    // FROM Beer beer 
    //     JOIN Brewer brewer ON beer.BrewerId = brewer.BrewerId 
    //     WHERE (@Country IS NULL OR Country = @Country) AND (@MinAlcohol IS NULL OR Alcohol >= @MinAlcohol)
    // ORDER BY beer.Name
    //
    // Echter, vervang de WHERE-clausule door gebruik te maken van de SqlBuilder.
    // 
    // Je krijgt dan dergelijke constructies in je code:
    // if(country != null) {
    //     builder = builder.Where("brewer.Country = @Country", new { Country = country });
    // }
    // De queryBuilder maakt het bijvoorbeeld mogelijk om een WHERE-clausule te bouwen met meerdere voorwaarden.
    // Dit kan handig voor het maken van een datagrid (tabel) waarbij de gebruiker meerdere filters / sorteringen kan gebruiken.
    
    // Voeg zelf code toe om minAlcohol te kunnen gebruiken in je WHERE clausule.
    // 
    // Voeg ook een ORDER BY toe, zodat de bieren op verschillende kolommen gesorteerd kunnen worden.
    // Hiervoor is de parameter (string orderBy = "beer.Name") toegevoegd aan de methode. 
    // Deze parameter heeft een default waarde. In de test kan je zien hoe deze methode aangeroepen wordt
    // (en dan met name naar de parameters).
    // Dit worden default parameters genoemd i.c.m. optionele parameters en named arguments. 
    // Om Dapper (en andere bibliotheken) te gebruiken, worden vaak bovenstaande technieken gebruikt voor het aanroepen van methoden.
    // Zo zie je maar dat er ook wat van je C#-skills verwacht wordt :-).
    public static List<string> GetAllBeersByCountryAndMinAlcoholOrderByWithSqlBuilder(
        string? country = null, decimal? minAlcohol = null, string orderBy = "beer.Name")
    {
        using IDbConnection connection = DbHelper.GetConnection();
        string sql = $"""
                      SELECT beer.Name
                      FROM Beer beer 
                           JOIN Brewer brewer ON beer.BrewerId = brewer.BrewerId 
                      /**where**/
                      /**orderby**/
                      """;
        
        SqlBuilder builder = new SqlBuilder();
        
        throw new NotImplementedException();
    }
    
    [Test]
    public void GetAllBeersByCountryAndTypeTest()
    {
        GetAllBeersByCountryAndMinAlcoholOrderByWithSqlBuilder("BEL", 5.5m).Should().HaveCount(213);
        GetAllBeersByCountryAndMinAlcoholOrderByWithSqlBuilder(minAlcohol: 5.5m).Should().HaveCount(626);
        GetAllBeersByCountryAndMinAlcoholOrderByWithSqlBuilder(country: "BEL").Should().HaveCount(296);
        GetAllBeersByCountryAndMinAlcoholOrderByWithSqlBuilder().Should().HaveCount(1617);
        
        IEnumerable<string> orderByName = GetAllBeersByCountryAndMinAlcoholOrderByWithSqlBuilder(minAlcohol: 10, country: "BEL", orderBy: "beer.Name").Take(2);
        orderByName.Should().BeEquivalentTo(["ADA 10", "BIERE DU BOUCANIER"]);
        IEnumerable<string> orderByAlcohol = GetAllBeersByCountryAndMinAlcoholOrderByWithSqlBuilder(orderBy: "Alcohol").Take(2);
        orderByAlcohol.Should().BeEquivalentTo(["GULL", "BERLINER WEISSE" ]);
        IEnumerable<string> orderByType = GetAllBeersByCountryAndMinAlcoholOrderByWithSqlBuilder(country: "BEL", orderBy: "Type").Take(2);
        orderByType.Should().BeEquivalentTo(["MAREDSOUS 8", "FLOREFFE" ]);
    }

    
    // Maak een view die de naam van het bier teruggeeft en de naam van de brouwerij
    // en de naam van de brouwmeester als de brouwerij deze heeft (LEFT JOIN).
    // Sorteer de resultaten op bier naam.
    //
    // Gebruik de volgende where-clause: WHERE BrewmasterName IS NOT NULL (in de query niet in de view)
    // Gebruik de klasse BrewerBeerBrewmaster om de resultaten in op te slaan. (directory DTO).
    public static List<BrewerBeerBrewmaster> GetAllBeerNamesWithBreweryAndBrewmaster()
    {
        throw new NotImplementedException();
    }
    
    [Test]
    public async Task GetAllBeerNamesWithBreweryAndBrewmasterTest()
    {
        List<BrewerBeerBrewmaster> result = GetAllBeerNamesWithBreweryAndBrewmaster();
        result.Should().HaveCount(152);
        await Verify(result.Take(3));
    }
    
    // Soms is het onhandig om veel parameters mee te geven aan een methode.
    // Dit kan je oplossen door een klasse te maken die de parameters bevat.
    // De kan je rechtstreeks meegeven aan de Query<T>(sql, param: filter).
    // of aan SqlBuilder.Template? queryTemplate = builder.AddTemplate(sql, filter);
    // Maak een query die bieren teruggeeft, gegeven het land en het type (beide optioneel).
    // Ook willen we een limiet en een offset meegeven in de query.
    // LIMIT @PageSize OFFSET @Offset  
    // Sorteer op OrderBy
    // Zie de klasse BeerFilter.
    public class BeerFilter
    {
        public string? Country { get; set; }
        public string? Type { get; set; }
        public int PageSize { get; set; } = 10;    //default value start at 0
        public int PageIndex { get; set; } = 0;    //default value start at 0
        
        public int Offset => PageSize * (PageIndex+1);
        
        public string OrderBy { get; set; } = "beer.Name";
    }
    public static List<Beer> GetBeersByCountryAndType(BeerFilter filter)
    {
        using IDbConnection connection = DbHelper.GetConnection();
        string sql = $"""
                      SELECT beer.BeerId, beer.Name, beer.Type, beer.Style, beer.Alcohol, beer.BrewerId
                      FROM Beer beer 
                           JOIN Brewer brewer ON beer.BrewerId = brewer.BrewerId
                      /**where**/
                      /**orderby**/
                      LIMIT @PageSize OFFSET @Offset
                      """;
        
        SqlBuilder builder = new SqlBuilder();

        throw new NotImplementedException();
    }
    
    [Test]
    public async Task GetBeersByCountryAndTypeTest()
    {
        BeerFilter filter = new BeerFilter { Country = "BEL", Type = "LAGER, AMBE" };
        List<Beer> beers1 = GetBeersByCountryAndType(filter);
        beers1.Should().HaveCount(0);
        
        filter = new BeerFilter { Country = "BEL", PageSize = 5, PageIndex = 2};
        List<Beer> beers2 = GetBeersByCountryAndType(filter);
        beers2.Should().HaveCount(5);
        
        filter = new BeerFilter { OrderBy = "beer.Alcohol DESC" };
        List<Beer> beers3 = GetBeersByCountryAndType(filter);
        beers3.Should().HaveCount(10);
        
        filter = new BeerFilter { };
        List<Beer> beers4 = GetBeersByCountryAndType(filter);
        beers4.Should().HaveCount(10);
        
        List<Beer> testBeers = beers1.Take(3)
            .Concat(beers2.Take(3))
            .Concat(beers3.Take(3))
            .Concat(beers4.Take(3))
            .ToList();
        
        await Verify(testBeers);
    }
    
}
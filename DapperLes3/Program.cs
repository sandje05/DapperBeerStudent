// See https://aka.ms/new-console-template for more information

using Dapper;
using DapperBeer;
using DapperBeer.Model;

// BeerBrewerExample();

// BrewerBeerExample();

CafeSellsBeer();

//1 Beer has 1 Brewer 
#region BeerBrewerExample
void BeerBrewerExample() {
    
    string sql = """
                    SELECT b.BeerId, b.Name, b.Type, b.Style, b.Alcohol, b.BrewerId,
                           '' as BrewerSplit,
                           br.BrewerId, br.Name, br.Country 
                    FROM Beer b
                    JOIN Brewer br ON b.BrewerId = br.BrewerId 
                    ORDER BY br.Name
                    LIMIT 5
                 """;

    var brewerDict = new Dictionary<int, Brewer>();

    var connection = DbHelper.GetConnection();
    var beersWithBrewers = connection.Query<Beer, Brewer, Beer>(sql, 
        map: (Beer beer, Brewer brewer) =>
        {
            brewerDict.TryAdd(brewer.BrewerId, brewer);
            
            brewer = brewerDict[brewer.BrewerId];
            
            beer.Brewer = brewer;
            return beer;
        }, 
        splitOn: "BrewerSplit")
        .ToList();

    foreach (var beer in beersWithBrewers)
    {
        Console.WriteLine($"{beer.Name} -- {beer.Brewer.Name}");
    }
}
#endregion

//1 Brewer has Many beers (1 to many)
#region BrewerBeer 
void BrewerBeerExample()
{
    string sql = """
                    SELECT br.BrewerId, br.Name, br.Country,
                           '' as BeerSplit,
                           b.BeerId, b.Name, b.Type, b.Style, b.Alcohol, b.BrewerId
                    FROM Brewer br 
                        JOIN Beer b ON br.BrewerId = b.BrewerId
                    ORDER BY br.Name, br.BrewerId
                    LIMIT 4
                 """;

    var connection = DbHelper.GetConnection();

    var brewerDict = new Dictionary<int, Brewer>();
    var brewersWithBeers = connection.Query<Brewer, Beer, Brewer>(
        sql, map: (Brewer brewer, Beer beer) =>
        {
            brewerDict.TryAdd(brewer.BrewerId, brewer);
            
            brewer = brewerDict[brewer.BrewerId];
                
            brewer.Beers.Add(beer);
            
            return brewer;
        },
        splitOn: "BeerSplit")
            .ToList().Distinct();

    foreach (var brewer in brewersWithBeers)
    {
        var beerNames = string.Join(",", brewer.Beers.Select(b => b.Name));
        Console.WriteLine($"{brewer.Name} -- {beerNames}");
    }
}
#endregion

//n - to - n between cafe and beer (sells)
#region CafeSellsBeer
void CafeSellsBeer()
{
    string sql =
        """
            SELECT c.CafeId, c.Name, c.Address, c.City,
                   '' AS BeerSplit,
                   b.BeerId, b.Name, b.Type, b.Style, b.Alcohol, b.BrewerId
            FROM 
                Cafe c 
                    JOIN Sells s on c.CafeId = s.CafeId
                        JOIN Beer b on s.BeerId = b.BeerId
            WHERE c.CafeId IN (9, 116)
            ORDER BY c.Name, c.CafeId, b.Name, b.BeerId
            -- cafe CASABLANCA (9) en cafe De KRAM (116) hebben beide het bier ASTRA (25) 
        """;

    var cafeDict = new Dictionary<int, Cafe>();
    var beerDict = new Dictionary<int, Beer>();
    
    var connection = DbHelper.GetConnection();
    var cafeWithBeers = connection.Query<Cafe, Beer, Cafe>(
        sql,
        map: (Cafe cafe, Beer beer) =>
        {
            cafeDict.TryAdd(cafe.CafeId, cafe);
            beerDict.TryAdd(beer.BeerId, beer);

            beer = beerDict[beer.BeerId];
            cafe = cafeDict[cafe.CafeId];
            
            cafe.Beers.Add(beer);
            beer.Cafes.Add(cafe);
            
            return cafe;
        },
        splitOn: "BeerSplit"
    ).ToList().Distinct();

    foreach (var cafe in cafeWithBeers)
    {
        var beerNames = string.Join(",", cafe.Beers.Select(x => x.Name));
        Console.WriteLine($"{cafe.Name} -- {beerNames}");
    }
}
#endregion


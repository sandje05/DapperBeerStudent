// // See https://aka.ms/new-console-template for more information
//
using Dapper;
using DapperBeer;
using DapperBeer.Model;

#region SQLInjectionPlusSolution
SqlInjectionExample();
SqlInjectionSolution();

void SqlInjectionExample()
{
    using var connection = DbHelper.GetConnection();
    string name = "ZIPPIL' OR 1=1 -- ";
    var beers = connection
        .Query<Beer>($"SELECT * FROM Beer WHERE Name = '{name}' ORDER BY Name")
        .ToList();

    foreach (var beer in beers)
    {
        Console.WriteLine(beer.Name);
    }    
}

//Query parameters placeholders in your SQL query
//Take a close look at @name in the query
//This is a parameter placeholder
//It is replaced by the value of the name variable
//This way you can prevent SQL injection
// @name doesn't include the single quote '' in the value,
// Dapper knows the type of the parameter and will escape it properly
void SqlInjectionSolution()
{
    using var connection = DbHelper.GetConnection();
    string name = "ZIPPIL' OR 1=1 -- "; //the space at the end is important!
    var beers = connection
        .Query<Beer>($"SELECT * FROM Beer WHERE Name = @name ORDER BY Name",
            param: new { name })
        .ToList();

    foreach (var beer in beers)
    {
        Console.WriteLine(beer.Name);
    }    
}

#endregion

#region Parameters
void ParametersExampleAnd()
{
    string sql = """
                 SELECT * FROM Beer
                 WHERE Style = @style AND 
                       Alcohol > @alcohol   ORDER BY Name
                 """;
    
    using var connection = DbHelper.GetConnection();
    var beers = connection
        .Query<Beer>(sql, new { style = "Ale", alcohol = 6.0 })
        .ToList();
}
#endregion

#region OptionalAndNamedParameter

ParametersExampleWithNull();
ParametersExampleWithNull("QUADRUPEL", 3.0);
ParametersExampleWithNull(null, 3.0);
ParametersExampleWithNull(alcohol: 3.0);
ParametersExampleWithNull(alcohol: 3.0, style: "QUADRUPEL");

//ParameterExample with NULL Trick and optional Parameters
void ParametersExampleWithNull(string? style = null, double? alcohol = null)
{
    string sql = """
                 SELECT * 
                 FROM Beer
                 WHERE 
                       (@style IS NULL OR Style = @style) 
                       AND 
                       (@alcohol IS NULL OR Alcohol > @alcohol) 
                 ORDER BY Name
                 """;
    
    using var connection = DbHelper.GetConnection();
    var beers = connection
        // .Query<Beer>(sql, new { style = "Ale", alcohol = 6.0 })
        .Query<Beer>(sql, param: new { style, alcohol })
        .ToList();

    foreach (var beer in beers)
    {
        Console.WriteLine($"{beer.Name} -- {beer.Style} -- {beer.Alcohol}");
    }
}

#endregion

#region OneRecordOrZero
Beer? beerWithId1 = GetBeerById(8);
Beer? beerWithNonExtisingId = GetBeerById(-1);

Beer? GetBeerById(int beerId)
{
    string sql = "SELECT * FROM Beer WHERE BeerId = @beerId";
    using var connection = DbHelper.GetConnection();
    Beer? beer = connection.QueryFirstOrDefault<Beer>(sql, new { beerId });

    return beer;
}
#endregion

#region OneValue
Console.WriteLine(GetMaxAlcoholBeer());

double GetMaxAlcoholBeer()
{
    string sql = "SELECT MAX(alcohol) FROM Beer";
    
    using var connection = DbHelper.GetConnection();
    var maxAlcohol = connection.QueryFirst<double>(sql);
    return maxAlcohol;
}
#endregion

#region Inserts

// Insert Into Beer
int brewerId = 680;
Beer beer1 = InsertBeer1("Test Beer 1", "Type 1", "Style 1", 100, brewerId);
Beer beer2 = InsertBeer2("Test Beer 2", "Type 2", "Style 2",  100, brewerId);
Beer beer3 = InsertBeer3("Test Beer 3", "Type 3", "Style 3", 100, brewerId);

var beerInsertParameters = new BeerInsertParameters()
{
    Name = "Test Beer 4",
    Type = "Type 1",
    Style = "Ale",
    Alcohol = 10,
    BrewerId = brewerId
};
Beer beer4 = InsertBeer4(beerInsertParameters);

Beer InsertBeer1(string name, string type, string style, double alcohol, int brewerId)
{
    string sql = @"
                 INSERT INTO Beer (Name, Type, Style, Alcohol, BrewerId)
                 VALUES (@Name, @Type, @Style, @Alcohol, @BrewerId);
                 SELECT * FROM Beer WHERE BeerId = LAST_INSERT_ID()
                 ";
    
    using var connection = DbHelper.GetConnection();
    var newBeer = connection.QuerySingle<Beer>(sql, 
        new { name, type, style, alcohol, brewerId });
    return newBeer;
}

Beer InsertBeer2(string name, string type, string style, double alcohol, int brewerId)
{
    string sql = @"
                 INSERT INTO Beer (Name, Type, Style, Alcohol, BrewerId)
                 VALUES (@name, @type, @style, @alcohol, @brewerId);
                    SELECT * FROM Beer WHERE BeerId = LAST_INSERT_ID();                 
                 ";
    
    using var connection = DbHelper.GetConnection();
    var newBeer = connection.QuerySingle<Beer>(sql, new { name, type, style, alcohol, brewerId });
    return newBeer;
}

Beer InsertBeer3(string name, string type, string style, double alcohol, int brewerId)
{
    string sql = @"
                 INSERT INTO Beer (Name, Type, Style, Alcohol, BrewerId)
                 VALUES (@name, @type, @style, @alcohol, @brewerId);
                 SELECT * FROM Beer WHERE BeerId = LAST_INSERT_ID();
                 ";
    
    using var connection = DbHelper.GetConnection();
    var beer = connection.QuerySingle<Beer>(sql, new { name, type, style, alcohol, brewerId });
    return beer;
}

Beer InsertBeer4(BeerInsertParameters beerParams)
{
    string sql = @"
                 INSERT INTO Beer (Name, Type, Style, Alcohol, BrewerId)
                 VALUES (@Name, @Type, @Style, @Alcohol, @BrewerId);
                 SELECT * FROM Beer WHERE BeerId = LAST_INSERT_ID();
                 ";
    
    using var connection = DbHelper.GetConnection();
    var beer = connection.QuerySingle<Beer>(sql, beerParams);
    return beer;
}

int countOfRecordsUpdated = UpdateBeer(4, "Zippil2", "Ale!", 5.9);

int UpdateBeer(int id, string name, string style, double alcohol)
{
    string sql = @"
                 UPDATE Beer
                 SET Name = @name, Style = @style, Alcohol = @alcohol
                 WHERE BeerId = @id
                 ";
    
    using var connection = DbHelper.GetConnection();
    var numberOfRecordsUpdated = connection.Execute(sql, new { id, name, style, alcohol });
    Console.WriteLine($"Number of records updated: {numberOfRecordsUpdated}");
    return numberOfRecordsUpdated;
}
#endregion

class BeerInsertParameters
{
    public string Name { get; set; }
    public string Type { get; set; }
    public string Style { get; set; }
    public double Alcohol { get; set; }
    public int BrewerId { get; set; }
}


    
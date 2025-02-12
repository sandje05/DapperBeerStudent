namespace DapperBeer.Tests;

public class TestHelper
{
    [OneTimeSetUp]
    public static void CreateAndPopulateDatabase()
    {
        DbHelper.CreateTablesAndInsertData();
    }   
}
// See https://aka.ms/new-console-template for more information

using Dapper;
using MySqlConnector;

Console.WriteLine("Hello, World!");

string conectionString = "server=localhost;uid=root;pwd=Test@1234!;database=Todo";
MySqlConnection connection = new MySqlConnection(conectionString);

List<Todo> todos = connection.Query<Todo>("SELECT * FROM Todo").ToList();

foreach (var todo in todos)
{
    Console.WriteLine(todo.Name + " " + todo.Completed);
}

public class Todo
{
    public int TodoId { get; set; }
    public string Name { get; set; }
    public bool Completed { get; set; } 
}
using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
// Import other necessary namespaces...

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
                webBuilder.UseUrls("http://localhost:5002"); // specify your URL here
            });
}

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(
                builder =>
                {
                    builder.WithOrigins("http://localhost:3000") // replace with your frontend URL
                           .AllowAnyHeader()
                           .AllowAnyMethod();
                });
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();

        app.UseCors();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}

public class User
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? PasswordHash { get; set; }
    public string? Role { get; set; }
    public decimal Budget { get; set; }
}

public class Purchase
{
    public int Id { get; set; }
    public User? User { get; set; }
    public string? ItemName { get; set; }
    public decimal Price { get; set; }
    public DateTime Date { get; set; }
    public string? Category { get; set; }
}

public class Summary
{
    public string? Month { get; set; }
    public decimal TotalSpending { get; set; }
}

public class DataAccessLayer
{
    private MySqlConnection connection;

    public DataAccessLayer()
    {
        string connectionString = "server=mari.vamk.fi;user=e2101098;password=cqgYeaFEN6A;database=e2101098_Windows;charset=utf8mb4";
        connection = new MySqlConnection(connectionString);
    }

    public void AddGroceryItem(string itemName, decimal price, DateTime date)
    {
        string query = "INSERT INTO Purchase (ItemName, Price, Date) VALUES (@itemName, @price, @date)";

        using (MySqlCommand command = new MySqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@itemName", itemName);
            command.Parameters.AddWithValue("@price", price);
            command.Parameters.AddWithValue("@date", date);

            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }
    }

    public List<Purchase> GetPriceHistory(string itemName)
    {
        List<Purchase> purchases = new List<Purchase>();

        string query = "SELECT * FROM Purchase WHERE ItemName = @itemName ORDER BY Date";

        using (MySqlCommand command = new MySqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@itemName", itemName);

            connection.Open();

            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Purchase purchase = new Purchase
                    {
                        Id = reader.GetInt32("Id"),
                        User = new User { Id = reader.GetInt32("UserId") }, // Replace with your actual logic for getting the User
                        ItemName = reader.GetString("ItemName"),
                        Price = reader.GetDecimal("Price"),
                        Date = reader.GetDateTime("Date"),
                        Category = reader.GetString("Category")
                        // Set other properties...
                    };
                    purchases.Add(purchase);
                }
            }

            connection.Close();
        }

        return purchases;
    }

    public List<Summary> GetMonthlySpending()
    {
        List<Summary> summaries = new List<Summary>();

        string query = @"
            SELECT DATE_FORMAT(Date, '%Y-%m') as Month, SUM(Price) as TotalSpending
            FROM Purchase
            GROUP BY Month
            ORDER BY Month";

        using (MySqlCommand command = new MySqlCommand(query, connection))
        {
            connection.Open();

            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Summary summary = new Summary
                    {
                        Month = reader.GetString("Month"),
                        TotalSpending = reader.GetDecimal("TotalSpending")
                        // Set other properties...
                    };
                    summaries.Add(summary);
                }
            }

            connection.Close();
        }

        return summaries;
    }

    public void SetBudget(int userId, decimal budget)
    {
        string query = "INSERT INTO User (Id, Budget) VALUES (@userId, @budget)";

        using (MySqlCommand command = new MySqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@userId", userId);
            command.Parameters.AddWithValue("@budget", budget);

            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }
    }

    public void UpdateBudget(int userId, decimal budget)
    {
        string query = "UPDATE User SET Budget = @budget WHERE Id = @userId";

        using (MySqlCommand command = new MySqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@userId", userId);
            command.Parameters.AddWithValue("@budget", budget);

            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }
    }

    public decimal CalculateRemainingBudget(int userId)
    {
        decimal budget = 0;
        decimal totalSpending = 0;

        string budgetQuery = "SELECT Budget FROM User WHERE Id = @userId";

        using (MySqlCommand command = new MySqlCommand(budgetQuery, connection))
        {
            command.Parameters.AddWithValue("@userId", userId);

            connection.Open();

            using (MySqlDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    budget = reader.GetDecimal("Budget");
                }
            }

            connection.Close();
        }

        string spendingQuery = "SELECT SUM(Price) as TotalSpending FROM Purchase WHERE UserId = @userId";

        using (MySqlCommand command = new MySqlCommand(spendingQuery, connection))
        {
            command.Parameters.AddWithValue("@userId", userId);

            connection.Open();

            using (MySqlDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    totalSpending = reader.IsDBNull(reader.GetOrdinal("TotalSpending")) ? 0 : reader.GetDecimal("TotalSpending");
                }
            }

            connection.Close();
        }

        return budget - totalSpending;
    }

    // Implement other methods to interact with the database (e.g., GetPurchases, UpdatePurchase, DeletePurchase)
}

[ApiController]
[Route("[controller]")]
public class ItemController : ControllerBase
{
    private DataAccessLayer dataAccessLayer;

    public ItemController()
    {
        dataAccessLayer = new DataAccessLayer();
    }

    [HttpPost]
    public ActionResult AddGroceryItem(string itemName, decimal price, DateTime date)
    {
        dataAccessLayer.AddGroceryItem(itemName, price, date);
        return Ok();
    }

    [HttpGet("{itemName}")]
    public ActionResult<List<Purchase>> GetPriceHistory(string itemName)
    {
        List<Purchase> purchases = dataAccessLayer.GetPriceHistory(itemName);
        if (purchases == null)
        {
            return NotFound();
        }
        return purchases;
    }

    [HttpGet("summary")]
    public ActionResult<List<Summary>> GetMonthlySpending()
    {
        List<Summary> summaries = dataAccessLayer.GetMonthlySpending();
        if (summaries == null)
        {
            return NotFound();
        }
        return summaries;
    }

    [HttpPost("budget")]
    public ActionResult SetBudget(int userId, decimal budget)
    {
        dataAccessLayer.SetBudget(userId, budget);
        return Ok();
    }

    [HttpPut("budget")]
    public ActionResult UpdateBudget(int userId, decimal budget)
    {
        dataAccessLayer.UpdateBudget(userId, budget);
        return Ok();
    }

    [HttpGet("{userId}/remainingBudget")]
    public ActionResult<decimal> CalculateRemainingBudget(int userId)
    {
        decimal remainingBudget = dataAccessLayer.CalculateRemainingBudget(userId);
        return Ok(remainingBudget);
    }

    // Implement other API endpoints (e.g., GET (all), POST, PUT, DELETE)
}

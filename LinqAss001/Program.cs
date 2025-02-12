using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Assignment02_LINQ
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Executing LINQ queries...");
            LINQQueries queries = new LINQQueries();
            queries.RunAllQueries();
        }
    }

    #region LINQ Queries Class
    class LINQQueries
    {
        private List<Product> products;
        private List<Customer> customers;
        private string[] dictionaryWords;

        public LINQQueries()
        {
            products = ListGenerators.GetProducts();
            customers = ListGenerators.GetCustomers();
            dictionaryWords = File.Exists("dictionary_english.txt") ? File.ReadAllLines("dictionary_english.txt") : new string[0];
        }

        public void RunAllQueries()
        {
            // Aggregate Operators
            Console.WriteLine("\nTotal Units in Stock per Category:");
            var totalUnitsInStock = products.GroupBy(p => p.Category)
                .Select(g => new { Category = g.Key, TotalStock = g.Sum(p => p.UnitsInStock) });
            foreach (var item in totalUnitsInStock) Console.WriteLine($"{item.Category}: {item.TotalStock}");

            Console.WriteLine("\nCheapest Product per Category:");
            var cheapestProducts = products.GroupBy(p => p.Category)
                .Select(g => g.OrderBy(p => p.Price).First());
            foreach (var product in cheapestProducts) Console.WriteLine($"{product.Category}: {product.Name} - {product.Price:C}");

            Console.WriteLine("\nMost Expensive Product per Category:");
            var expensiveProducts = products.GroupBy(p => p.Category)
                .Select(g => g.OrderByDescending(p => p.Price).First());
            foreach (var product in expensiveProducts) Console.WriteLine($"{product.Category}: {product.Name} - {product.Price:C}");

            Console.WriteLine("\nAverage Price per Category:");
            var averagePrices = products.GroupBy(p => p.Category)
                .Select(g => new { Category = g.Key, AvgPrice = g.Average(p => p.Price) });
            foreach (var item in averagePrices) Console.WriteLine($"{item.Category}: {item.AvgPrice:C}");

            // Set Operators
            Console.WriteLine("\nUnique First Letters from Product and Customer Names:");
            var uniqueLetters = products.Select(p => p.Name[0]).Union(customers.Select(c => c.Name[0]));
            foreach (var letter in uniqueLetters) Console.WriteLine(letter);

            Console.WriteLine("\nFirst Letters of Product Names Not in Customers:");
            var productUniqueLetters = products.Select(p => p.Name[0]).Except(customers.Select(c => c.Name[0]));
            foreach (var letter in productUniqueLetters) Console.WriteLine(letter);

            // Partitioning Operators
            Console.WriteLine("\nFirst 3 Orders from Customers in Washington:");
            var first3OrdersWA = customers.Where(c => c.City == "Washington").SelectMany(c => c.Orders).Take(3);
            foreach (var order in first3OrdersWA) Console.WriteLine($"OrderID: {order.OrderID}, Total: {order.Total:C}");

            // Quantifiers
            Console.WriteLine("\nAny Word in Dictionary Containing 'ei':");
            var wordsWithEi = dictionaryWords.Any(word => word.Contains("ei"));
            Console.WriteLine(wordsWithEi ? "Yes" : "No");

            Console.WriteLine("\nCategories with At Least One Product Out of Stock:");
            var outOfStockCategories = products.Where(p => p.UnitsInStock == 0).Select(p => p.Category).Distinct();
            foreach (var category in outOfStockCategories) Console.WriteLine(category);
        }
    }
    #endregion

    #region Product Class
    class Product
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public int UnitsInStock { get; set; }
    }
    #endregion

    #region Customer Class
    class Customer
    {
        public string Name { get; set; }
        public string City { get; set; }
        public List<Order> Orders { get; set; }
    }
    #endregion

    #region Order Class
    class Order
    {
        public int OrderID { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal Total { get; set; }
    }
    #endregion

    #region ListGenerators Class
    static class ListGenerators
    {
        public static List<Product> GetProducts()
        {
            return new List<Product>
            {
                new Product { Name = "Laptop", Category = "Electronics", Price = 1200, UnitsInStock = 10 },
                new Product { Name = "Phone", Category = "Electronics", Price = 800, UnitsInStock = 0 },
                new Product { Name = "Table", Category = "Furniture", Price = 300, UnitsInStock = 5 },
                new Product { Name = "Chair", Category = "Furniture", Price = 100, UnitsInStock = 20 }
            };
        }

        public static List<Customer> GetCustomers()
        {
            return new List<Customer>
            {
                new Customer { Name = "Mohamed", City = "Washington", Orders = new List<Order>
                    {
                        new Order { OrderID = 1, OrderDate = DateTime.Now.AddDays(-10), Total = 200 },
                        new Order { OrderID = 2, OrderDate = DateTime.Now.AddDays(-5), Total = 500 }
                    }
                }
            };
        }
    }
    #endregion
}

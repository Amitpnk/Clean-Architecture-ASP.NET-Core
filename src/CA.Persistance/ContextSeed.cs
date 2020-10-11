//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;

//namespace CA.Persistance
//{
//    public static class ContextSeed
//    {

//        public static void Seed(this ModelBuilder modelBuilder)
//        {

//            CreateCustomers(modelBuilder);

//            CreateEmployees(modelBuilder);

//            CreateProducts(modelBuilder);

//            CreateSales(modelBuilder);
//        }

//        private static void CreateCustomers(ModelBuilder modelBuilder)
//        {
//            List<Customer> customers = CustomersList();
//            modelBuilder.Entity<Customer>().HasData(customers);
//        }

//        private static void CreateEmployees(ModelBuilder modelBuilder)
//        {
//            modelBuilder.Entity<Employee>().HasData(EmployeeList());
//        }

//        private static void CreateProducts(ModelBuilder modelBuilder)
//        {
//            modelBuilder.Entity<Product>().HasData(ProductList());
//        }

//        private static List<Customer> CustomersList()
//        {
//            return new List<Customer>()
//            {
//                new Customer() { Id=1, Code ="1001", Name = "Martin Fowler" },
//                new Customer() { Id=2, Code ="1002", Name = "Uncle Bob" },
//                new Customer() { Id=3, Code ="1003", Name = "Kent Beck" },
//            };
//        }

//        private static List<Employee> EmployeeList()
//        {
//            return new List<Employee>()
//            {
//                new Employee() {Id=1,  Name = "Eric Evans" },
//                new Employee() {Id=2,  Name = "Greg Young" },
//                new Employee() {Id=3,  Name = "Udi Dahan" }
//            };
//        }

//        private static List<Product> ProductList()
//        {
//            return new List<Product>()
//            {
//             new Product() { Id=1,Name = "Spaghetti", Price = 5m },
//             new Product() { Id=2,Name = "Lasagna", Price = 10m },
//             new Product() { Id=3,Name = "Ravioli", Price = 15m }
//            };
//        }

//        private static void CreateSales(ModelBuilder modelBuilder)
//        {
//            modelBuilder.Entity<Sale>().HasData(
//                new Sale()
//                {
//                    Id = 1,
//                    Date = DateTime.Now.Date.AddDays(-3),
//                    CustomerId = CustomersList()[0].Id,
//                    EmployeeId = EmployeeList()[0].Id,
//                    ProductId = ProductList()[0].Id,
//                    UnitPrice = 5m,
//                    Quantity = 1
//                },

//                new Sale()
//                {
//                    Id = 2,
//                    Date = DateTime.Now.Date.AddDays(-2),
//                    CustomerId = CustomersList()[1].Id,
//                    EmployeeId = EmployeeList()[1].Id,
//                    ProductId = ProductList()[1].Id,
//                    UnitPrice = 10m,
//                    Quantity = 2
//                },

//                new Sale()
//                {
//                    Id = 3,
//                    Date = DateTime.Now.Date.AddDays(-1),
//                    CustomerId = CustomersList()[2].Id,
//                    EmployeeId = EmployeeList()[2].Id,
//                    ProductId = ProductList()[2].Id,
//                    UnitPrice = 15m,
//                    Quantity = 3
//                }
//            );

//        }
//    }
//}

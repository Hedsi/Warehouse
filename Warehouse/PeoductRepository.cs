using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WarehouseDAL.Interfaces;
using WarehouseDAL.Models;

namespace WarehouseDAL.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly string _connectionString;

        public ProductRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Product GetById(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            var command = new SqlCommand(
                "SELECT Id, Name, Description, Price, QuantityInStock, CreatedDate FROM Products WHERE Id = @Id",
                connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Product
                {
                    Id = reader.GetInt32("Id"),
                    Name = reader.GetString("Name"),
                    Description = reader.IsDBNull("Description") ? null : reader.GetString("Description"),
                    Price = reader.GetDecimal("Price"),
                    QuantityInStock = reader.GetInt32("QuantityInStock"),
                    CreatedDate = reader.GetDateTime("CreatedDate")
                };
            }
            return null;
        }

        public List<Product> GetAll()
        {
            var products = new List<Product>();

            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            var command = new SqlCommand(
                "SELECT Id, Name, Description, Price, QuantityInStock, CreatedDate FROM Products",
                connection);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                products.Add(new Product
                {
                    Id = reader.GetInt32("Id"),
                    Name = reader.GetString("Name"),
                    Description = reader.IsDBNull("Description") ? null : reader.GetString("Description"),
                    Price = reader.GetDecimal("Price"),
                    QuantityInStock = reader.GetInt32("QuantityInStock"),
                    CreatedDate = reader.GetDateTime("CreatedDate")
                });
            }
            return products;
        }

        public int Create(Product product)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            var command = new SqlCommand(
                @"INSERT INTO Products (Name, Description, Price, QuantityInStock, CreatedDate) 
          OUTPUT INSERTED.Id
          VALUES (@Name, @Description, @Price, @QuantityInStock, @CreatedDate)",
                connection);

            command.Parameters.AddWithValue("@Name", product.Name);
            command.Parameters.AddWithValue("@Description", (object)product.Description ?? DBNull.Value);
            command.Parameters.AddWithValue("@Price", product.Price);
            command.Parameters.AddWithValue("@QuantityInStock", product.QuantityInStock);

            // Перевірка дати
            if (product.CreatedDate < new DateTime(1753, 1, 1) || product.CreatedDate > new DateTime(9999, 12, 31))
            {
                product.CreatedDate = DateTime.Now;
            }
            command.Parameters.AddWithValue("@CreatedDate", product.CreatedDate);

            return (int)command.ExecuteScalar();
        }

        public bool Update(Product product)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            var command = new SqlCommand(
                @"UPDATE Products 
                  SET Name = @Name, Description = @Description, Price = @Price, QuantityInStock = @QuantityInStock
                  WHERE Id = @Id",
                connection);

            command.Parameters.AddWithValue("@Id", product.Id);
            command.Parameters.AddWithValue("@Name", product.Name);
            command.Parameters.AddWithValue("@Description", (object)product.Description ?? DBNull.Value);
            command.Parameters.AddWithValue("@Price", product.Price);
            command.Parameters.AddWithValue("@QuantityInStock", product.QuantityInStock);

            return command.ExecuteNonQuery() > 0;
        }

        public bool Delete(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            var command = new SqlCommand("DELETE FROM Products WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);

            return command.ExecuteNonQuery() > 0;
        }
    }
}
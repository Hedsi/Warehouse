using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WarehouseDAL.Interfaces;
using WarehouseDAL.Models;

namespace WarehouseDAL.Repositories
{
    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly string _connectionString;

        public OrderItemRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public OrderItem GetById(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            var command = new SqlCommand(
                @"SELECT oi.Id, oi.OrderId, oi.ProductId, oi.Quantity, oi.UnitPrice,
                         p.Name as ProductName, p.Description as ProductDescription
                  FROM OrderItems oi
                  INNER JOIN Products p ON oi.ProductId = p.Id
                  WHERE oi.Id = @Id",
                connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new OrderItem
                {
                    Id = reader.GetInt32("Id"),
                    OrderId = reader.GetInt32("OrderId"),
                    ProductId = reader.GetInt32("ProductId"),
                    Quantity = reader.GetInt32("Quantity"),
                    UnitPrice = reader.GetDecimal("UnitPrice"),
                    Product = new Product
                    {
                        Id = reader.GetInt32("ProductId"),
                        Name = reader.GetString("ProductName"),
                        Description = reader.IsDBNull("ProductDescription") ? null : reader.GetString("ProductDescription")
                    }
                };
            }
            return null;
        }

        public List<OrderItem> GetByOrderId(int orderId)
        {
            var orderItems = new List<OrderItem>();

            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            var command = new SqlCommand(
                @"SELECT oi.Id, oi.OrderId, oi.ProductId, oi.Quantity, oi.UnitPrice,
                         p.Name as ProductName, p.Description as ProductDescription
                  FROM OrderItems oi
                  INNER JOIN Products p ON oi.ProductId = p.Id
                  WHERE oi.OrderId = @OrderId",
                connection);
            command.Parameters.AddWithValue("@OrderId", orderId);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                orderItems.Add(new OrderItem
                {
                    Id = reader.GetInt32("Id"),
                    OrderId = reader.GetInt32("OrderId"),
                    ProductId = reader.GetInt32("ProductId"),
                    Quantity = reader.GetInt32("Quantity"),
                    UnitPrice = reader.GetDecimal("UnitPrice"),
                    Product = new Product
                    {
                        Id = reader.GetInt32("ProductId"),
                        Name = reader.GetString("ProductName"),
                        Description = reader.IsDBNull("ProductDescription") ? null : reader.GetString("ProductDescription")
                    }
                });
            }
            return orderItems;
        }

        public List<OrderItem> GetByProductId(int productId)
        {
            var orderItems = new List<OrderItem>();

            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            var command = new SqlCommand(
                @"SELECT oi.Id, oi.OrderId, oi.ProductId, oi.Quantity, oi.UnitPrice,
                         p.Name as ProductName
                  FROM OrderItems oi
                  INNER JOIN Products p ON oi.ProductId = p.Id
                  WHERE oi.ProductId = @ProductId",
                connection);
            command.Parameters.AddWithValue("@ProductId", productId);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                orderItems.Add(new OrderItem
                {
                    Id = reader.GetInt32("Id"),
                    OrderId = reader.GetInt32("OrderId"),
                    ProductId = reader.GetInt32("ProductId"),
                    Quantity = reader.GetInt32("Quantity"),
                    UnitPrice = reader.GetDecimal("UnitPrice"),
                    Product = new Product
                    {
                        Id = reader.GetInt32("ProductId"),
                        Name = reader.GetString("ProductName")
                    }
                });
            }
            return orderItems;
        }

        public int Create(OrderItem orderItem)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            var command = new SqlCommand(
                @"INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice) 
                  OUTPUT INSERTED.Id
                  VALUES (@OrderId, @ProductId, @Quantity, @UnitPrice)",
                connection);

            command.Parameters.AddWithValue("@OrderId", orderItem.OrderId);
            command.Parameters.AddWithValue("@ProductId", orderItem.ProductId);
            command.Parameters.AddWithValue("@Quantity", orderItem.Quantity);
            command.Parameters.AddWithValue("@UnitPrice", orderItem.UnitPrice);

            return (int)command.ExecuteScalar();
        }

        public bool Update(OrderItem orderItem)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            var command = new SqlCommand(
                @"UPDATE OrderItems 
                  SET OrderId = @OrderId, ProductId = @ProductId, 
                      Quantity = @Quantity, UnitPrice = @UnitPrice
                  WHERE Id = @Id",
                connection);

            command.Parameters.AddWithValue("@Id", orderItem.Id);
            command.Parameters.AddWithValue("@OrderId", orderItem.OrderId);
            command.Parameters.AddWithValue("@ProductId", orderItem.ProductId);
            command.Parameters.AddWithValue("@Quantity", orderItem.Quantity);
            command.Parameters.AddWithValue("@UnitPrice", orderItem.UnitPrice);

            return command.ExecuteNonQuery() > 0;
        }

        public bool Delete(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            var command = new SqlCommand("DELETE FROM OrderItems WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);

            return command.ExecuteNonQuery() > 0;
        }

        public bool DeleteByOrderId(int orderId)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            var command = new SqlCommand("DELETE FROM OrderItems WHERE OrderId = @OrderId", connection);
            command.Parameters.AddWithValue("@OrderId", orderId);

            return command.ExecuteNonQuery() > 0;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WarehouseDAL.Interfaces;
using WarehouseDAL.Models;

namespace WarehouseDAL.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly string _connectionString;
        private readonly IOrderItemRepository _orderItemRepository;

        public OrderRepository(string connectionString)
        {
            _connectionString = connectionString;
            _orderItemRepository = new OrderItemRepository(connectionString);
        }

        public Order GetById(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            var command = new SqlCommand(
                @"SELECT o.Id, o.SupplierId, o.OrderDate, o.Status, o.TotalAmount,
                         s.Name as SupplierName, s.ContactInfo, s.IsActive
                  FROM Orders o
                  INNER JOIN Suppliers s ON o.SupplierId = s.Id
                  WHERE o.Id = @Id",
                connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                var order = new Order
                {
                    Id = reader.GetInt32("Id"),
                    SupplierId = reader.GetInt32("SupplierId"),
                    OrderDate = reader.GetDateTime("OrderDate"),
                    Status = reader.GetString("Status"),
                    TotalAmount = reader.GetDecimal("TotalAmount"),
                    Supplier = new Supplier
                    {
                        Id = reader.GetInt32("SupplierId"),
                        Name = reader.GetString("SupplierName"),
                        ContactInfo = reader.IsDBNull("ContactInfo") ? null : reader.GetString("ContactInfo"),
                        IsActive = reader.GetBoolean("IsActive")
                    }
                };

                // Завантажуємо OrderItems
                order.OrderItems = _orderItemRepository.GetByOrderId(order.Id);
                return order;
            }
            return null;
        }

        public List<Order> GetAll()
        {
            var orders = new List<Order>();

            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            var command = new SqlCommand(
                @"SELECT o.Id, o.SupplierId, o.OrderDate, o.Status, o.TotalAmount,
                         s.Name as SupplierName, s.ContactInfo, s.IsActive
                  FROM Orders o
                  INNER JOIN Suppliers s ON o.SupplierId = s.Id",
                connection);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var order = new Order
                {
                    Id = reader.GetInt32("Id"),
                    SupplierId = reader.GetInt32("SupplierId"),
                    OrderDate = reader.GetDateTime("OrderDate"),
                    Status = reader.GetString("Status"),
                    TotalAmount = reader.GetDecimal("TotalAmount"),
                    Supplier = new Supplier
                    {
                        Id = reader.GetInt32("SupplierId"),
                        Name = reader.GetString("SupplierName"),
                        ContactInfo = reader.IsDBNull("ContactInfo") ? null : reader.GetString("ContactInfo"),
                        IsActive = reader.GetBoolean("IsActive")
                    }
                };

                // Завантажуємо OrderItems
                order.OrderItems = _orderItemRepository.GetByOrderId(order.Id);
                orders.Add(order);
            }
            return orders;
        }

        public List<Order> GetBySupplierId(int supplierId)
        {
            var orders = new List<Order>();

            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            var command = new SqlCommand(
                @"SELECT o.Id, o.SupplierId, o.OrderDate, o.Status, o.TotalAmount,
                         s.Name as SupplierName, s.ContactInfo, s.IsActive
                  FROM Orders o
                  INNER JOIN Suppliers s ON o.SupplierId = s.Id
                  WHERE o.SupplierId = @SupplierId",
                connection);
            command.Parameters.AddWithValue("@SupplierId", supplierId);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var order = new Order
                {
                    Id = reader.GetInt32("Id"),
                    SupplierId = reader.GetInt32("SupplierId"),
                    OrderDate = reader.GetDateTime("OrderDate"),
                    Status = reader.GetString("Status"),
                    TotalAmount = reader.GetDecimal("TotalAmount"),
                    Supplier = new Supplier
                    {
                        Id = reader.GetInt32("SupplierId"),
                        Name = reader.GetString("SupplierName"),
                        ContactInfo = reader.IsDBNull("ContactInfo") ? null : reader.GetString("ContactInfo"),
                        IsActive = reader.GetBoolean("IsActive")
                    }
                };

                // Завантажуємо OrderItems
                order.OrderItems = _orderItemRepository.GetByOrderId(order.Id);
                orders.Add(order);
            }
            return orders;
        }

        public int Create(Order order)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            var command = new SqlCommand(
                @"INSERT INTO Orders (SupplierId, OrderDate, Status, TotalAmount) 
          OUTPUT INSERTED.Id
          VALUES (@SupplierId, @OrderDate, @Status, @TotalAmount)",
                connection);

            command.Parameters.AddWithValue("@SupplierId", order.SupplierId);

            // Перевірка дати
            if (order.OrderDate < new DateTime(1753, 1, 1) || order.OrderDate > new DateTime(9999, 12, 31))
            {
                order.OrderDate = DateTime.Now;
            }
            command.Parameters.AddWithValue("@OrderDate", order.OrderDate);

            command.Parameters.AddWithValue("@Status", order.Status);
            command.Parameters.AddWithValue("@TotalAmount", order.TotalAmount);

            return (int)command.ExecuteScalar();
        }

        public bool Update(Order order)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            var command = new SqlCommand(
                @"UPDATE Orders 
          SET SupplierId = @SupplierId, OrderDate = @OrderDate, 
              Status = @Status, TotalAmount = @TotalAmount
          WHERE Id = @Id",
                connection);

            command.Parameters.AddWithValue("@Id", order.Id);
            command.Parameters.AddWithValue("@SupplierId", order.SupplierId);

            // Перевірка дати
            if (order.OrderDate < new DateTime(1753, 1, 1) || order.OrderDate > new DateTime(9999, 12, 31))
            {
                order.OrderDate = DateTime.Now;
            }
            command.Parameters.AddWithValue("@OrderDate", order.OrderDate);

            command.Parameters.AddWithValue("@Status", order.Status);
            command.Parameters.AddWithValue("@TotalAmount", order.TotalAmount);

            return command.ExecuteNonQuery() > 0;
        }

        public bool Delete(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            // Спочатку видаляємо всі OrderItems, пов'язані з цим замовленням
            _orderItemRepository.DeleteByOrderId(id);

            // Потім видаляємо саме замовлення
            var command = new SqlCommand("DELETE FROM Orders WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);

            return command.ExecuteNonQuery() > 0;
        }

        public decimal CalculateOrderTotal(int orderId)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            var command = new SqlCommand(
                "SELECT SUM(Quantity * UnitPrice) FROM OrderItems WHERE OrderId = @OrderId",
                connection);
            command.Parameters.AddWithValue("@OrderId", orderId);

            var result = command.ExecuteScalar();
            return result == DBNull.Value ? 0 : Convert.ToDecimal(result);
        }
    }
}
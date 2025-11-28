using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WarehouseDAL.Interfaces;
using WarehouseDAL.Models;

namespace WarehouseDAL.Repositories
{
    public class SupplierRepository : ISupplierRepository
    {
        private readonly string _connectionString;

        public SupplierRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Supplier GetById(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            var command = new SqlCommand(
                "SELECT Id, Name, ContactInfo, IsActive FROM Suppliers WHERE Id = @Id",
                connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Supplier
                {
                    Id = reader.GetInt32("Id"),
                    Name = reader.GetString("Name"),
                    ContactInfo = reader.IsDBNull("ContactInfo") ? null : reader.GetString("ContactInfo"),
                    IsActive = reader.GetBoolean("IsActive")
                };
            }
            return null;
        }

        public List<Supplier> GetAll()
        {
            var suppliers = new List<Supplier>();

            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            var command = new SqlCommand(
                "SELECT Id, Name, ContactInfo, IsActive FROM Suppliers",
                connection);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                suppliers.Add(new Supplier
                {
                    Id = reader.GetInt32("Id"),
                    Name = reader.GetString("Name"),
                    ContactInfo = reader.IsDBNull("ContactInfo") ? null : reader.GetString("ContactInfo"),
                    IsActive = reader.GetBoolean("IsActive")
                });
            }
            return suppliers;
        }

        public List<Supplier> GetActiveSuppliers()
        {
            var suppliers = new List<Supplier>();

            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            var command = new SqlCommand(
                "SELECT Id, Name, ContactInfo, IsActive FROM Suppliers WHERE IsActive = 1",
                connection);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                suppliers.Add(new Supplier
                {
                    Id = reader.GetInt32("Id"),
                    Name = reader.GetString("Name"),
                    ContactInfo = reader.IsDBNull("ContactInfo") ? null : reader.GetString("ContactInfo"),
                    IsActive = reader.GetBoolean("IsActive")
                });
            }
            return suppliers;
        }

        public int Create(Supplier supplier)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            var command = new SqlCommand(
                @"INSERT INTO Suppliers (Name, ContactInfo, IsActive) 
                  OUTPUT INSERTED.Id
                  VALUES (@Name, @ContactInfo, @IsActive)",
                connection);

            command.Parameters.AddWithValue("@Name", supplier.Name);
            command.Parameters.AddWithValue("@ContactInfo", (object)supplier.ContactInfo ?? DBNull.Value);
            command.Parameters.AddWithValue("@IsActive", supplier.IsActive);

            return (int)command.ExecuteScalar();
        }

        public bool Update(Supplier supplier)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            var command = new SqlCommand(
                @"UPDATE Suppliers 
                  SET Name = @Name, ContactInfo = @ContactInfo, IsActive = @IsActive
                  WHERE Id = @Id",
                connection);

            command.Parameters.AddWithValue("@Id", supplier.Id);
            command.Parameters.AddWithValue("@Name", supplier.Name);
            command.Parameters.AddWithValue("@ContactInfo", (object)supplier.ContactInfo ?? DBNull.Value);
            command.Parameters.AddWithValue("@IsActive", supplier.IsActive);

            return command.ExecuteNonQuery() > 0;
        }

        public bool Delete(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            var command = new SqlCommand("DELETE FROM Suppliers WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);

            return command.ExecuteNonQuery() > 0;
        }
    }
}
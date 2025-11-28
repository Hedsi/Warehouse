using System;
using System.Collections.Generic;

namespace WarehouseDAL.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int QuantityInStock { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class Supplier
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ContactInfo { get; set; }
        public bool IsActive { get; set; }
    }

    public class Order
    {
        public int Id { get; set; }
        public int SupplierId { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
        public Supplier Supplier { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }

    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public Product Product { get; set; }
        public Order Order { get; set; }
    }
}
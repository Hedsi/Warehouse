using System.Collections.Generic;
using WarehouseDAL.Models;

namespace WarehouseDAL.Interfaces
{
    public interface IProductRepository
    {
        Product GetById(int id);
        List<Product> GetAll();
        int Create(Product product);
        bool Update(Product product);
        bool Delete(int id);
    }

    public interface ISupplierRepository
    {
        Supplier GetById(int id);
        List<Supplier> GetAll();
        List<Supplier> GetActiveSuppliers();
        int Create(Supplier supplier);
        bool Update(Supplier supplier);
        bool Delete(int id);
    }

    public interface IOrderRepository
    {
        Order GetById(int id);
        List<Order> GetAll();
        List<Order> GetBySupplierId(int supplierId);
        int Create(Order order);
        bool Update(Order order);
        bool Delete(int id);
        decimal CalculateOrderTotal(int orderId);
    }

    public interface IOrderItemRepository
    {
        OrderItem GetById(int id);
        List<OrderItem> GetByOrderId(int orderId);
        List<OrderItem> GetByProductId(int productId);
        int Create(OrderItem orderItem);
        bool Update(OrderItem orderItem);
        bool Delete(int id);
        bool DeleteByOrderId(int orderId);
    }
}
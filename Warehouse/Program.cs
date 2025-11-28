using System;
using System.Collections.Generic;
using WarehouseDAL.Models;
using WarehouseDAL.Repositories;

namespace WarehouseConsoleApp
{
    class Program
    {
        private static readonly string connectionString = "Server=.;Database=WarehouseDB;Trusted_Connection=true;";
        private static ProductRepository productRepository;
        private static SupplierRepository supplierRepository;
        private static OrderRepository orderRepository;
        private static OrderItemRepository orderItemRepository;

        static void Main(string[] args)
        {
            productRepository = new ProductRepository(connectionString);
            supplierRepository = new SupplierRepository(connectionString);
            orderRepository = new OrderRepository(connectionString);
            orderItemRepository = new OrderItemRepository(connectionString);

            Console.WriteLine("=== Warehouse Management System ===");

            while (true)
            {
                ShowMainMenu();
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ProductMenu();
                        break;
                    case "2":
                        SupplierMenu();
                        break;
                    case "3":
                        OrderMenu();
                        break;
                    case "4":
                        RunDALTests();
                        break;
                    case "0":
                        Console.WriteLine("Goodbye!");
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        static void ShowMainMenu()
        {
            Console.WriteLine("\n=== MAIN MENU ===");
            Console.WriteLine("1. Products Management");
            Console.WriteLine("2. Suppliers Management");
            Console.WriteLine("3. Orders Management");
            Console.WriteLine("4. Run All DAL Tests");
            Console.WriteLine("0. Exit");
            Console.Write("Choose an option: ");
        }

        static void RunDALTests()
        {
            Console.WriteLine("\n=== RUNNING DAL TESTS ===");
            var tests = new DALTests();
            tests.RunAllTests();
            Console.WriteLine("\n=== TESTS COMPLETED ===");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        #region Product Menu Methods
        static void ProductMenu()
        {
            while (true)
            {
                Console.WriteLine("\n=== PRODUCTS MANAGEMENT ===");
                Console.WriteLine("1. List all products");
                Console.WriteLine("2. Get product by ID");
                Console.WriteLine("3. Create new product");
                Console.WriteLine("4. Update product");
                Console.WriteLine("5. Delete product");
                Console.WriteLine("0. Back to main menu");
                Console.Write("Choose an option: ");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ListAllProducts();
                        break;
                    case "2":
                        GetProductById();
                        break;
                    case "3":
                        CreateProduct();
                        break;
                    case "4":
                        UpdateProduct();
                        break;
                    case "5":
                        DeleteProduct();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        static void ListAllProducts()
        {
            Console.WriteLine("\n--- All Products ---");
            var products = productRepository.GetAll();

            if (products.Count == 0)
            {
                Console.WriteLine("No products found.");
                return;
            }

            foreach (var product in products)
            {
                Console.WriteLine($"ID: {product.Id}, Name: {product.Name}, Price: {product.Price:C}, Stock: {product.QuantityInStock}");
            }
        }

        static void GetProductById()
        {
            Console.Write("Enter product ID: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var product = productRepository.GetById(id);
                if (product != null)
                {
                    Console.WriteLine($"\nProduct Details:");
                    Console.WriteLine($"ID: {product.Id}");
                    Console.WriteLine($"Name: {product.Name}");
                    Console.WriteLine($"Description: {product.Description}");
                    Console.WriteLine($"Price: {product.Price:C}");
                    Console.WriteLine($"Stock: {product.QuantityInStock}");
                    Console.WriteLine($"Created: {product.CreatedDate}");
                }
                else
                {
                    Console.WriteLine("Product not found.");
                }
            }
            else
            {
                Console.WriteLine("Invalid ID format.");
            }
        }

        static void CreateProduct()
        {
            Console.WriteLine("\n--- Create New Product ---");

            var product = new Product();

            Console.Write("Enter product name: ");
            product.Name = Console.ReadLine();

            Console.Write("Enter description: ");
            product.Description = Console.ReadLine();

            Console.Write("Enter price: ");
            if (decimal.TryParse(Console.ReadLine(), out decimal price))
                product.Price = price;
            else
            {
                Console.WriteLine("Invalid price format.");
                return;
            }

            Console.Write("Enter quantity in stock: ");
            if (int.TryParse(Console.ReadLine(), out int quantity))
                product.QuantityInStock = quantity;
            else
            {
                Console.WriteLine("Invalid quantity format.");
                return;
            }



            try
            {
                var newId = productRepository.Create(product);
                Console.WriteLine($"Product created successfully with ID: {newId}");

                var createdProduct = productRepository.GetById(newId);
                Console.WriteLine($"Created date: {createdProduct.CreatedDate}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating product: {ex.Message}");
            }
        }

        static void UpdateProduct()
        {
            Console.Write("Enter product ID to update: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid ID format.");
                return;
            }

            var existingProduct = productRepository.GetById(id);
            if (existingProduct == null)
            {
                Console.WriteLine("Product not found.");
                return;
            }

            Console.WriteLine($"Current name: {existingProduct.Name}");
            Console.Write("Enter new name (press enter to keep current): ");
            var nameInput = Console.ReadLine();
            if (!string.IsNullOrEmpty(nameInput))
                existingProduct.Name = nameInput;

            Console.WriteLine($"Current description: {existingProduct.Description}");
            Console.Write("Enter new description (press enter to keep current): ");
            var descInput = Console.ReadLine();
            if (!string.IsNullOrEmpty(descInput))
                existingProduct.Description = descInput;

            Console.WriteLine($"Current price: {existingProduct.Price:C}");
            Console.Write("Enter new price (press enter to keep current): ");
            var priceInput = Console.ReadLine();
            if (!string.IsNullOrEmpty(priceInput) && decimal.TryParse(priceInput, out decimal newPrice))
                existingProduct.Price = newPrice;

            Console.WriteLine($"Current stock: {existingProduct.QuantityInStock}");
            Console.Write("Enter new quantity (press enter to keep current): ");
            var qtyInput = Console.ReadLine();
            if (!string.IsNullOrEmpty(qtyInput) && int.TryParse(qtyInput, out int newQty))
                existingProduct.QuantityInStock = newQty;

            try
            {
                if (productRepository.Update(existingProduct))
                    Console.WriteLine("Product updated successfully.");
                else
                    Console.WriteLine("Failed to update product.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating product: {ex.Message}");
            }
        }

        static void DeleteProduct()
        {
            Console.Write("Enter product ID to delete: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                try
                {
                    if (productRepository.Delete(id))
                        Console.WriteLine("Product deleted successfully.");
                    else
                        Console.WriteLine("Product not found or could not be deleted.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting product: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Invalid ID format.");
            }
        }
        #endregion

        #region Supplier Menu Methods
        static void SupplierMenu()
        {
            while (true)
            {
                Console.WriteLine("\n=== SUPPLIERS MANAGEMENT ===");
                Console.WriteLine("1. List all suppliers");
                Console.WriteLine("2. List active suppliers");
                Console.WriteLine("3. Get supplier by ID");
                Console.WriteLine("4. Create new supplier");
                Console.WriteLine("5. Update supplier");
                Console.WriteLine("6. Delete supplier");
                Console.WriteLine("0. Back to main menu");
                Console.Write("Choose an option: ");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ListAllSuppliers();
                        break;
                    case "2":
                        ListActiveSuppliers();
                        break;
                    case "3":
                        GetSupplierById();
                        break;
                    case "4":
                        CreateSupplier();
                        break;
                    case "5":
                        UpdateSupplier();
                        break;
                    case "6":
                        DeleteSupplier();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        static void ListAllSuppliers()
        {
            Console.WriteLine("\n--- All Suppliers ---");
            var suppliers = supplierRepository.GetAll();

            foreach (var supplier in suppliers)
            {
                Console.WriteLine($"ID: {supplier.Id}, Name: {supplier.Name}, Active: {supplier.IsActive}");
            }
        }

        static void ListActiveSuppliers()
        {
            Console.WriteLine("\n--- Active Suppliers ---");
            var suppliers = supplierRepository.GetActiveSuppliers();

            foreach (var supplier in suppliers)
            {
                Console.WriteLine($"ID: {supplier.Id}, Name: {supplier.Name}, Contact: {supplier.ContactInfo}");
            }
        }

        static void GetSupplierById()
        {
            Console.Write("Enter supplier ID: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var supplier = supplierRepository.GetById(id);
                if (supplier != null)
                {
                    Console.WriteLine($"\nSupplier Details:");
                    Console.WriteLine($"ID: {supplier.Id}");
                    Console.WriteLine($"Name: {supplier.Name}");
                    Console.WriteLine($"Contact: {supplier.ContactInfo}");
                    Console.WriteLine($"Active: {supplier.IsActive}");
                }
                else
                {
                    Console.WriteLine("Supplier not found.");
                }
            }
            else
            {
                Console.WriteLine("Invalid ID format.");
            }
        }

        static void CreateSupplier()
        {
            Console.WriteLine("\n--- Create New Supplier ---");

            var supplier = new Supplier();

            Console.Write("Enter supplier name: ");
            supplier.Name = Console.ReadLine();

            Console.Write("Enter contact info: ");
            supplier.ContactInfo = Console.ReadLine();

            Console.Write("Is active (true/false): ");
            if (bool.TryParse(Console.ReadLine(), out bool isActive))
                supplier.IsActive = isActive;
            else
                supplier.IsActive = true;

            try
            {
                var newId = supplierRepository.Create(supplier);
                Console.WriteLine($"Supplier created successfully with ID: {newId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating supplier: {ex.Message}");
            }
        }

        static void UpdateSupplier()
        {
            Console.Write("Enter supplier ID to update: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid ID format.");
                return;
            }

            var existingSupplier = supplierRepository.GetById(id);
            if (existingSupplier == null)
            {
                Console.WriteLine("Supplier not found.");
                return;
            }

            Console.WriteLine($"Current name: {existingSupplier.Name}");
            Console.Write("Enter new name (press enter to keep current): ");
            var nameInput = Console.ReadLine();
            if (!string.IsNullOrEmpty(nameInput))
                existingSupplier.Name = nameInput;

            Console.WriteLine($"Current contact info: {existingSupplier.ContactInfo}");
            Console.Write("Enter new contact info (press enter to keep current): ");
            var contactInput = Console.ReadLine();
            if (!string.IsNullOrEmpty(contactInput))
                existingSupplier.ContactInfo = contactInput;

            Console.WriteLine($"Current active status: {existingSupplier.IsActive}");
            Console.Write("Enter new active status (true/false, press enter to keep current): ");
            var activeInput = Console.ReadLine();
            if (!string.IsNullOrEmpty(activeInput) && bool.TryParse(activeInput, out bool newActive))
                existingSupplier.IsActive = newActive;

            try
            {
                if (supplierRepository.Update(existingSupplier))
                    Console.WriteLine("Supplier updated successfully.");
                else
                    Console.WriteLine("Failed to update supplier.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating supplier: {ex.Message}");
            }
        }

        static void DeleteSupplier()
        {
            Console.Write("Enter supplier ID to delete: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                try
                {
                    if (supplierRepository.Delete(id))
                        Console.WriteLine("Supplier deleted successfully.");
                    else
                        Console.WriteLine("Supplier not found or could not be deleted.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting supplier: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Invalid ID format.");
            }
        }
        #endregion

        #region Order Menu Methods
        static void OrderMenu()
        {
            while (true)
            {
                Console.WriteLine("\n=== ORDERS MANAGEMENT ===");
                Console.WriteLine("1. List all orders");
                Console.WriteLine("2. Get order by ID");
                Console.WriteLine("3. Get orders by supplier ID");
                Console.WriteLine("4. Create new order");
                Console.WriteLine("5. Update order");
                Console.WriteLine("6. Delete order");
                Console.WriteLine("7. Manage order items");
                Console.WriteLine("0. Back to main menu");
                Console.Write("Choose an option: ");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ListAllOrders();
                        break;
                    case "2":
                        GetOrderById();
                        break;
                    case "3":
                        GetOrdersBySupplier();
                        break;
                    case "4":
                        CreateOrder();
                        break;
                    case "5":
                        UpdateOrder();
                        break;
                    case "6":
                        DeleteOrder();
                        break;
                    case "7":
                        ManageOrderItems();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        static void ListAllOrders()
        {
            Console.WriteLine("\n--- All Orders ---");
            var orders = orderRepository.GetAll();

            foreach (var order in orders)
            {
                Console.WriteLine($"ID: {order.Id}, Supplier: {order.Supplier.Name}, Date: {order.OrderDate:yyyy-MM-dd}, Status: {order.Status}, Total: {order.TotalAmount:C}");
                if (order.OrderItems.Count > 0)
                {
                    Console.WriteLine("  Items:");
                    foreach (var item in order.OrderItems)
                    {
                        Console.WriteLine($"    - {item.Product.Name}: {item.Quantity} x {item.UnitPrice:C} = {item.Quantity * item.UnitPrice:C}");
                    }
                }
            }
        }

        static void GetOrderById()
        {
            Console.Write("Enter order ID: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var order = orderRepository.GetById(id);
                if (order != null)
                {
                    Console.WriteLine($"\nOrder Details:");
                    Console.WriteLine($"ID: {order.Id}");
                    Console.WriteLine($"Supplier: {order.Supplier.Name}");
                    Console.WriteLine($"Date: {order.OrderDate}");
                    Console.WriteLine($"Status: {order.Status}");
                    Console.WriteLine($"Total Amount: {order.TotalAmount:C}");

                    Console.WriteLine("Order Items:");
                    if (order.OrderItems.Count > 0)
                    {
                        foreach (var item in order.OrderItems)
                        {
                            Console.WriteLine($"  - {item.Product.Name}: {item.Quantity} x {item.UnitPrice:C} = {item.Quantity * item.UnitPrice:C}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("  No items in this order.");
                    }
                }
                else
                {
                    Console.WriteLine("Order not found.");
                }
            }
            else
            {
                Console.WriteLine("Invalid ID format.");
            }
        }

        static void GetOrdersBySupplier()
        {
            Console.Write("Enter supplier ID: ");
            if (int.TryParse(Console.ReadLine(), out int supplierId))
            {
                var orders = orderRepository.GetBySupplierId(supplierId);
                Console.WriteLine($"\n--- Orders for Supplier ID {supplierId} ---");

                foreach (var order in orders)
                {
                    Console.WriteLine($"ID: {order.Id}, Date: {order.OrderDate:yyyy-MM-dd}, Status: {order.Status}, Total: {order.TotalAmount:C}");
                }
            }
            else
            {
                Console.WriteLine("Invalid ID format.");
            }
        }

        static void CreateOrder()
        {
            Console.WriteLine("\n--- Create New Order ---");

            var order = new Order();

            Console.Write("Enter supplier ID: ");
            if (!int.TryParse(Console.ReadLine(), out int supplierId))
            {
                Console.WriteLine("Invalid supplier ID.");
                return;
            }
            order.SupplierId = supplierId;

            Console.Write("Enter status (default: Pending): ");
            var status = Console.ReadLine();
            order.Status = string.IsNullOrEmpty(status) ? "Pending" : status;

            Console.Write("Enter total amount: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal totalAmount))
            {
                Console.WriteLine("Invalid amount format.");
                return;
            }
            order.TotalAmount = totalAmount;

            // Не встановлюємо дату - дозволимо SQL Server зробити це
            // order.OrderDate = DateTime.Now;

            try
            {
                var newId = orderRepository.Create(order);
                Console.WriteLine($"Order created successfully with ID: {newId}");

                // Отримуємо створене замовлення, щоб побачити реальну дату
                var createdOrder = orderRepository.GetById(newId);
                Console.WriteLine($"Order date: {createdOrder.OrderDate}");

                // Пропонуємо додати товари до замовлення
                Console.Write("Do you want to add items to this order? (y/n): ");
                var addItems = Console.ReadLine();
                if (addItems?.ToLower() == "y")
                {
                    OrderItemMenu(newId);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating order: {ex.Message}");
            }
        }

        static void UpdateOrder()
        {
            Console.Write("Enter order ID to update: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid ID format.");
                return;
            }

            var existingOrder = orderRepository.GetById(id);
            if (existingOrder == null)
            {
                Console.WriteLine("Order not found.");
                return;
            }

            Console.WriteLine($"Current supplier ID: {existingOrder.SupplierId}");
            Console.Write("Enter new supplier ID (press enter to keep current): ");
            var supplierInput = Console.ReadLine();
            if (!string.IsNullOrEmpty(supplierInput) && int.TryParse(supplierInput, out int newSupplierId))
                existingOrder.SupplierId = newSupplierId;

            Console.WriteLine($"Current status: {existingOrder.Status}");
            Console.Write("Enter new status (press enter to keep current): ");
            var statusInput = Console.ReadLine();
            if (!string.IsNullOrEmpty(statusInput))
                existingOrder.Status = statusInput;

            Console.WriteLine($"Current total amount: {existingOrder.TotalAmount:C}");
            Console.Write("Enter new total amount (press enter to keep current): ");
            var amountInput = Console.ReadLine();
            if (!string.IsNullOrEmpty(amountInput) && decimal.TryParse(amountInput, out decimal newAmount))
                existingOrder.TotalAmount = newAmount;

            try
            {
                if (orderRepository.Update(existingOrder))
                    Console.WriteLine("Order updated successfully.");
                else
                    Console.WriteLine("Failed to update order.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating order: {ex.Message}");
            }
        }

        static void DeleteOrder()
        {
            Console.Write("Enter order ID to delete: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                try
                {
                    if (orderRepository.Delete(id))
                        Console.WriteLine("Order deleted successfully.");
                    else
                        Console.WriteLine("Order not found or could not be deleted.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting order: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Invalid ID format.");
            }
        }

        static void ManageOrderItems()
        {
            Console.Write("Enter order ID to manage items: ");
            if (int.TryParse(Console.ReadLine(), out int orderId))
            {
                var order = orderRepository.GetById(orderId);
                if (order != null)
                {
                    OrderItemMenu(orderId);
                }
                else
                {
                    Console.WriteLine("Order not found.");
                }
            }
            else
            {
                Console.WriteLine("Invalid order ID.");
            }
        }
        #endregion

        #region Order Item Methods
        static void OrderItemMenu(int orderId)
        {
            while (true)
            {
                Console.WriteLine($"\n=== ORDER ITEMS FOR ORDER {orderId} ===");
                Console.WriteLine("1. List all items in this order");
                Console.WriteLine("2. Add item to order");
                Console.WriteLine("3. Update order item");
                Console.WriteLine("4. Remove item from order");
                Console.WriteLine("5. Calculate order total");
                Console.WriteLine("0. Back to order menu");
                Console.Write("Choose an option: ");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ListOrderItems(orderId);
                        break;
                    case "2":
                        AddOrderItem(orderId);
                        break;
                    case "3":
                        UpdateOrderItem(orderId);
                        break;
                    case "4":
                        DeleteOrderItem();
                        break;
                    case "5":
                        CalculateOrderTotal(orderId);
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        static void ListOrderItems(int orderId)
        {
            Console.WriteLine($"\n--- Items in Order {orderId} ---");
            var orderItems = orderItemRepository.GetByOrderId(orderId);

            if (orderItems.Count == 0)
            {
                Console.WriteLine("No items found in this order.");
                return;
            }

            decimal total = 0;
            foreach (var item in orderItems)
            {
                var itemTotal = item.Quantity * item.UnitPrice;
                total += itemTotal;
                Console.WriteLine($"ID: {item.Id}, Product: {item.Product.Name}, Quantity: {item.Quantity}, Price: {item.UnitPrice:C}, Total: {itemTotal:C}");
            }
            Console.WriteLine($"Grand Total: {total:C}");
        }

        static void AddOrderItem(int orderId)
        {
            Console.WriteLine("\n--- Add Item to Order ---");

            // Показуємо доступні товари
            Console.WriteLine("Available products:");
            var products = productRepository.GetAll();
            foreach (var product in products)
            {
                Console.WriteLine($"ID: {product.Id}, Name: {product.Name}, Price: {product.Price:C}, Stock: {product.QuantityInStock}");
            }

            var orderItem = new OrderItem { OrderId = orderId };

            Console.Write("Enter product ID: ");
            if (!int.TryParse(Console.ReadLine(), out int productId))
            {
                Console.WriteLine("Invalid product ID.");
                return;
            }
            orderItem.ProductId = productId;

            Console.Write("Enter quantity: ");
            if (!int.TryParse(Console.ReadLine(), out int quantity))
            {
                Console.WriteLine("Invalid quantity.");
                return;
            }
            orderItem.Quantity = quantity;

            Console.Write("Enter unit price: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal unitPrice))
            {
                Console.WriteLine("Invalid unit price.");
                return;
            }
            orderItem.UnitPrice = unitPrice;

            try
            {
                var newId = orderItemRepository.Create(orderItem);
                Console.WriteLine($"Order item created successfully with ID: {newId}");

                // Оновлюємо загальну суму замовлення
                var newTotal = orderRepository.CalculateOrderTotal(orderId);
                var order = orderRepository.GetById(orderId);
                order.TotalAmount = newTotal;
                orderRepository.Update(order);
                Console.WriteLine($"Order total updated to: {newTotal:C}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating order item: {ex.Message}");
            }
        }

        static void UpdateOrderItem(int orderId)
        {
            Console.Write("Enter order item ID to update: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid ID format.");
                return;
            }

            var existingItem = orderItemRepository.GetById(id);
            if (existingItem == null || existingItem.OrderId != orderId)
            {
                Console.WriteLine("Order item not found in this order.");
                return;
            }

            Console.WriteLine($"Current product ID: {existingItem.ProductId}");
            Console.Write("Enter new product ID (press enter to keep current): ");
            var productInput = Console.ReadLine();
            if (!string.IsNullOrEmpty(productInput) && int.TryParse(productInput, out int newProductId))
                existingItem.ProductId = newProductId;

            Console.WriteLine($"Current quantity: {existingItem.Quantity}");
            Console.Write("Enter new quantity (press enter to keep current): ");
            var quantityInput = Console.ReadLine();
            if (!string.IsNullOrEmpty(quantityInput) && int.TryParse(quantityInput, out int newQuantity))
                existingItem.Quantity = newQuantity;

            Console.WriteLine($"Current unit price: {existingItem.UnitPrice:C}");
            Console.Write("Enter new unit price (press enter to keep current): ");
            var priceInput = Console.ReadLine();
            if (!string.IsNullOrEmpty(priceInput) && decimal.TryParse(priceInput, out decimal newPrice))
                existingItem.UnitPrice = newPrice;

            try
            {
                if (orderItemRepository.Update(existingItem))
                {
                    Console.WriteLine("Order item updated successfully.");

                    // Оновлюємо загальну суму замовлення
                    var newTotal = orderRepository.CalculateOrderTotal(orderId);
                    var order = orderRepository.GetById(orderId);
                    order.TotalAmount = newTotal;
                    orderRepository.Update(order);
                    Console.WriteLine($"Order total updated to: {newTotal:C}");
                }
                else
                {
                    Console.WriteLine("Failed to update order item.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating order item: {ex.Message}");
            }
        }

        static void DeleteOrderItem()
        {
            Console.Write("Enter order item ID to delete: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                try
                {
                    var item = orderItemRepository.GetById(id);
                    if (item != null)
                    {
                        if (orderItemRepository.Delete(id))
                        {
                            Console.WriteLine("Order item deleted successfully.");

                            // Оновлюємо загальну суму замовлення
                            var newTotal = orderRepository.CalculateOrderTotal(item.OrderId);
                            var order = orderRepository.GetById(item.OrderId);
                            order.TotalAmount = newTotal;
                            orderRepository.Update(order);
                            Console.WriteLine($"Order total updated to: {newTotal:C}");
                        }
                        else
                        {
                            Console.WriteLine("Order item not found or could not be deleted.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Order item not found.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting order item: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Invalid ID format.");
            }
        }

        static void CalculateOrderTotal(int orderId)
        {
            var total = orderRepository.CalculateOrderTotal(orderId);
            Console.WriteLine($"Calculated order total: {total:C}");
        }
        #endregion
    }

    public class DALTests
    {
        private readonly string connectionString = "Server=.;Database=WarehouseDB;Trusted_Connection=true;";

        public void RunAllTests()
        {
            TestProducts();
            TestSuppliers();
            TestOrders();
            TestOrderItems();
        }

        public void TestProducts()
        {
            Console.WriteLine("=== TESTING PRODUCTS ===");
            var repository = new ProductRepository(connectionString);

            // Create
            var newProduct = new Product
            {
                Name = "Test Product",
                Description = "Test Description",
                Price = 99.99m,
                QuantityInStock = 10
            };

            var productId = repository.Create(newProduct);
            Console.WriteLine($"Created product with ID: {productId}");

            // Read
            var product = repository.GetById(productId);
            Console.WriteLine($"Retrieved product: {product.Name}");

            // Update
            product.Price = 129.99m;
            var updated = repository.Update(product);
            Console.WriteLine($"Product updated: {updated}");

            // Delete
            var deleted = repository.Delete(productId);
            Console.WriteLine($"Product deleted: {deleted}");
        }

        public void TestSuppliers()
        {
            Console.WriteLine("\n=== TESTING SUPPLIERS ===");
            var repository = new SupplierRepository(connectionString);

            // Create
            var newSupplier = new Supplier
            {
                Name = "Test Supplier",
                ContactInfo = "test@supplier.com",
                IsActive = true
            };

            var supplierId = repository.Create(newSupplier);
            Console.WriteLine($"Created supplier with ID: {supplierId}");

            // Read
            var supplier = repository.GetById(supplierId);
            Console.WriteLine($"Retrieved supplier: {supplier.Name}");

            // Get active suppliers
            var activeSuppliers = repository.GetActiveSuppliers();
            Console.WriteLine($"Active suppliers count: {activeSuppliers.Count}");

            // Cleanup
            repository.Delete(supplierId);
        }

        public void TestOrders()
        {
            Console.WriteLine("\n=== TESTING ORDERS ===");

            // First create a supplier
            var supplierRepo = new SupplierRepository(connectionString);
            var supplier = new Supplier
            {
                Name = "Order Test Supplier",
                ContactInfo = "order@test.com",
                IsActive = true
            };
            var supplierId = supplierRepo.Create(supplier);

            var orderRepo = new OrderRepository(connectionString);

            // Create
            var newOrder = new Order
            {
                SupplierId = supplierId,
                OrderDate = DateTime.Now,
                Status = "Pending",
                TotalAmount = 1500.00m
            };

            var orderId = orderRepo.Create(newOrder);
            Console.WriteLine($"Created order with ID: {orderId}");

            // Read with join
            var order = orderRepo.GetById(orderId);
            Console.WriteLine($"Retrieved order from supplier: {order.Supplier.Name}");

            // Get by supplier
            var supplierOrders = orderRepo.GetBySupplierId(supplierId);
            Console.WriteLine($"Orders for supplier: {supplierOrders.Count}");

            // Cleanup
            orderRepo.Delete(orderId);
            supplierRepo.Delete(supplierId);
        }

        public void TestOrderItems()
        {
            Console.WriteLine("\n=== TESTING ORDER ITEMS ===");

            // Create test data
            var productRepo = new ProductRepository(connectionString);
            var supplierRepo = new SupplierRepository(connectionString);
            var orderRepo = new OrderRepository(connectionString);
            var orderItemRepo = new OrderItemRepository(connectionString);

            var product = new Product
            {
                Name = "Test Product for Order",
                Price = 50.00m,
                QuantityInStock = 100
            };
            var productId = productRepo.Create(product);

            var supplier = new Supplier
            {
                Name = "Test Supplier for Order Items",
                IsActive = true
            };
            var supplierId = supplierRepo.Create(supplier);

            var order = new Order
            {
                SupplierId = supplierId,
                Status = "Pending",
                TotalAmount = 0
            };
            var orderId = orderRepo.Create(order);

            // Create order item
            var orderItem = new OrderItem
            {
                OrderId = orderId,
                ProductId = productId,
                Quantity = 3,
                UnitPrice = 45.00m
            };

            var orderItemId = orderItemRepo.Create(orderItem);
            Console.WriteLine($"Created order item with ID: {orderItemId}");

            // Read order item
            var retrievedItem = orderItemRepo.GetById(orderItemId);
            Console.WriteLine($"Retrieved order item: {retrievedItem.Quantity} x {retrievedItem.UnitPrice:C}");

            // Get by order
            var orderItems = orderItemRepo.GetByOrderId(orderId);
            Console.WriteLine($"Order items in order: {orderItems.Count}");

            // Cleanup
            orderRepo.Delete(orderId);
            supplierRepo.Delete(supplierId);
            productRepo.Delete(productId);
        }
    }
}
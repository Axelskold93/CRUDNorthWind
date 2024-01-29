using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace Inlämningsuppgift_CRUD_Axel_Sköld
{
    public class ManageDatabase
    {
        public string ConnectionString { get; set; }


        public ManageDatabase()
        {
            ConnectionString = "SERVER = AxelsDator\\SQLEXPRESS01; " +
                                  "Database = Northwind2023_Axel_Sköld;" +
                                  "Integrated Security = true;" +
                                  "TrustServerCertificate = true";
        }
        public Order CreateNewOrder()
        {
            List<Products> availableProducts = new List<Products>();

            string sqlQuery = "SELECT ProductID, ProductName, UnitPrice, UnitsInStock FROM Products";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sqlQuery, connection))
                {
                    connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Products product = new Products((int)reader["ProductID"], (string)reader["ProductName"], (decimal?)reader["UnitPrice"], (short?)reader["UnitsInStock"]);
                        availableProducts.Add(product);
                    }
                }
            }
            foreach (Products product in availableProducts)
            {
                Console.WriteLine($"ID: {product.ProductID}. Name: {product.ProductName}. Price: {product.UnitPrice:F1}. Stock: {product.UnitsInStock}");
            }
            List<Products> selectedProducts = new List<Products>();
            while (true)
            {
                Console.WriteLine("Enter product ID to add to order (enter 0 to finish):");
                int selectedProductID = int.Parse(Console.ReadLine());

                if (selectedProductID == 0)
                {
                    break;
                }
                else if (selectedProducts.Any(product => product.ProductID == selectedProductID))
                {
                    Console.WriteLine("The product is already in the order.");
                }
                else
                {
                    Products selectedProduct = availableProducts.FirstOrDefault(x => x.ProductID == selectedProductID);

                    if (selectedProduct != null)
                    {
                        selectedProducts.Add(selectedProduct);
                        Console.WriteLine($"Product {selectedProduct.ProductName} added to order.");
                    }
                    else
                    {
                        Console.WriteLine("Error: Product does not exist.");
                    }
                }
            }
                DateTime orderDate = DateTime.Now;
                Console.WriteLine("Enter required date in format yyyy-mm-dd:");
                string enteredRequiredDate = Console.ReadLine();
                DateTime requiredDate = DateTime.Parse(enteredRequiredDate);
                Console.WriteLine("Enter shipped date:");
                string enteredShippedDate = Console.ReadLine();
                DateTime? shippedDate = DateTime.Parse(enteredShippedDate);
                Console.WriteLine("Enter freight cost:");
                decimal freight = decimal.Parse(Console.ReadLine());
                Console.WriteLine("Enter ship name:");
                string shipName = Console.ReadLine();
                Console.WriteLine("Enter ship address:");
                string shipAddress = Console.ReadLine();
                Console.WriteLine("Enter ship city:");
                string shipCity = Console.ReadLine();
                Console.WriteLine("Enter ship region:");
                string shipRegion = Console.ReadLine();
                Console.WriteLine("Enter ship postal code:");
                string shipPostalCode = Console.ReadLine();
                Console.WriteLine("Enter ship country:");
                string shipCountry = Console.ReadLine();
                Order newOrder = new Order(orderDate, requiredDate, shippedDate, freight, shipName, shipAddress, shipCity, shipRegion, shipPostalCode, shipCountry, selectedProducts);
                return newOrder;
            
        }
        public void AddOrder()
        {   Console.Clear();
            int latestOrderID = 0;

            Console.WriteLine("Please enter the customerID associated with the order:");
            string customerID = Console.ReadLine();
            if (IsCustomerIDUnique(customerID))
            {
                Console.WriteLine("Error: CustomerID does not exist.");
                Console.ReadKey();
                return;
            }
            Order newOrder = CreateNewOrder();
            string sqlQuery1 = "INSERT INTO Orders (CustomerID,OrderDate, RequiredDate, ShippedDate," +
                " Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry)" +
                              "VALUES (@CustomerID,@OrderDate, @RequiredDate, @ShippedDate, @Freight, @ShipName," +
                              " @ShipAddress, @ShipCity, @ShipRegion, @ShipPostalCode, @ShipCountry)";
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(sqlQuery1, connection))
            {
                connection.Open();
                cmd.Parameters.AddWithValue("@CustomerID", customerID);
                cmd.Parameters.AddWithValue("@OrderDate", newOrder.OrderDate);
                cmd.Parameters.AddWithValue("@RequiredDate", newOrder.RequiredDate);
                cmd.Parameters.AddWithValue("@ShippedDate", newOrder.ShippedDate);
                cmd.Parameters.AddWithValue("@Freight", newOrder.Freight);
                cmd.Parameters.AddWithValue("@ShipName", newOrder.ShipName);
                cmd.Parameters.AddWithValue("@ShipAddress", newOrder.ShipAddress);
                cmd.Parameters.AddWithValue("@ShipCity", newOrder.ShipCity);
                cmd.Parameters.AddWithValue("@ShipRegion", newOrder.ShipRegion);
                cmd.Parameters.AddWithValue("@ShipPostalCode", newOrder.ShipPostalCode);
                cmd.Parameters.AddWithValue("@ShipCountry", newOrder.ShipCountry);
                cmd.ExecuteNonQuery();
            }
            string sqlQuery2 = "SELECT MAX(OrderID) FROM Orders";
            using (SqlConnection connection = new SqlConnection(ConnectionString))
                using (SqlCommand cmd = new SqlCommand(sqlQuery2, connection))
            {
                connection.Open();
                 latestOrderID = (int)cmd.ExecuteScalar();
            }
            foreach (Products product in newOrder.SelectedProducts)
            {
                string sqlQuery3 = "INSERT INTO [Order Details] (OrderID, ProductID, UnitPrice, Quantity)" +
                                  "VALUES (@OrderID, @ProductID, @UnitPrice, @Quantity)";
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                using (SqlCommand cmd = new SqlCommand(sqlQuery3, connection))
                {
                    connection.Open();
                    cmd.Parameters.AddWithValue("@OrderID", latestOrderID);
                    cmd.Parameters.AddWithValue("@ProductID", product.ProductID);
                    cmd.Parameters.AddWithValue("@UnitPrice", product.UnitPrice);
                    cmd.Parameters.AddWithValue("@Quantity", product.UnitsInStock);
                    cmd.Parameters.AddWithValue("@Discount", 0);
                    cmd.ExecuteNonQuery();
                }
            }
            Console.WriteLine("Order added.");
            Console.ReadKey();
        }
        public Customer CreateNewCustomer()
        {
            string customerID = ""; 
            while (true)
            {
                Console.WriteLine("Enter CustomerID, 5 characters:");
                customerID = Console.ReadLine();
                if (customerID.Length == 5)
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Error: CustomerID must be exactly 5 characters long.");
                    Console.ReadKey();
                }
            }
            

            Console.WriteLine("Checking if CustomerID is unique...");
            while (!IsCustomerIDUnique(customerID))
            {
                Console.WriteLine("Error: The entered customer ID is not unique.");
                customerID = Console.ReadLine();
            }
            Console.WriteLine("Name of company: ");
            string companyName = Console.ReadLine();
            Console.WriteLine("Name of contact:");
            string contactName = Console.ReadLine();
            Console.WriteLine("Title:");
            string contactTitle = Console.ReadLine();
            Console.WriteLine("Address:");
            string address = Console.ReadLine();
            Console.WriteLine("City:");
            string city = Console.ReadLine();
            Console.WriteLine("Region:");
            string region = Console.ReadLine();
            Console.WriteLine("Postal Code:");
            string postalCode = Console.ReadLine();
            Console.WriteLine("Country:");
            string country = Console.ReadLine();
            Console.WriteLine("Phone number:");
            string phoneNumber = Console.ReadLine();
            Console.WriteLine("Fax number:");
            string fax = Console.ReadLine();
            Customer newCustomer = new Customer(
            customerID, companyName, contactName, contactTitle, address, city, region,
            postalCode, country, phoneNumber, fax);
            return newCustomer;
        }
        public Customer AddCustomer()
        {
            Customer newCustomer = CreateNewCustomer();

            string sqlQuery = "INSERT INTO Customers (CustomerID, CompanyName, ContactName, ContactTitle, Address, City, Region, PostalCode, Country, Phone, Fax)" +
                              "VALUES (@CustomerID, @CompanyName, @ContactName, @ContactTitle, @Address, @City, @Region, @PostalCode, @Country, @Phone, @Fax)";
            using (SqlConnection connection = new SqlConnection(ConnectionString))

            using (SqlCommand cmd = new SqlCommand(sqlQuery, connection))
            {
                connection.Open();
                cmd.Parameters.AddWithValue("@CustomerID", newCustomer.CustomerID);
                cmd.Parameters.AddWithValue("@CompanyName", newCustomer.CompanyName);
                cmd.Parameters.AddWithValue("@ContactName", newCustomer.ContactName);
                cmd.Parameters.AddWithValue("@ContactTitle", newCustomer.ContactTitle);
                cmd.Parameters.AddWithValue("@Address", newCustomer.Address);
                cmd.Parameters.AddWithValue("@City", newCustomer.City);
                cmd.Parameters.AddWithValue("@Region", newCustomer.Region);
                cmd.Parameters.AddWithValue("@PostalCode", newCustomer.PostalCode);
                cmd.Parameters.AddWithValue("@Country", newCustomer.Country);
                cmd.Parameters.AddWithValue("@Phone", newCustomer.Phone);
                cmd.Parameters.AddWithValue("@Fax", newCustomer.Fax);
                cmd.ExecuteNonQuery();

            }
            Console.WriteLine("Customer added.");
            Console.ReadKey();
            return newCustomer;
            
        }
        public bool IsCustomerIDUnique(string customerID)
        {
            string sqlQuery = "SELECT COUNT(*) FROM Customers WHERE CustomerID = @customerID";
            using (SqlConnection connection = new SqlConnection(ConnectionString))

            using (SqlCommand cmd = new SqlCommand(sqlQuery, connection))
            {
               
                connection.Open();
                cmd.Parameters.AddWithValue("@CustomerID", customerID);
                int count = (int)cmd.ExecuteScalar();
                return count == 0;
            }
           
        }
        public void DeleteCustomer()
        {
            Console.Clear();
            int option = Program.ShowMenu("Do you want to delete based on CustomerID or Company name?", new[]
            {
                "CustomerID",
                "Company name"
            });
            if (option == 0)
            {
                Console.WriteLine("Enter CustomerID: ");
                string customerID = Console.ReadLine();
                string sqlQuery1 = "DELETE FROM [Order Details] WHERE OrderID IN (SELECT OrderID FROM Orders WHERE CustomerID = @CustomerID)";
                string sqlQuery2 = "DELETE FROM Orders WHERE CustomerID = @CustomerID";
                string sqlQuery3 = "DELETE FROM Customers WHERE CustomerID = @CustomerID";
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(sqlQuery1, connection))
                    {
                        cmd.Parameters.AddWithValue("@CustomerID", customerID);
                        cmd.ExecuteNonQuery();
                    }
                    using (SqlCommand cmd = new SqlCommand(sqlQuery2, connection))
                    {
                        cmd.Parameters.AddWithValue("@CustomerID", customerID);
                        cmd.ExecuteNonQuery();
                    }
                    using (SqlCommand cmd = new SqlCommand(sqlQuery3, connection))
                    {
                        cmd.Parameters.AddWithValue("@CustomerID", customerID);
                        cmd.ExecuteNonQuery();
                    }
                }
                Console.WriteLine("Customer successfully deleted.");
                Console.ReadKey();
            }
            else if (option == 1)
            {
                Console.WriteLine("Enter Company name: ");
                string companyName = Console.ReadLine();
                string sqlQuery1 = "SELECT CustomerID FROM Customers WHERE CompanyName = @CompanyName";
                string customerID = "";
                string sqlQuery2 = "DELETE FROM [Order Details] WHERE OrderID IN (SELECT OrderID FROM Orders WHERE CustomerID = @CustomerID)";
                string sqlQuery3 = "DELETE FROM Orders WHERE CustomerID = @CustomerID";
                string sqlQuery4 = "DELETE FROM Customers WHERE CustomerID = @CustomerID";
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(sqlQuery1, connection))
                    {
                        cmd.Parameters.AddWithValue("@CompanyName", companyName);
                        customerID = (string)cmd.ExecuteScalar();
                    }
                    using (SqlCommand cmd = new SqlCommand(sqlQuery2, connection))
                    {
                        cmd.Parameters.AddWithValue("@CustomerID", customerID);
                        cmd.ExecuteNonQuery();
                    }
                    using (SqlCommand cmd = new SqlCommand(sqlQuery3, connection))
                    {
                        cmd.Parameters.AddWithValue("@CustomerID", customerID);
                        cmd.ExecuteNonQuery();
                    }
                    using (SqlCommand cmd = new SqlCommand(sqlQuery4, connection))
                    {
                        cmd.Parameters.AddWithValue("@CustomerID", customerID);
                        cmd.ExecuteNonQuery();
                    }
                }
                Console.WriteLine("Customer successfully deleted.");
                Console.ReadKey();
            }

        }

        public void UpdateEmployee()
        {
            Console.Clear();
            string customerID = "";
            while (true)
            {
                Console.WriteLine("Please enter customerID:");
                customerID = Console.ReadLine();
                Console.WriteLine("Checking if customer exists...");
                if (IsCustomerIDUnique(customerID))
                {
                    Console.WriteLine("Error: Customer does not exist.");
                    Console.WriteLine("Press any key to try again.");
                    Console.ReadKey();
                    
                }
                else 
                {                
                  break;
                }
               
            }
            Console.WriteLine("Please enter new address:");
            string newAddress = Console.ReadLine();
            Console.WriteLine("Please enter new city:");
            string newCity = Console.ReadLine();
            Console.WriteLine("Please enter new region:");
            string newRegion = Console.ReadLine();
            Console.WriteLine("Please enter new postal code:");
            string newPostalCode = Console.ReadLine();
            Console.WriteLine("Please enter new country:");
            string newCountry = Console.ReadLine();
            string sqlQuery = "UPDATE Customers SET Address = @newAddress, City = @newCity, Region = @newRegion, PostalCode = @newPostalCode," +
                " Country = @newCountry WHERE CustomerID = @CustomerID";
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sqlQuery, connection))
                {
                    connection.Open();
                    cmd.Parameters.AddWithValue("@newAddress", newAddress);
                    cmd.Parameters.AddWithValue("@newCity", newCity);
                    cmd.Parameters.AddWithValue("@newRegion", newRegion);
                    cmd.Parameters.AddWithValue("@newPostalCode", newPostalCode);
                    cmd.Parameters.AddWithValue("@newCountry", newCountry);
                    cmd.Parameters.AddWithValue("@CustomerID", customerID);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void ShowCountrySales()
        {
            decimal totalSales = 0;
            Console.WriteLine("Select which country to show sales from:");
            string country = Console.ReadLine();
            string sqlQuery = "SELECT e.FirstName + ' ' + e.LastName AS Seller, o.ShipCountry, SUM(od.Quantity * od.UnitPrice) AS Sales " +
                      "FROM Employees AS e " +
                      "JOIN Orders o ON e.EmployeeID = o.EmployeeID " +
                      "JOIN [Order Details] od ON o.OrderID = od.OrderID " +
                      "WHERE o.ShipCountry = @Country " +
                      "GROUP BY e.FirstName, e.LastName, o.ShipCountry " +
                      "ORDER BY Seller DESC";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sqlQuery, connection))
                {
                    connection.Open();
                    cmd.Parameters.AddWithValue("@Country", country);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        decimal sales = (decimal)reader["Sales"];
                        Console.WriteLine($"{reader["Seller"]}, {reader["ShipCountry"]}, {reader["Sales"]}");
                        totalSales += sales;
                    }
                    Console.WriteLine($"Total sales: {totalSales}");
                    Console.ReadKey();
                }
            }
        }
        public void AddOrderAndNewCustomer()
        {
            Customer newCustomer = CreateNewCustomer();

            AddCustomer();
            AddOrder();

        }

    }
}

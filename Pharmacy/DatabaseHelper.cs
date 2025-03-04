using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

namespace Pharmacy
{
    public class DatabaseHelper
    {
        private string dbFilePath = "pharmacy_inventory.db";
        private bool verbose = true; 

        
        private void ExecuteSql(SQLiteConnection connection, string sql, Dictionary<string, object> parameters = null)
        {
            using (var command = new SQLiteCommand(sql, connection))
            {
                if (parameters != null)
                {
                    foreach (var param in parameters)
                    {
                        command.Parameters.AddWithValue(param.Key, param.Value);
                    }
                }

                if (verbose)
                {
                    Console.WriteLine($"Executing SQL: {sql}");
                    if (parameters != null)
                    {
                        Console.WriteLine("Parameters:");
                        foreach (var param in parameters)
                        {
                            Console.WriteLine($"  {param.Key}: {param.Value}");
                        }
                    }
                }

                command.ExecuteNonQuery();
            }
        }

        
        private SQLiteDataReader ExecuteReader(SQLiteConnection connection, string sql, Dictionary<string, object> parameters = null)
        {
            using (var command = new SQLiteCommand(sql, connection))
            {
                if (parameters != null)
                {
                    foreach (var param in parameters)
                    {
                        command.Parameters.AddWithValue(param.Key, param.Value);
                    }
                }

                if (verbose)
                {
                    Console.WriteLine($"Executing SQL: {sql}");
                    if (parameters != null)
                    {
                        Console.WriteLine("Parameters:");
                        foreach (var param in parameters)
                        {
                            Console.WriteLine($"  {param.Key}: {param.Value}");
                        }
                    }
                }

                return command.ExecuteReader();
            }
        }

        public void InitializeDatabase()
        {
            bool databaseExists = File.Exists(dbFilePath);
            if (!databaseExists)
            {
                SQLiteConnection.CreateFile(dbFilePath);
                Console.WriteLine("Database created!");
            }

            using (var connection = new SQLiteConnection($"Data Source={dbFilePath};Version=3;"))
            {
                connection.Open();

                string createMedicinesTable = @"
                    CREATE TABLE IF NOT EXISTS Medicines (
                        Id INTEGER PRIMARY KEY,
                        Name TEXT NOT NULL,
                        BatchNumber TEXT NOT NULL,
                        Quantity INTEGER NOT NULL,
                        ExpiryDate TEXT NOT NULL,
                        Supplier TEXT NOT NULL,
                        Manufacturer TEXT NOT NULL
                    );";

                string createBuyersTable = @"
                    CREATE TABLE IF NOT EXISTS Buyers (
                        Id INTEGER PRIMARY KEY,
                        Name TEXT NOT NULL,
                        Contact TEXT NOT NULL
                    );";

                string createSuppliersTable = @"
                    CREATE TABLE IF NOT EXISTS Suppliers (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        Contact TEXT NOT NULL
                    );";

                string createPurchaseHistoryTable = @"
                    CREATE TABLE IF NOT EXISTS PurchaseHistory (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        BuyerId INTEGER,
                        PurchaseDetail TEXT NOT NULL,
                        FOREIGN KEY (BuyerId) REFERENCES Buyers(Id)
                    );";

                ExecuteSql(connection, createMedicinesTable);
                ExecuteSql(connection, createBuyersTable);
                ExecuteSql(connection, createSuppliersTable);
                ExecuteSql(connection, createPurchaseHistoryTable);

                if (!databaseExists)
                {
                    Console.WriteLine("All tables initialized!");
                }
                else
                {
                    Console.WriteLine("Database tables verified!");
                }

                connection.Close();
            }
        }

        public void InsertMedicine(Medicine medicine)
        {
            using (var connection = new SQLiteConnection($"Data Source={dbFilePath};Version=3;"))
            {
                connection.Open();

                string insertMedicine = @"
                    INSERT INTO Medicines (Id, Name, BatchNumber, Quantity, ExpiryDate, Supplier, Manufacturer)
                    VALUES (@Id, @Name, @BatchNumber, @Quantity, @ExpiryDate, @Supplier, @Manufacturer);
                ";

                var parameters = new Dictionary<string, object>
                {
                    {"@Id", medicine.Id},
                    {"@Name", medicine.Name},
                    {"@BatchNumber", medicine.BatchNumber},
                    {"@Quantity", medicine.Quantity},
                    {"@ExpiryDate", medicine.ExpiryDate.ToString("yyyy-MM-dd")},
                    {"@Supplier", medicine.Supplier},
                    {"@Manufacturer", medicine.Manufacturer}
                };

                ExecuteSql(connection, insertMedicine, parameters);
                connection.Close();
            }
        }

        public List<Medicine> GetAllMedicines()
        {
            List<Medicine> medicines = new List<Medicine>();

            using (var connection = new SQLiteConnection($"Data Source={dbFilePath};Version=3;"))
            {
                connection.Open();

                string selectMedicines = "SELECT * FROM Medicines";
                using (var reader = ExecuteReader(connection, selectMedicines))
                {
                    while (reader.Read())
                    {
                        var medicine = new Medicine(
                            reader.GetInt32(0),  // Id
                            reader.GetString(1), // Name
                            reader.GetString(2), // BatchNumber
                            reader.GetInt32(3),  // Quantity
                            DateTime.Parse(reader.GetString(4)), // ExpiryDate
                            reader.GetString(5), // Supplier
                            reader.GetString(6)  // Manufacturer
                        );
                        medicines.Add(medicine);
                    }
                }

                connection.Close();
            }

            return medicines;
        }

        public Medicine GetMedicineById(int id)
        {
            using (var connection = new SQLiteConnection($"Data Source={dbFilePath};Version=3;"))
            {
                connection.Open();

                string selectMedicine = "SELECT * FROM Medicines WHERE Id = @Id";
                var parameters = new Dictionary<string, object> { { "@Id", id } };
                using (var reader = ExecuteReader(connection, selectMedicine, parameters))
                {
                    if (reader.Read())
                    {
                        return new Medicine(
                            reader.GetInt32(0),  // Id
                            reader.GetString(1), // Name
                            reader.GetString(2), // BatchNumber
                            reader.GetInt32(3),  // Quantity
                            DateTime.Parse(reader.GetString(4)), // ExpiryDate
                            reader.GetString(5), // Supplier
                            reader.GetString(6)  // Manufacturer
                        );
                    }
                }

                connection.Close();
            }

            return null;
        }

        public void UpdateMedicine(Medicine medicine)
        {
            using (var connection = new SQLiteConnection($"Data Source={dbFilePath};Version=3;"))
            {
                connection.Open();

                string updateMedicine = @"
            UPDATE Medicines
            SET Name = @Name, BatchNumber = @BatchNumber, Quantity = @Quantity, 
                ExpiryDate = @ExpiryDate, Supplier = @Supplier, Manufacturer = @Manufacturer
            WHERE Id = @Id;
        ";

                var parameters = new Dictionary<string, object>
        {
            {"@Id", medicine.Id},
            {"@Name", medicine.Name},
            {"@BatchNumber", medicine.BatchNumber},
            {"@Quantity", medicine.Quantity},
            {"@ExpiryDate", medicine.ExpiryDate.ToString("yyyy-MM-dd")},
            {"@Supplier", medicine.Supplier},
            {"@Manufacturer", medicine.Manufacturer}
        };

                ExecuteSql(connection, updateMedicine, parameters);
                connection.Close();
            }
        }

        public void InsertBuyer(Buyer newBuyer)
        {
            using (var connection = new SQLiteConnection($"Data Source={dbFilePath};Version=3;"))
            {
                connection.Open();

                string insertBuyer = @"
                    INSERT INTO Buyers (Id, Name, Contact)
                    VALUES (@Id, @Name, @Contact);
                ";

                var parameters = new Dictionary<string, object>
                {
                    {"@Id", newBuyer.BuyerId},
                    {"@Name", newBuyer.Name},
                    {"@Contact", newBuyer.Contact}
                };

                ExecuteSql(connection, insertBuyer, parameters);
                connection.Close();
            }
        }

        public List<Buyer> GetAllBuyers()
        {
            List<Buyer> buyers = new List<Buyer>();

            using (var connection = new SQLiteConnection($"Data Source={dbFilePath};Version=3;"))
            {
                connection.Open();

                string selectBuyers = "SELECT * FROM Buyers";
                using (var reader = ExecuteReader(connection, selectBuyers))
                {
                    while (reader.Read())
                    {
                        var buyer = new Buyer(
                            reader.GetInt32(0),  // Id
                            reader.GetString(1), // Name
                            reader.GetString(2)  // Contact
                        );
                        buyers.Add(buyer);
                    }
                }

                // Load purchase history for each buyer
                foreach (var buyer in buyers)
                {
                    string selectPurchases = "SELECT PurchaseDetail FROM PurchaseHistory WHERE BuyerId = @BuyerId";
                    var purchaseParams = new Dictionary<string, object> { { "@BuyerId", buyer.BuyerId } };
                    using (var purchaseReader = ExecuteReader(connection, selectPurchases, purchaseParams))
                    {
                        while (purchaseReader.Read())
                        {
                            buyer.AddPurchase(purchaseReader.GetString(0));
                        }
                    }
                }

                connection.Close();
            }

            return buyers;
        }

        public void AddSupplier(Supplier newSupplier)
        {
            using (var connection = new SQLiteConnection($"Data Source={dbFilePath};Version=3;"))
            {
                connection.Open();

                string insertSupplier = @"
                    INSERT INTO Suppliers (Name, Contact)
                    VALUES (@Name, @Contact);
                ";

                var parameters = new Dictionary<string, object>
                {
                    {"@Name", newSupplier.Name},
                    {"@Contact", newSupplier.Contact}
                };

                ExecuteSql(connection, insertSupplier, parameters);
                connection.Close();
            }
        }

        public List<Supplier> GetAllSuppliers()
        {
            List<Supplier> suppliers = new List<Supplier>();

            using (var connection = new SQLiteConnection($"Data Source={dbFilePath};Version=3;"))
            {
                connection.Open();

                string selectSuppliers = "SELECT * FROM Suppliers";
                using (var reader = ExecuteReader(connection, selectSuppliers))
                {
                    while (reader.Read())
                    {
                        var supplier = new Supplier(
                            reader.GetInt32(0),  // Id
                            reader.GetString(1), // Name
                            reader.GetString(2)  // Contact
                        );
                        suppliers.Add(supplier);
                    }
                }

                connection.Close();
            }

            return suppliers;
        }

        public Buyer? GetBuyerById(int id)
        {
            using (var connection = new SQLiteConnection($"Data Source={dbFilePath};Version=3;"))
            {
                connection.Open();

                string selectBuyer = "SELECT * FROM Buyers WHERE Id = @Id";
                Buyer buyer = null;
                var parameters = new Dictionary<string, object> { { "@Id", id } };

                using (var reader = ExecuteReader(connection, selectBuyer, parameters))
                {
                    if (reader.Read())
                    {
                        buyer = new Buyer(
                            reader.GetInt32(0),  // Id
                            reader.GetString(1), // Name
                            reader.GetString(2)  // Contact
                        );
                    }
                }

                if (buyer != null)
                {
                    string selectPurchases = "SELECT PurchaseDetail FROM PurchaseHistory WHERE BuyerId = @BuyerId";
                    var purchaseParams = new Dictionary<string, object> { { "@BuyerId", id } };
                    using (var purchaseReader = ExecuteReader(connection, selectPurchases, purchaseParams))
                    {
                        while (purchaseReader.Read())
                        {
                            buyer.AddPurchase(purchaseReader.GetString(0));
                        }
                    }
                }

                connection.Close();
                return buyer;
            }
        }

        public void UpdateBuyer(Buyer buyer)
        {
            using (var connection = new SQLiteConnection($"Data Source={dbFilePath};Version=3;"))
            {
                connection.Open();

                string updateBuyer = @"
                    UPDATE Buyers
                    SET Name = @Name, Contact = @Contact
                    WHERE Id = @Id;
                ";

                var parameters = new Dictionary<string, object>
                {
                    {"@Id", buyer.BuyerId},
                    {"@Name", buyer.Name},
                    {"@Contact", buyer.Contact}
                };

                ExecuteSql(connection, updateBuyer, parameters);
                connection.Close();
            }
        }

        public void InsertPurchase(int buyerId, string purchaseDetail)
        {
            using (var connection = new SQLiteConnection($"Data Source={dbFilePath};Version=3;"))
            {
                connection.Open();

                string insertPurchase = @"
                    INSERT INTO PurchaseHistory (BuyerId, PurchaseDetail)
                    VALUES (@BuyerId, @PurchaseDetail);
                ";

                var parameters = new Dictionary<string, object>
                {
                    {"@BuyerId", buyerId},
                    {"@PurchaseDetail", purchaseDetail}
                };

                ExecuteSql(connection, insertPurchase, parameters);
                connection.Close();
            }
        }
    }
}
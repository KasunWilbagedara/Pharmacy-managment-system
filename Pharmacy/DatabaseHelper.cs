using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

namespace Pharmacy
{
    public class DatabaseHelper
    {
        private string dbFilePath = "pharmacy_inventory.db"; // Database file path

        // Create database and tables if not exist
        public void InitializeDatabase()
        {
            if (!File.Exists(dbFilePath))
            {
                SQLiteConnection.CreateFile(dbFilePath);
                Console.WriteLine("Database created!");

                using (var connection = new SQLiteConnection($"Data Source={dbFilePath};Version=3;"))
                {
                    connection.Open();

                    // Create Medicines Table
                    string createMedicinesTable = @"
                        CREATE TABLE IF NOT EXISTS Medicines (
                            Id INTEGER PRIMARY KEY,
                            Name TEXT NOT NULL,
                            BatchNumber TEXT NOT NULL,
                            Quantity INTEGER NOT NULL,
                            ExpiryDate TEXT NOT NULL,
                            Supplier TEXT NOT NULL,
                            Manufacturer TEXT NOT NULL
                        );
                    ";

                    // Create Buyers Table
                    string createBuyersTable = @"
                        CREATE TABLE IF NOT EXISTS Buyers (
                            Id INTEGER PRIMARY KEY,
                            Name TEXT NOT NULL,
                            Contact TEXT NOT NULL
                        );
                    ";

                    // Create Suppliers Table
                    string createSuppliersTable = @"
                        CREATE TABLE IF NOT EXISTS Suppliers (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            Name TEXT NOT NULL,
                            Contact TEXT NOT NULL
                        );
                    ";

                    using (var command = new SQLiteCommand(createMedicinesTable, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    using (var command = new SQLiteCommand(createBuyersTable, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    using (var command = new SQLiteCommand(createSuppliersTable, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    connection.Close();
                }
            }
        }

        // Add a new medicine to the database
        public void InsertMedicine(Medicine medicine)
        {
            using (var connection = new SQLiteConnection($"Data Source={dbFilePath};Version=3;"))
            {
                connection.Open();

                string insertMedicine = @"
                    INSERT INTO Medicines (Id, Name, BatchNumber, Quantity, ExpiryDate, Supplier, Manufacturer)
                    VALUES (@Id, @Name, @BatchNumber, @Quantity, @ExpiryDate, @Supplier, @Manufacturer);
                ";

                using (var command = new SQLiteCommand(insertMedicine, connection))
                {
                    command.Parameters.AddWithValue("@Id", medicine.Id);
                    command.Parameters.AddWithValue("@Name", medicine.Name);
                    command.Parameters.AddWithValue("@BatchNumber", medicine.BatchNumber);
                    command.Parameters.AddWithValue("@Quantity", medicine.Quantity);
                    command.Parameters.AddWithValue("@ExpiryDate", medicine.ExpiryDate.ToString("yyyy-MM-dd"));
                    command.Parameters.AddWithValue("@Supplier", medicine.Supplier);
                    command.Parameters.AddWithValue("@Manufacturer", medicine.Manufacturer);
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }

        // Get all medicines from the database
        public List<Medicine> GetAllMedicines()
        {
            List<Medicine> medicines = new List<Medicine>();

            using (var connection = new SQLiteConnection($"Data Source={dbFilePath};Version=3;"))
            {
                connection.Open();

                string selectMedicines = "SELECT * FROM Medicines";
                using (var command = new SQLiteCommand(selectMedicines, connection))
                {
                    using (var reader = command.ExecuteReader())
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
                }

                connection.Close();
            }

            return medicines;
        }
        // Get medicine by ID
        public Medicine GetMedicineById(int id)
        {
            using (var connection = new SQLiteConnection($"Data Source={dbFilePath};Version=3;"))
            {
                connection.Open();

                string selectMedicine = "SELECT * FROM Medicines WHERE Id = @Id";
                using (var command = new SQLiteCommand(selectMedicine, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (var reader = command.ExecuteReader())
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
                }

                connection.Close();
            }

            return null;
        }

        // Update medicine in the database
        public void UpdateMedicine(Medicine medicine)
        {
            using (var connection = new SQLiteConnection($"Data Source={dbFilePath};Version=3;"))
            {
                connection.Open();

                string updateMedicine = @"
                    UPDATE Medicines
                    SET Name = @Name, BatchNumber = @BatchNumber, Quantity = @Quantity, ExpiryDate = @ExpiryDate, Supplier = @Supplier, Manufacturer = @Manufacturer
                    WHERE Id = @Id;
                ";

                using (var command = new SQLiteCommand(updateMedicine, connection))
                {
                    command.Parameters.AddWithValue("@Id", medicine.Id);
                    command.Parameters.AddWithValue("@Name", medicine.Name);
                    command.Parameters.AddWithValue("@BatchNumber", medicine.BatchNumber);
                    command.Parameters.AddWithValue("@Quantity", medicine.Quantity);
                    command.Parameters.AddWithValue("@ExpiryDate", medicine.ExpiryDate.ToString("yyyy-MM-dd"));
                    command.Parameters.AddWithValue("@Supplier", medicine.Supplier);
                    command.Parameters.AddWithValue("@Manufacturer", medicine.Manufacturer);
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }

        // Add buyer
        public void InsertBuyer(Buyer newBuyer)
        {
            using (var connection = new SQLiteConnection($"Data Source={dbFilePath};Version=3;"))
            {
                connection.Open();

                string insertBuyer = @"
                    INSERT INTO Buyers (Id, Name, Contact)
                    VALUES (@Id, @Name, @Contact);
                ";

                using (var command = new SQLiteCommand(insertBuyer, connection))
                {
                    command.Parameters.AddWithValue("@Id", newBuyer.BuyerId);
                    command.Parameters.AddWithValue("@Name", newBuyer.Name);
                    command.Parameters.AddWithValue("@Contact", newBuyer.Contact);
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }

        // Get all buyers
        public List<Buyer> GetAllBuyers()
        {
            List<Buyer> buyers = new List<Buyer>();

            using (var connection = new SQLiteConnection($"Data Source={dbFilePath};Version=3;"))
            {
                connection.Open();

                string selectBuyers = "SELECT * FROM Buyers";
                using (var command = new SQLiteCommand(selectBuyers, connection))
                {
                    using (var reader = command.ExecuteReader())
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
                }

                connection.Close();
            }

            return buyers;
        }

        // Add Supplier
        public void AddSupplier(Supplier newSupplier)
        {
            using (var connection = new SQLiteConnection($"Data Source={dbFilePath};Version=3;"))
            {
                connection.Open();

                string insertSupplier = @"
                    INSERT INTO Suppliers (Name, Contact)
                    VALUES (@Name, @Contact);
                ";

                using (var command = new SQLiteCommand(insertSupplier, connection))
                {
                    command.Parameters.AddWithValue("@Name", newSupplier.Name);
                    command.Parameters.AddWithValue("@Contact", newSupplier.Contact);
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }

        // Get all suppliers
        public List<Supplier> GetAllSuppliers()
        {
            List<Supplier> suppliers = new List<Supplier>();

            using (var connection = new SQLiteConnection($"Data Source={dbFilePath};Version=3;"))
            {
                connection.Open();

                string selectSuppliers = "SELECT * FROM Suppliers";
                using (var command = new SQLiteCommand(selectSuppliers, connection))
                {
                    using (var reader = command.ExecuteReader())
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
                using (var command = new SQLiteCommand(selectBuyer, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Buyer(
                                reader.GetInt32(0),  // Id
                                reader.GetString(1), // Name
                                reader.GetString(2)  // Contact
                            );
                        }
                    }
                }

                connection.Close();
            }

            return null; // Return null if buyer is not found
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

                using (var command = new SQLiteCommand(updateBuyer, connection))
                {
                    command.Parameters.AddWithValue("@Id", buyer.BuyerId);
                    command.Parameters.AddWithValue("@Name", buyer.Name);
                    command.Parameters.AddWithValue("@Contact", buyer.Contact);
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }
    }
}
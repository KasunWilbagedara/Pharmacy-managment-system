﻿using System;

namespace Pharmacy
{
    public class Program
    {
        static void Main(string[] args)
        {
            // Create an instance of the DatabaseHelper class
            DatabaseHelper dbHelper = new DatabaseHelper();

            // Initialize the database (this will create the database and tables if not already present)
            dbHelper.InitializeDatabase();

            // Create the Inventory object and pass dbHelper to it
            Inventory pharmacy = new Inventory(dbHelper);

            // Display the menu
            while (true)
            {
                Console.WriteLine("\nPharmacy Inventory Management System");
                Console.WriteLine("1. Add Medicine");
                Console.WriteLine("2. Modify Medicine Details");
                Console.WriteLine("3. Sell Medicine");
                Console.WriteLine("4. Display All Medicines");
                Console.WriteLine("5. Exit");
                Console.Write("Enter your choice: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddMedicine(pharmacy);
                        break;
                    case "2":
                        ModifyMedicine(pharmacy);
                        break;
                    case "3":
                        SellMedicine(pharmacy);
                        break;
                    case "4":
                        DisplayAllMedicines(pharmacy);
                        break;
                    case "5":
                        Console.WriteLine("Exiting the system...");
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        // Method to add medicine
        static void AddMedicine(Inventory pharmacy)
        {
            Console.WriteLine("\nAdd New Medicine");

            Console.Write("Enter Medicine ID: ");
            int id = int.Parse(Console.ReadLine());

            Console.Write("Enter Medicine Name: ");
            string name = Console.ReadLine();

            Console.Write("Enter Batch Number: ");
            string batchNumber = Console.ReadLine();

            Console.Write("Enter Quantity: ");
            int quantity = int.Parse(Console.ReadLine());

            Console.Write("Enter Expiry Date (yyyy-MM-dd): ");
            DateTime expiryDate = DateTime.Parse(Console.ReadLine());

            Console.Write("Enter Supplier: ");
            string supplier = Console.ReadLine();

            Console.Write("Enter Manufacturer: ");
            string manufacturer = Console.ReadLine();

            pharmacy.AddMedicine(id, name, batchNumber, quantity, expiryDate, supplier, manufacturer);
            Console.WriteLine("Medicine added successfully!");
        }

        // Method to modify medicine details
        static void ModifyMedicine(Inventory pharmacy)
        {
            Console.WriteLine("\nModify Medicine Details");

            Console.Write("Enter Medicine ID to modify: ");
            int id = int.Parse(Console.ReadLine());

            // Fetch the existing medicine details
            var medicine = pharmacy.GetMedicineById(id);
            if (medicine == null)
            {
                Console.WriteLine("Medicine not found.");
                return;
            }

            Console.WriteLine($"Current Details: {medicine.Name}, Batch: {medicine.BatchNumber}, Quantity: {medicine.Quantity}, Expiry: {medicine.ExpiryDate.ToShortDateString()}, Supplier: {medicine.Supplier}, Manufacturer: {medicine.Manufacturer}");

            Console.Write("Enter new Medicine Name (leave blank to keep current): ");
            string name = Console.ReadLine();
            if (!string.IsNullOrEmpty(name))
            {
                medicine.Name = name;
            }

            Console.Write("Enter new Batch Number (leave blank to keep current): ");
            string batchNumber = Console.ReadLine();
            if (!string.IsNullOrEmpty(batchNumber))
            {
                medicine.BatchNumber = batchNumber;
            }

            Console.Write("Enter new Quantity (leave blank to keep current): ");
            string quantityInput = Console.ReadLine();
            if (!string.IsNullOrEmpty(quantityInput))
            {
                medicine.Quantity = int.Parse(quantityInput);
            }

            Console.Write("Enter new Expiry Date (yyyy-MM-dd, leave blank to keep current): ");
            string expiryDateInput = Console.ReadLine();
            if (!string.IsNullOrEmpty(expiryDateInput))
            {
                medicine.ExpiryDate = DateTime.Parse(expiryDateInput);
            }

            Console.Write("Enter new Supplier (leave blank to keep current): ");
            string supplier = Console.ReadLine();
            if (!string.IsNullOrEmpty(supplier))
            {
                medicine.Supplier = supplier;
            }

            Console.Write("Enter new Manufacturer (leave blank to keep current): ");
            string manufacturer = Console.ReadLine();
            if (!string.IsNullOrEmpty(manufacturer))
            {
                medicine.Manufacturer = manufacturer;
            }

            // Update the medicine in the database
            pharmacy.UpdateMedicine(medicine);
            Console.WriteLine("Medicine details updated successfully!");
        }

        // Method to sell medicine
        static void SellMedicine(Inventory pharmacy)
        {
            Console.WriteLine("\nSell Medicine");

            Console.Write("Enter Medicine ID to sell: ");
            int id = int.Parse(Console.ReadLine());

            pharmacy.SellMedicine(id);
        }

        // Method to display all medicines
        static void DisplayAllMedicines(Inventory pharmacy)
        {
            Console.WriteLine("\nList of All Medicines:");
            var medicines = pharmacy.GetAllMedicines();
            foreach (var medicine in medicines)
            {
                Console.WriteLine($"ID: {medicine.Id}, Name: {medicine.Name}, Batch: {medicine.BatchNumber}, Quantity: {medicine.Quantity}, Expiry: {medicine.ExpiryDate.ToShortDateString()}, Supplier: {medicine.Supplier}, Manufacturer: {medicine.Manufacturer}");
            }
        }
    }
    
}
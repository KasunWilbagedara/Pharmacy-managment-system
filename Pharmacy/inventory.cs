using System;
using System.Collections.Generic;
using System.Linq;

namespace Pharmacy
{
    public class Inventory
    {
        private readonly DatabaseHelper _dbHelper;

        public Inventory(DatabaseHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }
        public Medicine GetMedicineById(int id)
        {
            return _dbHelper.GetMedicineById(id);
        }
        public Buyer GetBuyerById(int id)
        {
            return _dbHelper.GetBuyerById(id);
        }
        public void UpdateBuyer(Buyer buyer)
        {
            _dbHelper.UpdateBuyer(buyer);
        }
        public void AddBuyer(Buyer buyer)
        {
            _dbHelper.InsertBuyer(buyer);
        }
        public Medicine? GetMedicineByName(string name)
        {
            return _dbHelper.GetAllMedicines().FirstOrDefault(m => m.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public void UpdateMedicine(Medicine medicine)
        {
            _dbHelper.UpdateMedicine(medicine);
        }
        // Add medicine to the inventory
        public void AddMedicine(int id, string name, string batchNumber, int quantity, DateTime expiryDate, string supplier, string manufacturer)
        {
            var existingMedicine = _dbHelper.GetMedicineById(id);

            if (existingMedicine != null)
            {
                // If the medicine already exists, update the quantity
                existingMedicine.Quantity += quantity;
                _dbHelper.UpdateMedicine(existingMedicine);
                Console.WriteLine($"Updated {name} in inventory. New quantity: {existingMedicine.Quantity}");
            }
            else
            {
                // Add new medicine
                Medicine newMedicine = new Medicine(id, name, batchNumber, quantity, expiryDate, supplier, manufacturer);
                _dbHelper.InsertMedicine(newMedicine);
                Console.WriteLine($"Added new medicine: {name} to inventory.");
            }
        }
        public List<Medicine> GetAllMedicines()
        {
            return _dbHelper.GetAllMedicines();
        }
        // Sell a medicine (decrease its quantity)
        // In Inventory.cs, replace the existing SellMedicine() method with this:
        // In Inventory.cs, update SellMedicine()
        // In Inventory.cs, replace the existing SellMedicine method with this:
        public void SellMedicine()
        {
            Console.WriteLine("\nSell Medicine");

            // Ask for buyer ID
            Console.Write("Enter Buyer ID: ");
            int buyerId = int.Parse(Console.ReadLine() ?? "0");

            // Check if buyer exists
            var buyer = _dbHelper.GetBuyerById(buyerId);
            if (buyer == null)
            {
                Console.Write("Enter Buyer Name: ");
                string name = Console.ReadLine() ?? string.Empty;

                Console.Write("Enter Buyer Contact: ");
                string contact = Console.ReadLine() ?? string.Empty;

                buyer = new Buyer(buyerId, name, contact);
                _dbHelper.InsertBuyer(buyer);
                Console.WriteLine($"New buyer {buyer.Name} added.");
            }

            // Ask for medicine name
            Console.Write("Enter Medicine Name: ");
            string medicineName = Console.ReadLine() ?? string.Empty;

            // Find medicine by name
            var medicine = _dbHelper.GetAllMedicines().FirstOrDefault(m => m.Name.Equals(medicineName, StringComparison.OrdinalIgnoreCase));
            if (medicine == null)
            {
                Console.WriteLine("Medicine not found.");
                return;
            }

            // Display current quantity
            Console.WriteLine($"Current stock of {medicine.Name}: {medicine.Quantity} units");

            // Ask for quantity
            Console.Write("Enter Quantity to Sell: ");
            int quantity = int.Parse(Console.ReadLine() ?? "0");

            // Check if enough stock is available
            if (medicine.Quantity < quantity)
            {
                Console.WriteLine($"Not enough stock. Available: {medicine.Quantity}");
                return;
            }

            // Update medicine quantity
            medicine.Quantity -= quantity;
            _dbHelper.UpdateMedicine(medicine); // Persist the updated quantity to the database
            Console.WriteLine($"Updated stock of {medicine.Name}: {medicine.Quantity} units remaining");

            // Update buyer's purchase history in database
            string purchaseDetail = $"Purchased {quantity} units of {medicineName} on {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}";
            _dbHelper.InsertPurchase(buyer.BuyerId, purchaseDetail);

            // Reload buyer to get updated history
            buyer = _dbHelper.GetBuyerById(buyerId);

            // Display confirmation and updated buyer history
            Console.WriteLine($"Sale successful! {quantity} units of {medicineName} sold to {buyer.Name}.");
            Console.WriteLine("\nUpdated Buyer Purchase History:");
            buyer.ShowPurchaseHistory();
        }

        // Register a buyer
        public void RegisterBuyer(int id, string name, string contact)
        {
            Buyer newBuyer = new Buyer(id, name, contact);
            _dbHelper.InsertBuyer(newBuyer);
            Console.WriteLine($"Registered new buyer: {name}");


        }


        // In Inventory.cs, add this method to the Inventory class
        public void SearchMedicineByNameOrId()
        {
            Console.WriteLine("\nSearch Medicine");
            Console.WriteLine("1. Search by Name");
            Console.WriteLine("2. Search by ID");
            Console.Write("Enter your choice (1 or 2): ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.Write("Enter Medicine Name: ");
                    string name = Console.ReadLine() ?? string.Empty;
                    var medicineByName = _dbHelper.GetAllMedicines()
                        .FirstOrDefault(m => m.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                    if (medicineByName != null)
                    {
                        DisplayMedicineDetails(medicineByName);
                    }
                    else
                    {
                        Console.WriteLine($"No medicine found with name '{name}'.");
                    }
                    break;

                case "2":
                    Console.Write("Enter Medicine ID: ");
                    int id;
                    if (!int.TryParse(Console.ReadLine(), out id))
                    {
                        Console.WriteLine("Invalid ID. Please enter a number.");
                        return;
                    }
                    var medicineById = _dbHelper.GetMedicineById(id);
                    if (medicineById != null)
                    {
                        DisplayMedicineDetails(medicineById);
                    }
                    else
                    {
                        Console.WriteLine($"No medicine found with ID '{id}'.");
                    }
                    break;

                default:
                    Console.WriteLine("Invalid choice. Please select 1 or 2.");
                    break;
            }
        }

        // Helper method to display medicine details
        private void DisplayMedicineDetails(Medicine medicine)
        {
            Console.WriteLine("\nMedicine Details:");
            Console.WriteLine($"ID: {medicine.Id}");
            Console.WriteLine($"Name: {medicine.Name}");
            Console.WriteLine($"Batch Number: {medicine.BatchNumber}");
            Console.WriteLine($"Quantity: {medicine.Quantity} units");
            Console.WriteLine($"Expiry Date: {medicine.ExpiryDate.ToString("yyyy-MM-dd")}");
            Console.WriteLine($"Supplier: {medicine.Supplier}");
            Console.WriteLine($"Manufacturer: {medicine.Manufacturer}");
        }

        // Display list of suppliers
        public void ShowSuppliersList()
        {
            var suppliers = _dbHelper.GetAllSuppliers();

            Console.WriteLine("Suppliers List:");
            foreach (var supplier in suppliers)
            {
                Console.WriteLine($"Supplier: {supplier.Name}, Contact: {supplier.Contact}");
            }
        }

        // Display list of buyers
        public void ShowBuyersList()
        {
            var buyers = _dbHelper.GetAllBuyers();

            Console.WriteLine("Buyers List:");
            foreach (var buyer in buyers)
            {
                Console.WriteLine($"Buyer: {buyer.Name}, Contact: {buyer.Contact}");
            }
        }

        // Display medicines sorted by quantity
        public void DisplayByQuantity()
        {
            var medicines = _dbHelper.GetAllMedicines().OrderBy(m => m.Quantity).ToList();

            Console.WriteLine("Medicines sorted by Quantity:");
            foreach (var medicine in medicines)
            {
                Console.WriteLine($"{medicine.Name} - {medicine.Quantity} units left");
            }
        }

        // Search medicines by name
        public void SearchMedicine(string name)
        {
            var medicines = _dbHelper.GetAllMedicines().Where(m => m.Name.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();

            if (medicines.Any())
            {
                foreach (var medicine in medicines)
                {
                    Console.WriteLine($"Found: {medicine.Name}, Expiry Date: {medicine.ExpiryDate.ToShortDateString()}, Supplier: {medicine.Supplier}");
                }
            }
            else
            {
                Console.WriteLine("No medicines found matching your search.");
            }
        }

        // Display medicines sorted by name
        public void DisplayByName()
        {
            var medicines = _dbHelper.GetAllMedicines().OrderBy(m => m.Name).ToList();

            Console.WriteLine("Medicines sorted by Name:");
            foreach (var medicine in medicines)
            {
                Console.WriteLine($"{medicine.Name} - {medicine.Quantity} units left");
            }
        }

        // Display medicines sorted by expiry date
        public void DisplayByExpiry()
        {
            var medicines = _dbHelper.GetAllMedicines().OrderBy(m => m.ExpiryDate).ToList();

            Console.WriteLine("Medicines sorted by Expiry Date:");
            foreach (var medicine in medicines)
            {
                Console.WriteLine($"{medicine.Name} - Expiry Date: {medicine.ExpiryDate.ToShortDateString()}");
            }
        }

        // Check if there is any low stock
        public void CheckLowStock(int threshold)
        {
            var lowStockMedicines = _dbHelper.GetAllMedicines().Where(m => m.Quantity < threshold).ToList();

            Console.WriteLine($"Medicines with less than {threshold} units in stock:");
            foreach (var medicine in lowStockMedicines)
            {
                Console.WriteLine($"{medicine.Name} - {medicine.Quantity} units left");
            }
        }

        // Check expiry alerts
        public void CheckExpiryAlerts()
        {
            var today = DateTime.Today;
            var expiringMedicines = _dbHelper.GetAllMedicines().Where(m => m.ExpiryDate <= today.AddMonths(1)).ToList();

            Console.WriteLine("Medicines expiring soon (within 1 month):");
            foreach (var medicine in expiringMedicines)
            {
                Console.WriteLine($"{medicine.Name} - Expiry Date: {medicine.ExpiryDate.ToShortDateString()}");
            }
        }

        // Search for a buyer
        public void SearchBuyer(string name)
        {
            var buyers = _dbHelper.GetAllBuyers().Where(b => b.Name.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();

            if (buyers.Any())
            {
                foreach (var buyer in buyers)
                {
                    Console.WriteLine($"Found: {buyer.Name}, Contact: {buyer.Contact}");
                }
            }
            else
            {
                Console.WriteLine("No buyers found matching your search.");
            }
        }
        public void DisplayBuyersHistory()
        {
            Console.WriteLine("\nSort Buyers By:");
            Console.WriteLine("1. ID");
            Console.WriteLine("2. Name");
            Console.Write("Enter your choice: ");
            string sortChoice = Console.ReadLine();

            var buyers = _dbHelper.GetAllBuyers();

            switch (sortChoice)
            {
                case "1":
                    buyers = buyers.OrderBy(b => b.BuyerId).ToList();
                    break;
                case "2":
                    buyers = buyers.OrderBy(b => b.Name).ToList();
                    break;
                default:
                    Console.WriteLine("Invalid choice. Displaying unsorted list.");
                    break;
            }

            Console.WriteLine("\nBuyers and Purchase History:");
            foreach (var buyer in buyers)
            {
                Console.WriteLine($"\nBuyer ID: {buyer.BuyerId}, Name: {buyer.Name}, Contact: {buyer.Contact}");
                Console.WriteLine("Purchase History:");
                if (buyer.PurchaseHistory.Count == 0)
                {
                    Console.WriteLine("  No purchases recorded.");
                }
                else
                {
                    foreach (var purchase in buyer.PurchaseHistory)
                    {
                        Console.WriteLine($"  - {purchase}");
                    }
                }
            }
        }

        // Search Buyer by ID and show summary
        public void SearchBuyerById()
        {
            Console.Write("\nEnter Buyer ID to search: ");
            int id = int.Parse(Console.ReadLine() ?? "0");

            var buyer = _dbHelper.GetBuyerById(id);
            if (buyer == null)
            {
                Console.WriteLine("Buyer not found.");
                return;
            }

            Console.WriteLine("\nBuyer Summary:");
            Console.WriteLine($"ID: {buyer.BuyerId}");
            Console.WriteLine($"Name: {buyer.Name}");
            Console.WriteLine($"Contact: {buyer.Contact}");
            Console.WriteLine($"Total Purchases: {buyer.PurchaseHistory.Count}");
            Console.WriteLine("Purchase History:");
            if (buyer.PurchaseHistory.Count == 0)
            {
                Console.WriteLine("  No purchases recorded.");
            }
            else
            {
                foreach (var purchase in buyer.PurchaseHistory)
                {
                    Console.WriteLine($"  - {purchase}");
                }
            }
        }
    }
}
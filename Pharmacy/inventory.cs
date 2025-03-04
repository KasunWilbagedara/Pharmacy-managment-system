using System;
using System.Collections.Generic;
using System.Linq;

namespace Pharmacy
{
    public class Inventory
    {
        private readonly DatabaseHelper _dbHelper;
        private AVL_Tree medicineIdTree;    
        private BinarySearchTree medicineNameTree; 
        private AVL_Tree buyerIdTree;       
        private BinarySearchTree buyerNameTree; 

        public Inventory(DatabaseHelper dbHelper)
        {
            _dbHelper = dbHelper;
            medicineIdTree = new AVL_Tree();
            medicineNameTree = new BinarySearchTree();
            buyerIdTree = new AVL_Tree();
            buyerNameTree = new BinarySearchTree();
            InitializeTrees();
        }

        private void InitializeTrees()
        {
            var medicines = _dbHelper.GetAllMedicines();
            foreach (var med in medicines)
            {
                medicineIdTree.Insert(med);
                medicineNameTree.Insert(med);
            }

            var buyers = _dbHelper.GetAllBuyers();
            foreach (var buyer in buyers)
            {
                buyerIdTree.Insert(buyer);
                buyerNameTree.Insert(buyer);
            }
        }

        public void SearchBuyer()
        {
            Console.WriteLine("\nSearch Buyer");
            Console.WriteLine("1. Search by ID (AVL Tree)");
            Console.WriteLine("2. Search by Name (Binary Search Tree)");
            Console.Write("Enter your choice (1 or 2): ");
            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.Write("Enter Buyer ID: ");
                    if (!int.TryParse(Console.ReadLine(), out int id))
                    {
                        Console.WriteLine("Invalid ID. Please enter a number.");
                        return;
                    }
                    var buyerById = buyerIdTree.Search(id) as Buyer;
                    if (buyerById != null)
                        DisplayBuyerDetails(buyerById);
                    else
                        Console.WriteLine($"No buyer found with ID '{id}'.");
                    break;

                case "2":
                    Console.Write("Enter Buyer Name: ");
                    string? name = Console.ReadLine();
                    var buyerByName = buyerNameTree.Search(name ?? "") as Buyer;
                    if (buyerByName != null)
                        DisplayBuyerDetails(buyerByName);
                    else
                        Console.WriteLine($"No buyer found with name '{name ?? "null"}'.");
                    break;

                default:
                    Console.WriteLine("Invalid choice. Please select 1 or 2.");
                    break;
            }
        }

        private void DisplayBuyerDetails(Buyer buyer)
        {
            Console.WriteLine("\nBuyer Details:");
            Console.WriteLine($"ID: {buyer.BuyerId}");
            Console.WriteLine($"Name: {buyer.Name}");
            Console.WriteLine($"Contact: {buyer.Contact}");
            Console.WriteLine("Purchase History:");
            if (buyer.PurchaseHistory.Count == 0)
                Console.WriteLine("  No purchases recorded.");
            else
                buyer.PurchaseHistory.ForEach(p => Console.WriteLine($"  - {p}"));
        }

        public void SearchMedicineByNameOrId()
        {
            Console.WriteLine("\nSearch Medicine");
            Console.WriteLine("1. Search by Name (Binary Search Tree)");
            Console.WriteLine("2. Search by ID (AVL Tree)");
            Console.Write("Enter your choice (1 or 2): ");
            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.Write("Enter Medicine Name: ");
                    string? name = Console.ReadLine();
                    var medicineByName = medicineNameTree.Search(name ?? "") as Medicine;
                    if (medicineByName != null)
                        DisplayMedicineDetails(medicineByName);
                    else
                        Console.WriteLine($"No medicine found with name '{name ?? "null"}'.");
                    break;

                case "2":
                    Console.Write("Enter Medicine ID: ");
                    if (!int.TryParse(Console.ReadLine(), out int id))
                    {
                        Console.WriteLine("Invalid ID.");
                        return;
                    }
                    var medicineById = medicineIdTree.Search(id) as Medicine;
                    if (medicineById != null)
                        DisplayMedicineDetails(medicineById);
                    else
                        Console.WriteLine($"No medicine found with ID '{id}'.");
                    break;

                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
        }

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

        public void AddMedicine(int id, string name, string batchNumber, int quantity, DateTime expiryDate, string supplier, string manufacturer)
        {
            var existingMedicine = _dbHelper.GetMedicineById(id);
            if (existingMedicine != null)
            {
                existingMedicine.Quantity += quantity;
                _dbHelper.UpdateMedicine(existingMedicine);
                Console.WriteLine($"Updated {name} in inventory. New quantity: {existingMedicine.Quantity}");
            }
            else
            {
                Medicine newMedicine = new Medicine(id, name, batchNumber, quantity, expiryDate, supplier, manufacturer);
                _dbHelper.InsertMedicine(newMedicine);
                medicineIdTree.Insert(newMedicine);
                medicineNameTree.Insert(newMedicine);
                Console.WriteLine($"Added new medicine: {name} to inventory.");
            }
        }

        public void SellMedicine()
        {
            Console.WriteLine("\nSell Medicine");
            Console.Write("Enter Buyer ID: ");
            int buyerId = int.Parse(Console.ReadLine() ?? "0");

            var buyer = _dbHelper.GetBuyerById(buyerId);
            if (buyer == null)
            {
                Console.Write("Enter Buyer Name: ");
                string? buyerName = Console.ReadLine();
                Console.Write("Enter Buyer Contact: ");
                string? contact = Console.ReadLine();
                buyer = new Buyer(buyerId, buyerName ?? "", contact ?? "");
                _dbHelper.InsertBuyer(buyer);
                buyerIdTree.Insert(buyer);
                buyerNameTree.Insert(buyer);
                Console.WriteLine($"New buyer {buyer.Name} added.");
            }

            Console.Write("Enter Medicine Name: ");
            string? medicineName = Console.ReadLine();
            var medicine = _dbHelper.GetAllMedicines().FirstOrDefault(m => m.Name.Equals(medicineName, StringComparison.OrdinalIgnoreCase));
            if (medicine == null)
            {
                Console.WriteLine("Medicine not found.");
                return;
            }

            Console.WriteLine($"Current stock of {medicine.Name}: {medicine.Quantity} units");
            Console.Write("Enter Quantity to Sell: ");
            int quantity = int.Parse(Console.ReadLine() ?? "0");

            if (medicine.Quantity < quantity)
            {
                Console.WriteLine($"Not enough stock. Available: {medicine.Quantity}");
                return;
            }

            medicine.Quantity -= quantity;
            _dbHelper.UpdateMedicine(medicine);
            Console.WriteLine($"Updated stock of {medicine.Name}: {medicine.Quantity} units remaining");

            string purchaseDetail = $"Purchased {quantity} units of {medicineName} on {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}";
            _dbHelper.InsertPurchase(buyer.BuyerId, purchaseDetail);
            buyer = _dbHelper.GetBuyerById(buyerId) ?? throw new InvalidOperationException("Failed to retrieve updated buyer");

            Console.WriteLine($"Sale successful! {quantity} units of {medicineName} sold to {buyer.Name}.");
            Console.WriteLine("\nUpdated Buyer Purchase History:");
            buyer.ShowPurchaseHistory();

            // Rebuild trees
            medicineIdTree = new AVL_Tree();
            medicineNameTree = new BinarySearchTree();
            buyerIdTree = new AVL_Tree();
            buyerNameTree = new BinarySearchTree();
            InitializeTrees();
        }

        public Medicine GetMedicineById(int id) => _dbHelper.GetMedicineById(id);
        public Buyer GetBuyerById(int id) => _dbHelper.GetBuyerById(id);
        public void UpdateBuyer(Buyer buyer) => _dbHelper.UpdateBuyer(buyer);
        public void AddBuyer(Buyer buyer)
        {
            _dbHelper.InsertBuyer(buyer);
            buyerIdTree.Insert(buyer);
            buyerNameTree.Insert(buyer);
        }
        public Medicine? GetMedicineByName(string name) => _dbHelper.GetAllMedicines().FirstOrDefault(m => m.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        public void UpdateMedicine(Medicine medicine) => _dbHelper.UpdateMedicine(medicine);
        public List<Medicine> GetAllMedicines() => _dbHelper.GetAllMedicines();
        public void DisplayBuyersHistory()
        {
            Console.WriteLine("\nSort Buyers By:");
            Console.WriteLine("1. ID");
            Console.WriteLine("2. Name");
            Console.Write("Enter your choice: ");
            string? sortChoice = Console.ReadLine();
            var buyers = _dbHelper.GetAllBuyers();
            switch (sortChoice)
            {
                case "1": buyers = buyers.OrderBy(b => b.BuyerId).ToList(); break;
                case "2": buyers = buyers.OrderBy(b => b.Name).ToList(); break;
                default: Console.WriteLine("Invalid choice. Displaying unsorted list."); break;
            }
            Console.WriteLine("\nBuyers and Purchase History:");
            foreach (var buyer in buyers)
            {
                Console.WriteLine($"\nBuyer ID: {buyer.BuyerId}, Name: {buyer.Name}, Contact: {buyer.Contact}");
                buyer.ShowPurchaseHistory();
            }
        }
    }
}
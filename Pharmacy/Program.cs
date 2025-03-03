using System;

namespace Pharmacy
{
    public class Program
    {
        static void Main(string[] args)
        {
            DatabaseHelper dbHelper = new DatabaseHelper();
            dbHelper.InitializeDatabase();
            Inventory pharmacy = new Inventory(dbHelper);

            while (true)
            {
                Console.WriteLine("\nPharmacy Inventory Management System");
                Console.WriteLine("1. Add Medicine");
                Console.WriteLine("2. Modify Medicine Details");
                Console.WriteLine("3. Sell Medicine");
                Console.WriteLine("4. Display All Medicines");
                Console.WriteLine("5. Display Buyers History");
                Console.WriteLine("6. Search Buyer");
                Console.WriteLine("7. Search Medicine");
                Console.WriteLine("8. Exit");
                Console.Write("Enter your choice: ");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddMedicine(pharmacy);
                        break;
                    case "2":
                        ModifyMedicine(pharmacy);
                        break;
                    case "3":
                        pharmacy.SellMedicine();
                        break;
                    case "4":
                        DisplayAllMedicines(pharmacy);
                        break;
                    case "5":
                        DisplayBuyersHistory(pharmacy);
                        break;
                    case "6":
                        pharmacy.SearchBuyer();
                        break;
                    case "7":
                        pharmacy.SearchMedicineByNameOrId();
                        break;
                    case "8":
                        Console.WriteLine("Exiting the system...");
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        static void AddMedicine(Inventory pharmacy)
        {
            Console.WriteLine("\nAdd New Medicine");
            Console.Write("Enter Medicine ID: ");
            int id = int.Parse(Console.ReadLine() ?? "0");
            Console.Write("Enter Medicine Name: ");
            string? name = Console.ReadLine();
            Console.Write("Enter Batch Number: ");
            string? batchNumber = Console.ReadLine();
            Console.Write("Enter Quantity: ");
            int quantity = int.Parse(Console.ReadLine() ?? "0");
            Console.Write("Enter Expiry Date (yyyy-MM-dd): ");
            DateTime expiryDate = DateTime.Parse(Console.ReadLine() ?? DateTime.Now.ToString("yyyy-MM-dd"));
            Console.Write("Enter Supplier: ");
            string? supplier = Console.ReadLine();
            Console.Write("Enter Manufacturer: ");
            string? manufacturer = Console.ReadLine();

            pharmacy.AddMedicine(id, name ?? "", batchNumber ?? "", quantity, expiryDate, supplier ?? "", manufacturer ?? "");
            Console.WriteLine("Medicine added successfully!");
        }

        static void ModifyMedicine(Inventory pharmacy)
        {
            Console.WriteLine("\nModify Medicine Details");
            Console.Write("Enter Medicine ID to modify: ");
            int id = int.Parse(Console.ReadLine() ?? "0");
            var medicine = pharmacy.GetMedicineById(id);
            if (medicine == null)
            {
                Console.WriteLine("Medicine not found.");
                return;
            }

            Console.WriteLine($"Current Details: {medicine.Name}, Batch: {medicine.BatchNumber}, Quantity: {medicine.Quantity}, Expiry: {medicine.ExpiryDate.ToShortDateString()}");
            Console.Write("Enter new Medicine Name (leave blank to keep current): ");
            string? name = Console.ReadLine();
            if (!string.IsNullOrEmpty(name)) medicine.Name = name;
            Console.Write("Enter new Batch Number (leave blank to keep current): ");
            string? batchNumber = Console.ReadLine();
            if (!string.IsNullOrEmpty(batchNumber)) medicine.BatchNumber = batchNumber;
            Console.Write("Enter new Quantity (leave blank to keep current): ");
            string? quantityInput = Console.ReadLine();
            if (!string.IsNullOrEmpty(quantityInput)) medicine.Quantity = int.Parse(quantityInput);
            Console.Write("Enter new Expiry Date (yyyy-MM-dd, leave blank to keep current): ");
            string? expiryDateInput = Console.ReadLine();
            if (!string.IsNullOrEmpty(expiryDateInput)) medicine.ExpiryDate = DateTime.Parse(expiryDateInput);

            pharmacy.UpdateMedicine(medicine);
            Console.WriteLine("Medicine details updated successfully!");
        }

        static void DisplayAllMedicines(Inventory pharmacy)
        {
            Console.WriteLine("\nSort Medicines By:");
            Console.WriteLine("1. Name (Quick Sort)");
            Console.WriteLine("2. Quantity (Bubble Sort)");
            Console.WriteLine("3. Expiry Date (Merge Sort)");
            Console.Write("Enter your choice: ");
            string? sortChoice = Console.ReadLine();
            var medicines = pharmacy.GetAllMedicines();

            switch (sortChoice)
            {
                case "1":
                    Sorting.QuickSortByName(medicines, 0, medicines.Count - 1);
                    break;
                case "2":
                    Sorting.BubbleSortByQuantity(medicines);
                    break;
                case "3":
                    Sorting.MergeSortByExpiry(medicines);
                    break;
                default:
                    Console.WriteLine("Invalid choice. Displaying unsorted list.");
                    break;
            }

            Console.WriteLine("\nList of All Medicines:");
            foreach (var medicine in medicines)
            {
                Console.WriteLine($"ID: {medicine.Id}, Name: {medicine.Name}, Quantity: {medicine.Quantity}, Expiry: {medicine.ExpiryDate.ToShortDateString()}");
            }
        }

        static void DisplayBuyersHistory(Inventory pharmacy)
        {
            pharmacy.DisplayBuyersHistory();
        }
    }
}
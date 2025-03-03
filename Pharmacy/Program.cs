using System;

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

            // Ask for buyer ID
            Console.Write("Enter Buyer ID: ");
            int buyerId = int.Parse(Console.ReadLine());

            // Check if buyer exists
            var buyer = pharmacy.GetBuyerById(buyerId);
            if (buyer == null)
            {
                // If buyer does not exist, ask for name and contact
                Console.Write("Enter Buyer Name: ");
                string name = Console.ReadLine();

                Console.Write("Enter Buyer Contact: ");
                string contact = Console.ReadLine();

                // Create new buyer
                buyer = new Buyer(buyerId, name, contact);
                pharmacy.AddBuyer(buyer);
            }

            // Ask for medicine name
            Console.Write("Enter Medicine Name: ");
            string medicineName = Console.ReadLine();

            // Find medicine by name
            var medicine = pharmacy.GetMedicineByName(medicineName);
            if (medicine == null)
            {
                Console.WriteLine("Medicine not found.");
                return;
            }

            // Ask for quantity
            Console.Write("Enter Quantity: ");
            int quantity = int.Parse(Console.ReadLine());

            // Check if enough stock is available
            if (medicine.Quantity < quantity)
            {
                Console.WriteLine($"Not enough stock. Available: {medicine.Quantity}");
                return;
            }

            // Update medicine quantity
            medicine.Quantity -= quantity;
            pharmacy.UpdateMedicine(medicine);

            // Update buyer's purchase history
            string purchaseDetail = $"Purchased {quantity} units of {medicineName} on {DateTime.Now.ToShortDateString()}";
            buyer.AddPurchase(purchaseDetail);
            pharmacy.UpdateBuyer(buyer);

            // Display confirmation message
            Console.WriteLine($"Sale successful! {quantity} units of {medicineName} sold to {buyer.Name}.");
        }

        // Method to display all medicines
        static void DisplayAllMedicines(Inventory pharmacy)
        {
            Console.WriteLine("\nSort Medicines By:");
            Console.WriteLine("1. Name (Quick Sort)");
            Console.WriteLine("2. Quantity (Bubble Sort)");
            Console.WriteLine("3. Expiry Date (Merge Sort)");
            Console.Write("Enter your choice: ");
            string sortChoice = Console.ReadLine();

            var medicines = pharmacy.GetAllMedicines();

            switch (sortChoice)
            {
                case "1":
                    Sorting.QuickSortByName(medicines, 0, medicines.Count - 1); // Sort by name using Quick Sort
                    break;
                case "2":
                    Sorting.BubbleSortByQuantity(medicines); // Sort by quantity using Bubble Sort
                    break;
                case "3":
                    Sorting.MergeSortByExpiry(medicines); // Sort by expiry date using Merge Sort
                    break;
                default:
                    Console.WriteLine("Invalid choice. Displaying unsorted list.");
                    break;
            }

            Console.WriteLine("\nList of All Medicines:");
            foreach (var medicine in medicines)
            {
                Console.WriteLine($"ID: {medicine.Id}, Name: {medicine.Name}, Batch: {medicine.BatchNumber}, Quantity: {medicine.Quantity}, Expiry: {medicine.ExpiryDate.ToShortDateString()}, Supplier: {medicine.Supplier}, Manufacturer: {medicine.Manufacturer}");
            }
        }
    }
}
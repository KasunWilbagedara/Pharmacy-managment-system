namespace Pharmacy
{
    public class Buyer
    {
        public int BuyerId { get; set; }
        public string Name { get; set; }
        public string Contact { get; set; }
        public List<string> PurchaseHistory { get; set; } = new List<string>(); // Store purchase history

        public Buyer(int id, string name, string contact)
        {
            BuyerId = id;
            Name = name;
            Contact = contact;
        }

        public void AddPurchase(string purchaseDetail)
        {
            PurchaseHistory.Add(purchaseDetail);
        }

        public void ShowPurchaseHistory()
        {
            Console.WriteLine($"\nPurchase History for {Name} (ID: {BuyerId}):");
            if (PurchaseHistory.Count == 0)
            {
                Console.WriteLine("No purchases found.");
            }
            else
            {
                foreach (var purchase in PurchaseHistory)
                {
                    Console.WriteLine($"- {purchase}");
                }
            }
        }
    }
}
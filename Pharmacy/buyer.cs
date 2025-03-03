namespace Pharmacy
{
    public class Buyer
    {
        public int BuyerId { get; set; }
        public string Name { get; set; }
        public string Contact { get; set; }

        public Buyer(int id, string name, string contact)
        {
            BuyerId = id;
            Name = name;
            Contact = contact;
        }
    }
}
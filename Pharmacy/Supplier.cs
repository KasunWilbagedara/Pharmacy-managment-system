namespace Pharmacy
{
    public class Supplier
    {
        public int SupplierId { get; set; }
        public string Name { get; set; }
        public string Contact { get; set; }

        public Supplier(int id, string name, string contact)
        {
            SupplierId = id;
            Name = name;
            Contact = contact;
        }
    }
}
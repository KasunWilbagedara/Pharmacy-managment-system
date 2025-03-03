using System;

namespace Pharmacy
{
    public class Medicine
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string BatchNumber { get; set; }
        public int Quantity { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string Supplier { get; set; }
        public string Manufacturer { get; set; }

        public Medicine(int id, string name, string batchNumber, int quantity, DateTime expiryDate, string supplier, string manufacturer)
        {
            Id = id;
            Name = name;
            BatchNumber = batchNumber;
            Quantity = quantity;
            ExpiryDate = expiryDate;
            Supplier = supplier;
            Manufacturer = manufacturer;
        }
    }
}
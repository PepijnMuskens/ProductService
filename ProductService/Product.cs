using MongoDB.Bson;
using System.Text.Json;
namespace ProductService
{
    public class Product
    {
        public int _id { get; set; }
        public string Name { get; set; }
        public string Discription { get; set; }
        public int EAN { get; set; }
        public double Completeness { get; set; }
        public int Category { get; set; }
        public int Brand { get; set; }
        public int Retailer { get; set; }
        public object Atributes { get; set; }

        public Product(int id, string name, string discription, int ean, int category)
        {
            _id = id;
            Name = name;
            Discription = discription;
            EAN = ean;
            Category = category;
            Atributes = new object();
      
        }

        public void CalculateCompleteness()
        {
            double total = 4;
            double filled = 0;
            if (Name != "") filled++;
            if (Discription != "") filled++;
            if (EAN != 0) filled++;
            if (Category != 0) filled++;

            System.Reflection.PropertyInfo[] props = Atributes.GetType().GetProperties();
            
            Completeness = 100 * (filled / total);
        }
    }
}

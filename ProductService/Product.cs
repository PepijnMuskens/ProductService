using MongoDB.Bson;
using System.Text.Json;
namespace ProductService
{
    public class Product
    {
        public int _id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int EAN { get; set; }
        public double Completeness { get; set; }
        public int category { get; set; }
        public int brand { get; set; }
        public int Retailer { get; set; }
        public object Atributes { get; set; }

        public Product(int id, string name, string discription, int ean, int category)
        {
            _id = id;
            this.name = name;
            this.description = discription;
            EAN = ean;
            this.category = category;
        }

        public void CalculateCompleteness()
        {
            double total = 4;
            double filled = 0;
            if (this.name != "") filled++;
            if (this.description != "") filled++;
            if (EAN != 0) filled++;
            if (this.category != 0) filled++;

            System.Reflection.PropertyInfo[] props = Atributes.GetType().GetProperties();
            
            Completeness = 100 * (filled / total);
        }
    }
}

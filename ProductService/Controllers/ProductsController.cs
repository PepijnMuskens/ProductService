using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;
using System.Text.Json;

namespace ProductService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private IMongoDatabase database;
        
        public ProductsController()
        {
            //MongoClient dbClient = new MongoClient("mongodb+srv://WoCPim:Pim654321@cluster0.juyb1.mongodb.net");
            MongoClient dbClient = new MongoClient("mongodb+srv://Server:1234@cluster0.adbqh.mongodb.net/test");

            database = dbClient.GetDatabase("WoC-Pim");
            
            
        }
        [HttpGet("/product")]
        public string Get(string name)
        {

            try
            {
                var filter = Builders<BsonDocument>.Filter.Eq("Name", name);

                var products = database.GetCollection<BsonDocument>("Products");
                
                var product = products.Find(filter).FirstOrDefault();
                if (product != null)
                {
                    Product P = BsonSerializer.Deserialize<Product>(product);
                    P.CalculateCompleteness();
                    
                    return BsonDocument.Parse(JsonSerializer.Serialize(P)).ToString();
                }
                else
                {
                    return "Error :No products found";
                }
            }
            catch (Exception ex)
            {
                return "error";
            }
            
        }
        [HttpGet("/products")]
        public string Products()
        {
            try
            {
                var products = database.GetCollection<BsonDocument>("Products");
                var documents = products.Find(new BsonDocument()).ToList();

                if (documents != null)
                {
                    string docs = "[";
                    bool first = true;
                    List<Product> productList = new List<Product>();
                    foreach (BsonDocument doc in documents)
                    {
                        Product P = BsonSerializer.Deserialize<Product>(doc);
                        P.CalculateCompleteness();
                        productList.Add(P);
                        if (!first) docs += ",";
                        docs += doc;
                        first = false;
                    }
                    docs += "]";
                    return JsonSerializer.Serialize(productList);
                    return docs;
                }
                else
                {
                    return "Error :No products found";
                }
            }
            catch (Exception ex)
            {
                return "error";
            }

        }
        [HttpGet("/productsFromBrand")]
        public string GetProductsFromBrand(int brand)
        {

            try
            {
                var filter = Builders<BsonDocument>.Filter.Eq("Brand", brand);

                var products = database.GetCollection<BsonDocument>("Products");

                var product = products.Find(filter).FirstOrDefault();
                if (product != null)
                {
                    return product.ToString();
                }
                else
                {
                    return "Error :No products found";
                }
            }
            catch (Exception ex)
            {
                return "error";
            }

        }
        [HttpPost("addproduct")]
        public int Post(string doc)
        {
            try
            {
                Product product = new Product(2, "Burger", "very nice", 1234, 2);
                var text = BsonDocument.Parse(doc);
                Console.WriteLine(text);
                var products = database.GetCollection<BsonDocument>("Products");
                var obj = BsonSerializer.Deserialize<Product>(text);
                products.InsertOne(product.ToBsonDocument());
                return 1;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
            return 0;
        }
    }
}

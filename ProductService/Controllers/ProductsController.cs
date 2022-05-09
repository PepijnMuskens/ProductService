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
                    List<Product> productList = new List<Product>();
                    foreach (BsonDocument doc in documents)
                    {
                        Product P = BsonSerializer.Deserialize<Product>(doc);
                        P.CalculateCompleteness();
                        productList.Add(P);
                    }
                    return JsonSerializer.Serialize(productList);
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

                var documents = products.Find(filter).ToList();
                if (documents != null)
                {
                    List<Product> productList = new List<Product>();
                    foreach (BsonDocument doc in documents)
                    {
                        Product P = BsonSerializer.Deserialize<Product>(doc);
                        P.CalculateCompleteness();
                        productList.Add(P);
                    }
                    return JsonSerializer.Serialize(productList);
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
        public async Task<string> Post(string doc)
        {
            try
            {
                Product product = JsonSerializer.Deserialize<Product>(doc);
                HttpClient client = new HttpClient();
                int brandid = Convert.ToInt32(await client.GetStringAsync("https://1437675.luna.fhict.nl/brand/brands"));

                var products = database.GetCollection<BsonDocument>("Products");

                

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

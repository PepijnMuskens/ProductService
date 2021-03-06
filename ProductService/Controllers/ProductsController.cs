using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace ProductService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private IMongoDatabase database;
        private readonly MongoClient dbClient;

        public ProductsController()
        {
            dbClient = new MongoClient("mongodb+srv://Server:1234@cluster0.adbqh.mongodb.net/test");
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
                    
                    return BsonDocument.Parse(System.Text.Json.JsonSerializer.Serialize(P)).ToString();
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

                    var dotNetObjList = documents.ConvertAll(BsonTypeMapper.MapToDotNetValue);
                    return System.Text.Json.JsonSerializer.Serialize(dotNetObjList);
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
                var filter = Builders<BsonDocument>.Filter.Eq("partner", brand);

                var products = database.GetCollection<BsonDocument>("Products");

                var documents = products.Find(filter).ToList();
                if (documents != null)
                {
                    var dotNetObjList = documents.ConvertAll(BsonTypeMapper.MapToDotNetValue);
                    return System.Text.Json.JsonSerializer.Serialize(dotNetObjList);
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
        [HttpGet("/addproduct")]
        public async Task<string> Post(string doc)
        {
            try
            {
                var products = database.GetCollection<BsonDocument>("Products");
                products.InsertOne(BsonDocument.Parse(doc));

                return "great succes";
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
            return "";
        }

        [HttpGet("/deleteproduct")]
        public async Task<string> delete(int id)
        {
            try
            {
                var products = database.GetCollection<BsonDocument>("Products");
                var filter = Builders<BsonDocument>.Filter.Eq("_id", id);
                products.DeleteOne(filter);

                return "great succes";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return "";
        }
    }
}

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
    public class CategoryController : ControllerBase
    {
        private IMongoDatabase database;
        private readonly MongoClient dbClient;

        public CategoryController()
        {
            dbClient = new MongoClient("mongodb+srv://Server:1234@cluster0.adbqh.mongodb.net/test");
            database = dbClient.GetDatabase("WoC-Pim");
        }
        [HttpGet("/category")]
        public string Get(string name)
        {
            try
            {
                var filter = Builders<BsonDocument>.Filter.Eq("Name", name);

                var categories = database.GetCollection<BsonDocument>("Categories");
                
                var category = categories.Find(filter).FirstOrDefault();
                if (category != null)
                {
                    Category C = BsonSerializer.Deserialize<Category>(category);
                    return BsonDocument.Parse(System.Text.Json.JsonSerializer.Serialize(C)).ToString();
                }
                else
                {
                    return "Error :No categories found";
                }
            }
            catch (Exception ex)
            {
                return "error";
            }
            
        }
        [HttpGet("/categories")]
        public string Categories()
        {
            try
            {
                
                var categories = database.GetCollection<BsonDocument>("Categories");
                var documents = categories.Find(new BsonDocument()).ToList();
                List<Category> categoryList = new List<Category>();
                if (documents != null)
                {
                    
                    foreach (BsonDocument doc in documents)
                    {
                        try
                        {
                            Category C = BsonSerializer.Deserialize<Category>(doc);
                            categoryList.Add(C);
                        }
                        catch
                        {
                            //go on
                        }
                        
                    }
                    return System.Text.Json.JsonSerializer.Serialize(categoryList);
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
    }
}

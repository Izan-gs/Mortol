using MongoDB.Driver;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MongoDB.Bson.IO;
using System.Xml;
using Newtonsoft.Json;
using MongoDB.Bson;

// Saves analytics in MongoDB
public class MongoAnalyticsRepository
{
    private MongoClient client; // Entrance door to Mongo
    private IMongoDatabase database; // Specific database reference
    private IMongoCollection<BsonDocument> collection; // A table in Mongo

    public MongoAnalyticsRepository()
    {
        string connection = "mongodb+srv://izan:izan@cluster0.clypws3.mongodb.net/?appName=Cluster0";
        try
        {
            client = new MongoClient(connection); // Connects the client

            database = client.GetDatabase("MortolDB"); // Selects DB

            collection = database.GetCollection<BsonDocument>("analytics"); // Selects collection
        }
        catch(System.Exception e)
        {
            Debug.LogError("MongoDB Connection Error: " + e.Message);
        }
    }
    // Saves the GameSession
    public async void Save(GameSessionAnalytics session)
    {
        // Settings for Newtonsoft
        var settings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };
        // Newtonsoft
        string json = Newtonsoft.Json.JsonConvert.SerializeObject(session, Newtonsoft.Json.Formatting.Indented, settings);

        // Convert JSON -> BSON document
        BsonDocument document = BsonDocument.Parse(json);

        // Await used in order to be fast and avoiding crashes
        await collection.InsertOneAsync(document);

        Debug.Log("Saved to Mongo");
        Debug.Log(document.ToJson());
        Debug.Log(json);

        // *Possible coroutine implementation* - Log JSON in Unity.
        //string json = JsonUtility.ToJson(session, true);
        //Debug.Log(json);

    }
}
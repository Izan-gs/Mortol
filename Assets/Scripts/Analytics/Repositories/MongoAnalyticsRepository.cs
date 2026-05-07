using MongoDB.Driver;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Saves analytics in MongoDB
public class MongoAnalyticsRepository
{
    private MongoClient client; // Entrance door to Mongo
    private IMongoDatabase database; // Specific database reference
    private IMongoCollection<GameSessionAnalytics> collection; // A table in Mongo

    public MongoAnalyticsRepository()
    {
        string connection = "mongodb+srv://izan:izan@cluster0.clypws3.mongodb.net/?appName=Cluster0";
        try
        {
            client = new MongoClient(connection); // Connects the client

            database = client.GetDatabase("MortolDB"); // Selects DB

            collection = database.GetCollection<GameSessionAnalytics>("analytics"); // Selects collection
        }
        catch(System.Exception e)
        {
            Debug.LogError("MongoDB Connection Error: " + e.Message);
        }
    }
    // Saves the GameSession
    public async void Save(GameSessionAnalytics session)
    {
        // Await used in order to be fast and avoiding crashes
        await collection.InsertOneAsync(session);

        Debug.Log("Saved to Mongo");

        // *Possible coroutine implementation* - Log JSON in Unity.
        string json = JsonUtility.ToJson(session, true);
        Debug.Log(json);
    }
}
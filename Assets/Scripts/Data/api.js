const { MongoClient } = require("mongodb");
const Express = require("express");
const BodyParser = require("body-parser");

const DATABASE_URI =
  "mongodb+srv://nocteln:rUCXu8qnZFmIsvS3@cluster0.cu600.mongodb.net/?retryWrites=true&w=majority&appName=Cluster0";
const PORT = 4000;
const DB_NAME = "blackoutData";

let client;

const app = Express();
app.use(BodyParser.json());
app.use(BodyParser.urlencoded({ extended: true }));

app.listen(PORT, "0.0.0.0", () => {
  console.log(`Server running on port ${PORT}`);
});

async function getClient() {
  if (!client) {
    client = new MongoClient(DATABASE_URI);
    await client.connect();
  }
  return client;
}

app.get("/", (req, res) => {
  res.send("Hello World");
});

app.get("/blackout/users", async (req, res) => {
  try {
    const client = await getClient();
    const database = client.db(DB_NAME);
    const users = database.collection("users");
    const documents = await users.find().toArray();
    res.send(documents);
  } catch (error) {
    console.error("Error fetching users:", error);
    res.status(500).send({ error: "Failed to fetch users" });
  }
});

app.post("/blackout/users", async (req, res) => {
  try {
    const client = await getClient();
    const database = client.db(DB_NAME);
    const users = database.collection("users");
    const result = await users.insertOne({ id: req.body.id });
    res.send(result);
  } catch (error) {
    console.error("Error adding user:", error);
    res.status(500).send({ error: "Failed to add user" });
  }
});

app.delete("/blackout/users", async (req, res) => {
  try {
    const client = await getClient();
    const database = client.db(DB_NAME);
    const users = database.collection("users");
    const result = await users.deleteOne({ id: req.body.id });
    res.send(result);
  } catch (error) {
    console.error("Error deleting user:", error);
    res.status(500).send({ error: "Failed to delete user" });
  }
});

app.get("/blackout/runs", async (req, res) => {
  try {
    const client = await getClient();
    const database = client.db(DB_NAME);
    const runs = database.collection("runs");
    const documents = await runs.find().toArray();
    res.send(documents);
  } catch (error) {
    console.error("Error fetching runs:", error);
    res.status(500).send({ error: "Failed to fetch runs" });
  }
});

app.post("/blackout/runs", async (req, res) => {
  try {
    const client = await getClient();
    const database = client.db(DB_NAME);
    const runs = database.collection("runs");
    const result = await runs.insertOne({
      id: req.body.id,
      nbOfrooms: req.body.nbOfrooms,
      time: req.body.time,
    });
    res.send(result);
  } catch (error) {
    console.error("Error adding run:", error);
    res.status(500).send({ error: "Failed to add run" });
  }
});

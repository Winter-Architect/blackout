const { MongoClient } = require("mongodb");
const Express = require("express");
const BodyParser = require("body-parser");
const cors = require("cors");

const DATABASE_URI =
  "mongodb+srv://nocteln:rUCXu8qnZFmIsvS3@cluster0.cu600.mongodb.net/?retryWrites=true&w=majority&appName=Cluster0";
const PORT = 4000;
const DB_NAME = "blackoutData";

let client;

const app = Express();
app.use(BodyParser.json());
app.use(BodyParser.urlencoded({ extended: true }));
app.use(cors());

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

app.get("/blackout/connectedUsers", async (req, res) => {
  try {
    const client = await getClient();
    const database = client.db(DB_NAME);
    const users = database.collection("connectedUsers");
    const documents = await users.find().toArray();
    res.send(documents);
  } catch (error) {
    console.error("Error fetching users:", error);
    res.status(500).send({ error: "Failed to fetch users" });
  }
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
    const usersConected = database.collection("connectedUsers");

    const result = await users.updateOne(
      { id: req.body.id },
      { $setOnInsert: { id: req.body.id } },
      { upsert: true }
    );

    const result2 = await usersConected.insertOne({ id: req.body.id });
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
    const users = database.collection("connectedUsers");
    const result = await users.deleteOne({ id: req.body.id });
    res.send(result);
  } catch (error) {
    console.error("Error deleting user:", error);
    res.status(500).send({ error: "Failed to delete user" });
  }
});

app.get("/blackout/download", async (req, res) => {
  try {
    const client = await getClient();
    const database = client.db(DB_NAME);
    const users = database.collection("downloads");
    const documents = await users.find().toArray();
    res.send(documents);
  } catch (error) {
    console.error("Error fetching users:", error);
    res.status(500).send({ error: "Failed to fetch users" });
  }
});

app.post("/blackout/download", async (req, res) => {
  try {
    const client = await getClient();
    const database = client.db(DB_NAME);
    const downloads = database.collection("downloads");
    const result = await downloads.insertOne({});
    res.send(result);
  } catch (error) {
    console.error("Error adding download:", error);
    res.status(500).send({ error: "Failed to add download" });
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

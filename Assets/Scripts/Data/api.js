const { MongoClient } = require("mongodb");
const Express = require("express");
const BodyParser = require("body-parser");

const DATABASE_URI =
  "mongodb+srv://nocteln:rUCXu8qnZFmIsvS3@cluster0.cu600.mongodb.net/?retryWrites=true&w=majority&appName=Cluster0/blackoutData";
const PORT = 4000;
const DB_NAME = "blackoutData";

const client = new MongoClient(DATABASE_URI);

var app = Express();
app.use(BodyParser.json());
app.use(BodyParser.urlencoded({ extended: true }));

app.get("/", (req, res) => {
  res.send("Hello World");
});

app.get("/users", async (req, res) => {
  try {
    await client.connect();
    const database = client.db(DB_NAME);
    const users = database.collection("users");

    const cursor = users.find();
    const documents = await cursor.toArray();
    res.send(documents);
  } finally {
    await client.close();
  }
});

app.post("/users", async (req, res) => {
  try {
    await client.connect();
    const database = client.db(DB_NAME);
    const users = database.collection("users");

    const result = await users.insertOne({ id: req.body.id });
    res.send(result);
  } finally {
    await client.close();
  }
});

app.delete("/users", async (req, res) => {
  try {
    await client.connect();
    const database = client.db(DB_NAME);
    const users = database.collection("users");

    const result = await users.deleteOne({ id: req.body.id });
    res.send(result);
  } finally {
    await client.close();
  }
});

app.get("/runs", async (req, res) => {
  try {
    await client.connect();
    const database = client.db(DB_NAME);
    const runs = database.collection("runs");

    const cursor = runs.find();
    const documents = await cursor.toArray();
    res.send(documents);
  } finally {
    await client.close();
  }
});

app.post("/runs", async (req, res) => {
  try {
    await client.connect();
    const database = client.db(DB_NAME);
    const runs = database.collection("runs");

    const result = await runs.insertOne({
      id: req.body.id,
      nbOfrooms: req.body.nbOfrooms,
      time: req.body.time,
    });

    res.send(result);
  } finally {
    await client.close();
  }
});

app.listen(PORT, () => {
  console.log(`Server running on port ${PORT}`);
});

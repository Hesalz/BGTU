const { MongoClient } = require('mongodb');

async function run() {
    const client = new MongoClient('mongodb://127.0.0.1:27017');
    try {
        await client.connect();
        const database = client.db('BSTU');

        const pulpits = database.collection('pulpit');
        await pulpits.insertMany([
            { pulpit: "ИСиТ", pulpit_name: "Информационных систем и технологий", faculty: "ИТ" },
            { pulpit: "ПИ", pulpit_name: "Программной инженерии", faculty: "ИТ" }
        ]);

        const faculties = database.collection('faculty');
        await faculties.insertMany([
            { faculty: "ИЭ", faculty_name: "Инженерно-экономический" },
            { faculty: "ИТ", faculty_name: "Информационных технологий" }
        ]);

        console.log("Коллекции созданы и данные вставлены успешно.");
    } finally {
        await client.close();
    }
}

run().catch(console.dir);
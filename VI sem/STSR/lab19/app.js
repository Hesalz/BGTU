const express = require('express');
const app = express();

const router = require('./router/router');

app.use(express.json());
app.use(express.urlencoded({ extended: true }));

app.use('/', router);

app.use((req, res) => {
    res.status(404).send('Маршрут не найден');
});

app.listen(3000, () => {
    console.log('http://localhost:3000');
});
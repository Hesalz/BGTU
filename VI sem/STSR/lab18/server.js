const http = require('http');
const { sequelize, Faculty, Pulpit, Subject, Teacher, Auditorium_type, Auditorium } = require('./index');
const http_handler = require('./API/allAPI'); 

const server = http.createServer(function (req, res) {
    try {
        http_handler(req, res);
    } catch (err) {
        console.error(err);
    }
}).listen(3000);
console.log("http://localhost:3000");

sequelize.authenticate()
    .then(() => console.log('Connection is successful!'))
    .catch((err) => console.log('Error in database connection', err));

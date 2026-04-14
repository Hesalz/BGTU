const Sequelize = require('sequelize');
require('dotenv').config();

const sequelize = require('./db');

const op = Sequelize.Op;
const Model = Sequelize.Model;

const Auditorium = require('./models/auditorium')(sequelize, Sequelize.DataTypes);
const Auditorium_type = require('./models/auditorium_type')(sequelize, Sequelize.DataTypes);
const Pulpit = require('./models/pulpit')(sequelize, Sequelize.DataTypes);
const Faculty = require('./models/faculty')(sequelize, Sequelize.DataTypes);
const Subject = require('./models/subject')(sequelize, Sequelize.DataTypes);
const Teacher = require('./models/teacher')(sequelize, Sequelize.DataTypes);

const setupAssociations = require('./models/associations');
setupAssociations({ Faculty, Pulpit, Subject, Teacher, Auditorium_type, Auditorium, Sequelize });

async function testConnection() {
    try {
        await sequelize.authenticate();
        console.log('Подключение к БД успешно!');
    } catch (error) {
        console.error('Ошибка подключения:', error.message);
    }
}

testConnection();

module.exports = {
    sequelize,
    Auditorium,
    Auditorium_type,
    Faculty,
    Pulpit,
    Subject,
    Teacher
};
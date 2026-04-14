const Sequelize = require('sequelize'); 
require("dotenv").config();

const sequelize = new Sequelize(
  process.env.DB,
  process.env.USER,
  process.env.PASSWORD,
  {
    host: process.env.HOST,
    port: process.env.SQL_PORT,
    dialect: process.env.DIALECT,
    dialectOptions: {
      options: { encrypt: false },
    },
    pool: {
      max: 10,
      min: 1,
      idle: 20000
  }
  }
);
sequelize.authenticate() 
  .then(() => {
    console.log('Соединение с базой данных установлено');
  })
  .catch(err => {
    console.log('Ошибка при соединении с базой данных', err);
  })
  .finally(() => {
    sequelize.close();
  });

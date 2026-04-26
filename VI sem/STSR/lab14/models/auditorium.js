const Sequelize = require('sequelize');
const Model = Sequelize.Model;
const { Auditorium_type } = require('./auditorium_type');

module.exports = (sequelize, DataTypes) => {
    class Auditorium extends Model { };

    Auditorium.init(
        {
            auditorium: { type: Sequelize.STRING, allowNull: false, primaryKey: true },
            auditorium_name: { type: Sequelize.STRING, allowNull: false },
            auditorium_capacity: { type: Sequelize.INTEGER, allowNull: false },
            auditorium_type: {
                type: Sequelize.STRING, allowNull: false,
                references: { model: Auditorium_type, key: 'auditorium_type' }
            }
        },
        {
            sequelize,
            modelName: 'Auditorium',
            tableName: 'Auditorium',
            timestamps: false
        });
    return Auditorium;
}
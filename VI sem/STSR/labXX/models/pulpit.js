const Sequelize = require('sequelize');
const Model = Sequelize.Model;
const { Faculty } = require('./faculty');

module.exports = (sequelize, DataTypes) => {
    class Pulpit extends Model { };

    Pulpit.init(
        {
            pulpit: { type: Sequelize.STRING, allowNull: false, primaryKey: true },
            pulpit_name: { type: Sequelize.STRING, allowNull: false },
            faculty: {
                type: Sequelize.STRING, allowNull: false,
                references: { model: Faculty, key: 'faculty' }
            }
        },
        {
            sequelize,
            modelName: 'Pulpit',
            tableName: 'Pulpit',
            timestamps: false
        });
    return Pulpit;
}
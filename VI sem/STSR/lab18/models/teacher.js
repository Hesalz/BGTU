const Sequelize = require('sequelize');
const Model = Sequelize.Model;
const { Pulpit } = require('./teacher');

module.exports = (sequelize, DataTypes) => {
    class Teacher extends Model { };

    Teacher.init(
        {
            teacher: { type: Sequelize.STRING, allowNull: false, primaryKey: true },
            teacher_name: { type: Sequelize.STRING, allowNull: false },
            pulpit: {
                type: Sequelize.STRING, allowNull: false,
                references: { model: Pulpit, key: 'pulpit' }
            }
        },
        {
            sequelize,
            modelName: 'Teacher',
            tableName: 'Teacher',
            timestamps: false
        });
    return Teacher;
}
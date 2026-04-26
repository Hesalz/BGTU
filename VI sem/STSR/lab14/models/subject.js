const Sequelize = require('sequelize');
const Model = Sequelize.Model;
const { Pulpit } = require('./pulpit');

module.exports = (sequelize, DataTypes) => {
    class Subject extends Model { };

    Subject.init(
        {
            subject: { type: Sequelize.STRING, allowNull: false, primaryKey: true },
            subject_name: { type: Sequelize.STRING, allowNull: false },
            pulpit: {
                type: Sequelize.STRING, allowNull: false,
                references: { model: Pulpit, key: 'pulpit' }
            }
        },
        {
            sequelize,
            modelName: 'Subject',
            tableName: 'Subject',
            timestamps: false
        });
    return Subject;
}
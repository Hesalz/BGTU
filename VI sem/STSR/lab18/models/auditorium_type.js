const Sequelize = require('sequelize');
const Model = Sequelize.Model;

module.exports = (sequelize, DataTypes) => {
    class Auditorium_type extends Model { };

    Auditorium_type.init(
        {
            auditorium_type: { type: Sequelize.STRING, allowNull: false, primaryKey: true },
            auditorium_typename: { type: Sequelize.STRING, allowNull: false },
        },
        {
            sequelize,
            modelName: 'Auditorium_type',
            tableName: 'Auditorium_type',
            timestamps: false
        });
    return Auditorium_type;
}

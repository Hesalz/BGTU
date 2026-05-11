const Sequelize = require('sequelize');
const Model = Sequelize.Model;

module.exports = (sequelize, DataTypes) => {
    class Faculty extends Model { };

    Faculty.init(
        {
            faculty: { type: Sequelize.STRING, allowNull: false, primaryKey: true },
            faculty_name: { type: Sequelize.STRING, allowNull: false }
        },
        {
            sequelize,
            modelName: 'Faculty',
            tableName: 'Faculty',
            timestamps: false
        }
    )
    return Faculty;
}
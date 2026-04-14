const Sequelize = require('sequelize');
require('dotenv').config();
const op = Sequelize.Op;
const Model = Sequelize.Model;

module.exports = ({ Faculty, Pulpit, Subject, Auditorium_type, Auditorium, Sequelize }) => {
    Pulpit.belongsTo(Faculty, {
        as: 'FACULTY',
        foreignKey: 'faculty',
    });
    Faculty.hasMany(Pulpit, {
        as: 'faculty_pulpits',
        foreignKey: 'faculty',
    });

    Pulpit.hasMany(Subject, {
        as: 'pulpit_subjects',
        foreignKey: 'pulpit'
    });
    Subject.belongsTo(Pulpit, {
        as: 'PULPIT',
        foreignKey: 'pulpit'
    });

    Auditorium_type.hasMany(Auditorium, {
        as: 'auditoriums',
        foreignKey: 'auditorium_type'
    });
    Auditorium.belongsTo(Auditorium_type, {
        as: 'type',
        foreignKey: 'auditorium_type'
    });

    Auditorium.addScope('capacityRange', {
        where: {
            auditorium_capacity: {
                [Sequelize.Op.and]: [
                    { [Sequelize.Op.gte]: 10 },
                    { [Sequelize.Op.lte]: 60 }
                ]
            }
        }
    });
};

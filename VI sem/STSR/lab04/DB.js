var util = require('util');
var ee = require('events');

var db_data = [
    {
        id: 1,
        name: 'Левицкий Антон',
        bday: '1981-05-01'
    },
    {
        id: 2,
        name: 'Халомонов Дмитрий',
        bday: '1992-10-20'
    },
    {
        id: 3,
        name: 'Вельбекин Станислав',
        bday: '2011-12-21'
    },
    {
        id: 4,
        name: 'Постила Валерия',
        bday: '2003-03-06'
    }
];

function DB() {
    this.select = () => { return db_data; };
    this.insert = (par) => { db_data.push(par); };
    this.update = (par) => {
        let index = db_data.findIndex(x => x.id === par.id);
        if (index!== -1) 
            db_data[index] = par;
    };
    this.delete = (par) => {
        let index = db_data.findIndex(x => x.id === par);
        if (index!== -1) 
           return db_data.splice(index, 1);
        return null;
    } 
}

util.inherits(DB, ee.EventEmitter);

exports.DB = DB;
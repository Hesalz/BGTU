var util = require('util')
var events = require('events')

var db_data = [
	{
		id: 1,
		name: 'Савко Сашка',
		bday: '1763-01-01'
	},
	{
		id: 2,
		name: 'Жук Светка',
		bday: '2004-10-20'
	},
	{
		id: 3,
		name: 'Сосновец Дашш',
		bday: '2004-10-26'
	},
	{
		id: 4,
		name: 'Ващилко Дашш',
		bday: '2004-10-06'
	}
];

function DB() {
	this.select = () => {
		return db_data
	}

	this.insert = row => {
		let maxId = 0;
		db_data.forEach(item => {
			if (item.id > maxId) maxId = item.id;
		});
		row.id = maxId + 1;
		db_data.push(row);
		return row;
	}

	this.update = row => {
		const index = db_data.findIndex(item => item.id == row.id)
		if (index === -1) {
			return false
		}

		db_data[index] = row
		return true
	}

	this.delete = id => {
		const index = db_data.findIndex(item => item.id == id)
		if (index === -1) {
			return false
		}

		const deletedRow = db_data.splice(index, 1)[0]
		return deletedRow
	}
}

util.inherits(DB, events.EventEmitter)

exports.DB = DB

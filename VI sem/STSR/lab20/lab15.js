const expressHandlebars = require('express-handlebars');
const express = require('express');
const path = require('path');
const fs = require('fs');
const qs = require('qs');

const handlebars = expressHandlebars.create({
	defaultLayout: 'main', 
	extname: 'hbs',
	helpers: {
		exit: `document.location='/'`
	}
});

let app = express();

app.engine('hbs', handlebars.engine);
app.set('view engine', 'hbs');

const publicPath = path.join(__dirname, 'views', 'public');

app.use(express.static(publicPath));


app.get('/update', async function(req, res) {
	fs.readFile('db.json', 'utf8', (err, users) => {
		if (err) {console.error(err);return;}
		let data = JSON.parse(users);
		let usr = data.find((elem) => elem.id == req.query.id)
		res.render("update.hbs", {
			users:data,
			user:usr, 
			clickable:true
		});
	});
});

app.get('/add', async function(req, res) {
	fs.readFile('db.json', 'utf8', (err, users) => {
		if (err) {console.error(err);return;}
		res.render("add", {users:JSON.parse(users), clickable:true});
	});
});

app.post('/add', async function(req, res) {
	let data = '';
	let filePath = 'db.json';
	req.on('data', (chunk)=>{
		data += chunk;
	});
	req.on('end', () => {
		let DB;

		let name = qs.parse(data)['name'];
		let number = qs.parse(data)['number'];		
		let newUser = {"id" : -1, "name":`${name}`, "number":`${number}`}

		try {
			DB = fs.readFileSync(filePath, 'utf8');
			DB = JSON.parse(DB);
			newUser.id = DB[DB.length - 1].id + 1;
			DB[DB.length] = newUser;
			DB = JSON.stringify(DB, null, 2);
		} catch (error) {
			console.error('Ошибка чтения из файла:', error.message);
		}

		fs.writeFile(filePath, DB, 'utf8', (err) => {
			if (err) {
				console.error('Ошибка записи в файл:', err.message);
			} else {
				console.log('Данные успешно записаны в JSON');
			}
		});

	})

	res.writeHead(302, {
		'Location': '/'
	});
	res.end();
});

app.post('/update', async function(req, res) {
	let data = '';
	let filePath = 'db.json';

	req.on('data', (chunk)=>{
		data += chunk;
	});
	req.on('end', ()=>{
		let DB;

		let name = qs.parse(data)['name'];
		let number = qs.parse(data)['number'];		
		let id = qs.parse(data)['id'];	
		let newUser = {"id" : id, "name":`${name}`, "number":`${number}`}

		try {
			DB = fs.readFileSync(filePath, 'utf8');
			DB = JSON.parse(DB);
			for(let i = 0; i < DB.length; i++){
				if(newUser.id == DB[i].id){
					DB[i] = newUser;
				}
			}
			DB = JSON.stringify(DB, null, 2);
		} catch (error) {
			console.error('Ошибка чтения из файла:', error.message);
		}

		fs.writeFile(filePath, DB, 'utf8', (err) => {
			if (err) {
				console.error('Ошибка записи в файл:', err.message);
			} else {
				console.log('Данные успешно записаны в JSON');
			}
		});

	})
	res.writeHead(302, {
		'Location': '/'
	});
	res.end();
});

app.delete('/delete/:id', async function(req, res) {
	let data = '';
	let filePath = 'db.json';

	req.on('end', ()=>{
		let DB;
		let id = req.params.id;
		let newUser = {"id" : id};

		try {
			DB = fs.readFileSync(filePath, 'utf8');
			DB = JSON.parse(DB);
			for(let i = 0; i < DB.length; i++){
				if(Number(newUser.id) === Number(DB[i].id)){
					DB.splice(i, 1);
				}
			}
			DB = JSON.stringify(DB, null, 2);
		} catch (error) {
			console.error('Ошибка чтения из файла:', error.message);
		}

		fs.writeFile(filePath, DB, 'utf8', (err) => {
			if (err) {
				console.error('Ошибка записи в файл:', err.message);
			} else {
				console.log('Данные успешно записаны в JSON');
			}
		});

	})

	res.writeHead(302, {
		'Location': '/'
	});
	res.end();
});

app.get('/', async function(req, res) {
	fs.readFile('db.json', 'utf8', (err, users) => {
		if (err) {console.error(err);return;}
		res.render("main", {users:JSON.parse(users), clickable:false});
	});
});

const PORT = 3000;

app.listen(PORT, () => {
  console.log(`http://localhost:${PORT}`);
}); 
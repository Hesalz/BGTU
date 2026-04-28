const mongoose = require('mongoose');
const http = require('http');
const url = require('url');

const uri = 'mongodb://127.0.0.1:27017/BSTU';

mongoose.connect(uri)
    .then(() => console.log('MongoDB подключена'))
    .catch(err => {
        console.error('Ошибка подключения:', err);
        process.exit(1);
    });

const facultySchema = new mongoose.Schema({
    faculty: { 
        type: String, 
        required: true, 
        unique: true, 
        trim: true 
    },
    faculty_name: { type: String, required: true },
}, { collection: 'faculty', versionKey: false });

const pulpitSchema = new mongoose.Schema({
    pulpit: { 
        type: String, 
        required: true, 
        unique: true, 
        trim: true 
    },
    pulpit_name: { type: String, required: true },
    faculty: {
        type: String,
        required: true,
        validate: {
            validator: async function(v) {
                return await mongoose.model('Faculty').countDocuments({ faculty: v }) > 0;
            },
            message: 'Факультет "{VALUE}" не существует!'
        }
    }
}, { collection: 'pulpit', versionKey: false });

const Faculty = mongoose.model('Faculty', facultySchema);
const Pulpit = mongoose.model('Pulpit', pulpitSchema);

http.createServer(async (req, res) => {
    res.setHeader('Access-Control-Allow-Origin', '*');
    res.setHeader('Access-Control-Allow-Methods', 'GET, POST, PUT, DELETE, OPTIONS');
    res.setHeader('Access-Control-Allow-Headers', 'Content-Type');

    if (req.method === 'OPTIONS') {
        res.writeHead(200);
        res.end();
        return;
    }

    const parsedUrl = url.parse(req.url, true);
    const parts = parsedUrl.pathname.split('/').filter(Boolean);
    const resource = parts[0] === 'api' ? parts[1] : null;
    const code = parts[2] ? decodeURIComponent(parts[2]) : null;

    try {
        if (req.method === 'GET' && resource === 'faculties') {
            const data = await Faculty.find({});
            sendJson(res, 200, data);
        }
        else if (req.method === 'GET' && resource === 'pulpits') {
            const data = await Pulpit.find({});
            sendJson(res, 200, data);
        }

        else if (req.method === 'POST' && resource === 'faculties') {
            const body = await getBody(req);
            const faculty = new Faculty(JSON.parse(body));
            const saved = await faculty.save();
            sendJson(res, 201, saved);
        }
        else if (req.method === 'POST' && resource === 'pulpits') {
            const body = await getBody(req);
            const pulpit = new Pulpit(JSON.parse(body));
            const saved = await pulpit.save();
            sendJson(res, 201, saved);
        }

        else if (req.method === 'PUT' && resource === 'faculties') {
            const body = await getBody(req);
            const data = JSON.parse(body);
            const updated = await Faculty.findOneAndUpdate(
                { faculty: data.faculty },
                data,
                { new: true, runValidators: true }
            );
            if (!updated) throw { status: 404, message: 'Факультет не найден' };
            sendJson(res, 200, updated);
        }
        else if (req.method === 'PUT' && resource === 'pulpits') {
            const body = await getBody(req);
            const data = JSON.parse(body);
            const updated = await Pulpit.findOneAndUpdate(
                { pulpit: data.pulpit },
                data,
                { new: true, runValidators: true }
            );
            if (!updated) throw { status: 404, message: 'Кафедра не найдена' };
            sendJson(res, 200, updated);
        }

        else if (req.method === 'DELETE' && resource === 'faculties' && code) {
            const pulpitCount = await Pulpit.countDocuments({ faculty: code });
            
            if (pulpitCount > 0) {
                throw { 
                    status: 400, 
                    message: `Нельзя удалить факультет "${code}" — на нём существует ${pulpitCount} кафедр(а/ы)!` 
                };
            }

            const deleted = await Faculty.findOneAndDelete({ faculty: code });
            
            if (!deleted) {
                throw { status: 404, message: 'Факультет не найден' };
            }

            sendJson(res, 200, {
                message: `Факультет "${code}" успешно удалён`,
                deleted: deleted
            });
        }
        else if (req.method === 'DELETE' && resource === 'pulpits' && code) {
            const deleted = await Pulpit.findOneAndDelete({ pulpit: code });
            if (!deleted) throw { status: 404, message: 'Кафедра не найдена' };
            sendJson(res, 200, deleted);
        }

        else {
            throw { status: 404, message: 'Не найдено' };
        }
    }
    catch (err) {
        let status = 500;
        let message = 'Внутренняя ошибка сервера';

        if (err.code === 11000) {
            status = 400;
            const field = Object.keys(err.keyValue)[0];
            message = `${field === 'faculty' ? 'Факультет' : 'Кафедра'} "${err.keyValue[field]}" уже существует!`;
        }
        else if (err.name === 'ValidationError') {
            status = 400;
            message = Object.values(err.errors)[0]?.message || 'Ошибка валидации';
        }
        else if (err.status) {
            status = err.status;
            message = err.message;
        }

        sendJson(res, status, { error: message });
    }
}).listen(3000, () => console.log('Сервер запущен: http://localhost:3000'));

function getBody(req) {
    return new Promise((resolve, reject) => {
        let body = '';
        req.on('data', chunk => body += chunk.toString());
        req.on('end', () => resolve(body));
        req.on('error', reject);
    });
}

function sendJson(res, status, data) {
    res.writeHead(status, { 'Content-Type': 'application/json; charset=utf-8' });
    res.end(JSON.stringify(data));
}
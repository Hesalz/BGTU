const http = require('http');
const xmlbuilder = require('xmlbuilder');

let xmlData = xmlbuilder.create('students')
    .att('faculty', 'Math')
    .att('speciality', 'Math 1')
    .ele('student')
    .ele('name', 'Ivan Ivanov').up()
    .ele('birthday', '2000-01-01').up()
    .end({ pretty: true });

let options = {
    host: 'localhost',
    path: '/xml',
    port: 3000,
    method: 'POST',
    headers: {
        'Content-Type': 'text/xml',
        'Accept': 'text/xml',
        'Content-Length': Buffer.byteLength(xmlData)
    }
};

let req = http.request(options, (res) => {
    let data = '';
    res.on('data', (chunk) => {
        data += chunk.toString('utf8');
    });
    res.on('end', () => {
        console.log('HTTP response:', data);
    });
});

req.on('error', (error) => {
    console.log('HTTP request error:', error.message);
});

req.write(xmlData);
req.end();
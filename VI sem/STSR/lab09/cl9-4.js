let http = require('http');

let JSONData = JSON.stringify({ x: 1, y: 2, s: 'Сообщение' });

let options = {
    host: 'localhost',
    path: '/mypath',
    port: 3000,
    method: 'POST',
    headers: {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
        'Content-Length': Buffer.byteLength(JSONData)
    }
};

let req = http.request(options, (res) => {
    console.log('Status Code:', res.statusCode);
    let data = '';

    res.on('data', (chunk) => {
        data += chunk.toString('utf8');
    });

    res.on('end', () => {
        console.log('HTTP response: body =', data);
        console.log('Parsed response:', JSON.parse(data));
    });
});

req.on('error', (error) => {
    console.log('HTTP request error:', error.message);
});

req.write(JSONData);
req.end();
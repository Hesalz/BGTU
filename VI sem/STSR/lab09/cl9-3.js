let http = require('http');
let query = require('querystring');

let params = query.stringify({ x: 3, y: 4, s: 'xxx' });
console.log('params:', params);

let options = {
    host: 'localhost',
    path: '/mypath',
    port: 3000,
    method: 'POST',
    headers: {
        'Content-Type': 'application/x-www-form-urlencoded',
        'Content-Length': Buffer.byteLength(params)
    }
};

let req = http.request(options, (res) => {
    console.log('Status Code:', res.statusCode);
    let data = '';

    res.on('data', (chunk) => {
        data += chunk.toString('utf8');
    });

    res.on('end', () => {
        console.log('Response Body:', data);
    });
});

req.on('error', (error) => {
    console.log('HTTP request error:', error.message);
});
req.write(params);
req.end();
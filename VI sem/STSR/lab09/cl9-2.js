let http = require('http');
let query = require('querystring');

let params = query.stringify({ x: 3, y: 4 });
let path = `/mypath?${params}`;

console.log('params:', params);
console.log('path:', path);

let options = {
    host: 'localhost',
    path: path,
    port: 3000,
    method: 'GET'
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
req.end();
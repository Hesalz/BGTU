const http = require('http');
const url = require('url');
const fs = require('fs');

function getfactorial(factorial) {
    if (factorial === 0 || factorial === 1) {
        return 1;
    } 
    else {
        return factorial * getfactorial(factorial - 1);
    }
}

http.createServer((req, res) => {
    res.setHeader('Access-Control-Allow-Origin', '*');
    if (req.method === 'GET' && req.url === '/') {
        fs.readFile('fetch.html', 'utf8', (err, data) => {
            if (err) {
                res.writeHead(500);
                res.end('Error loading fetch.html');
            } else {
                res.writeHead(200, { 'Content-Type': 'text/html' });
                res.end(data);
            }
        });
    }
    else if (req.method === 'GET' && req.url.startsWith('/fact')) {

        const queryParams = new URLSearchParams(url.parse(req.url).query);
        const param = queryParams.get('k');
        const factorial = getfactorial(param);

        const response = {
            k: param,
            fact: factorial
        };

        if (param) {
            res.writeHead(200, { 'Content-Type': 'application/json' });
            res.end(JSON.stringify(response));
        }
    } 
    
    
}).listen(5000);
console.log('http://localhost:5000/');
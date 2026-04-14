const http = require('http');
const url = require('url');
const fs = require('fs');

function getfactorial(n, callback) {
    process.nextTick(() => {
        if (n === 0 || n === 1) {
            callback(null, 1);
        } else {
            getfactorial(n - 1, (err, result) => {
                if (err) {
                    return callback(err);
                }

                callback(null, n * result);
            });
        }
    });
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
        const param = parseInt(queryParams.get('k'), 10);

        getfactorial(param, (err, factorial) => {
            if (err) {
                res.writeHead(500);
                res.end('Error calculating factorial');
                return;
            }

            const response = {
                k: param,
                fact: factorial
            };

            res.writeHead(200, { 'Content-Type': 'application/json' });
            res.end(JSON.stringify(response));
        });
    } 
    
}).listen(5000);
console.log('http://localhost:5000/');
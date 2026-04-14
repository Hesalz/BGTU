const http = require('http');
const url = require('url');
const querystring = require('querystring');

const server = http.createServer((req, res) => {
    if (req.method === 'POST' && req.url === '/mypath') {
        let body = '';

        req.on('data', (chunk) => {
            body += chunk.toString(); 
        });

        req.on('end', () => {
            const parsedBody = querystring.parse(body);
            const x = parsedBody.x;
            const y = parsedBody.y;
            const s = parsedBody.s;

            res.writeHead(200, { 'Content-Type': 'text/plain' });
            res.end(`Received x: ${x}, y: ${y}, s: ${s}`);
        });
    } else {
        res.writeHead(404, { 'Content-Type': 'text/plain' });
        res.end('404 Not Found');
    }
});

server.listen(3000, 'localhost', () => {
    console.log('Server is running at http://localhost:3000/');
});
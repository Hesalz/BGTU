const http = require('http');
const url = require('url');

const server = http.createServer((req, res) => {
    if (req.method === 'GET' && req.url.startsWith('/mypath')) {
        const queryObject = url.parse(req.url, true).query;
        const x = queryObject.x;
        const y = queryObject.y;

        res.writeHead(200, { 'Content-Type': 'text/plain' });
        
        res.end(`Received x: ${x}, y: ${y}`);
    } else {
        res.writeHead(404, { 'Content-Type': 'text/plain' });
        res.end('404 Not Found');
    }
});

server.listen(3000, 'localhost', () => {
    console.log('Server is running at http://localhost:3000/');
});
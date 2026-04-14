const http = require('http');

const server = http.createServer((req, res) => {
    if (req.url === '/api/name' && req.method === 'GET') {
        res.writeHead(200, { 'Content-Type': 'text/plain; charset=utf-8' });
        res.end('Бабашинский Глеб Александрович');
    } else {
        res.writeHead(404, { 'Content-Type': 'text/plain' });
        res.end('Not Found');
    }
});

server.listen(5000, () => {
    console.log('Server 02-03 running at http://localhost:5000/');
});
const http = require('http');

const server = http.createServer((req, res) => {
    if (req.method === 'GET' && req.url === '/mypath') {
        res.writeHead(200, { 'Content-Type': 'text/plain' });
        res.end('This is the response from /mypath!');
    } 
    else {
        res.writeHead(404, { 'Content-Type': 'text/plain' });
        res.end('404 Not Found');
    }
});

server.listen(3000, 'localhost', () => {
    console.log('Server is running at http://localhost:3000/');
});
const http = require('http');

const server = http.createServer((req, res) => {
    if (req.method === 'POST' && req.url === '/mypath') {
        let body = '';

        req.on('data', (chunk) => {
            body += chunk.toString();
        });

        req.on('end', () => {
            const parsedBody = JSON.parse(body);
            const x = parsedBody.x;
            const y = parsedBody.y;
            const s = parsedBody.s;

            const response = {
                receivedX: x,
                receivedY: y,
                receivedS: s,
                message: 'Data received successfully!'
            };

            res.writeHead(200, { 'Content-Type': 'application/json' });
            res.end(JSON.stringify(response));
        });
    } else {
        res.writeHead(404, { 'Content-Type': 'application/json' });
        res.end(JSON.stringify({ error: '404 Not Found' }));
    }
});

server.listen(3000, 'localhost', () => {
    console.log('Server is running at http://localhost:3000/');
});
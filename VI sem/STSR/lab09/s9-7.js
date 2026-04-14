const http = require('http');
const fs = require('fs');

const server = http.createServer((req, res) => {
    if (req.method === 'POST' && req.url === '/mypath') {
        let body = [];
        req.on('data', (chunk) => {
            body.push(chunk);
        });
        req.on('end', () => {
            const buffer = Buffer.concat(body);
            
            const boundary = req.headers['content-type'].split('boundary=')[1];
            const boundaryBuffer = Buffer.from(`--${boundary}`);
            
            let start = buffer.indexOf(Buffer.from('\r\n\r\n')) + 4;
            let end = buffer.indexOf(boundaryBuffer, start) - 2;
            
            const fileContent = buffer.slice(start, end);
            
            fs.writeFile('newPng.png', fileContent, (err) => {
                if (err) {
                    res.writeHead(500);
                    return res.end('Error saving file');
                }
                res.writeHead(200, { 'Content-Type': 'text/plain' });
                res.end('File uploaded successfully');
            });
        });
    } else {
        res.writeHead(404);
        res.end('Not found');
    }
});

server.listen(3000, () => {
    console.log('Server is listening on port 3000');
});
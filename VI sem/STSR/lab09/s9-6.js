const http = require('http');
const fs = require('fs');
const path = require('url').parse;

const server = http.createServer((req, res) => {
    if (req.method === 'POST' && req.url === '/mypath') {
        let body = '';
        req.on('data', (chunk) => {
            body += chunk;
        });
        req.on('end', () => {
            const fileData = body.split('\r\n')[4]; 
            fs.writeFile('UploadedFile.txt', fileData, (err) => {
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
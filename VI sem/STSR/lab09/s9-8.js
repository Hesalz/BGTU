const http = require('http');
const fs = require('fs');
const path = require('path');

const server = http.createServer((req, res) => {
    if (req.method === 'GET' && req.url === '/bmp/File.bmp') {
        const filePath = path.join(__dirname, 'File.bmp'); 

        fs.stat(filePath, (err, stat) => {
            if (err) {
                res.writeHead(404);
                return res.end('File not found');
            }

            res.writeHead(200, {
                'Content-Type': 'image/bmp',
                'Content-Length': stat.size
            });

            const readStream = fs.createReadStream(filePath);
            readStream.pipe(res);
        });
    } else {
        res.writeHead(404);
        res.end('Not found');
    }
});

server.listen(3000, () => {
    console.log('Server is listening on port 3000');
});
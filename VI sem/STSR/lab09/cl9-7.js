const http = require('http');
const fs = require('fs');

const boundary = '----WebKitFormBoundary';
const filePath = 'img.png'; 
const fileContent = fs.readFileSync(filePath);

const header1 = Buffer.from(`--${boundary}\r\n` +
    `Content-Disposition: form-data; name="file"; filename="img.png"\r\n` +
    `Content-Type: image/png\r\n\r\n`);

const footer = Buffer.from(`\r\n--${boundary}--\r\n`);

const body = Buffer.concat([header1, fileContent, footer]);

const options = {
    host: 'localhost',
    path: '/mypath',
    port: 3000,
    method: 'POST',
    headers: {
        'Content-Type': `multipart/form-data; boundary=${boundary}`,
        'Content-Length': body.length 
    }
};

const req = http.request(options, (res) => {
    let data = '';
    res.on('data', (chunk) => {
        data += chunk.toString('utf8');
    });
    res.on('end', () => {
        console.log('HTTP response:', data);
    });
});

req.on('error', (error) => {
    console.log('HTTP request error:', error.message);
});
req.write(body);
req.end();
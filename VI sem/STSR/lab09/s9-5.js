const http = require('http');
const xml2js = require('xml2js');
const xmlbuilder = require('xmlbuilder');

const server = http.createServer((req, res) => {
    if (req.method === 'POST' && req.url === '/xml') {
        let body = '';
        req.on('data', (chunk) => {
            body += chunk;
        });
        req.on('end', () => {
            xml2js.parseString(body, (err, result) => {
                if (err) {
                    res.writeHead(400, { 'Content-Type': 'text/xml' });
                    return res.end('<error>Invalid XML</error>');
                }

                console.log('Received data:', JSON.stringify(result, null, 2));

                let responseXml = xmlbuilder.create('response')
                    .ele('status', 'success')
                    .ele('message', 'Student data received successfully')
                    .end({ pretty: true });

                res.writeHead(200, { 'Content-Type': 'text/xml' });
                res.end(responseXml);
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
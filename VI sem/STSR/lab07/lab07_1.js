let http = require('http');
let stat = require('./m07-01')('./static');

let http_handler = (req, res) => {
    if (req.method !== 'GET') {
        res.statusCode = 405;
        res.end('Method Not Allowed');
        return;
    }
    if (stat.isStatic('html', req.url)) {
        stat.sendFile(req, res, { 'Content-Type': 'text/html; charset=utf-8' });
    } else if (stat.isStatic('css', req.url)) {
        stat.sendFile(req, res, { 'Content-Type': 'text/css; charset=utf-8' });
    } else if (stat.isStatic('js', req.url)) {
        stat.sendFile(req, res, { 'Content-Type': 'text/javascript; charset=utf-8' });
    } else if (stat.isStatic('png', req.url)) {
        stat.sendFile(req, res, { 'Content-Type': 'image/png' });
    } else if (stat.isStatic('docx', req.url)) {
        stat.sendFile(req, res, { 'Content-Type': 'application/msword' });
    } else if (stat.isStatic('json', req.url)) {
        stat.sendFile(req, res, { 'Content-Type': 'application/json; charset=utf-8' });
    } else if (stat.isStatic('xml', req.url)) {
        stat.sendFile(req, res, { 'Content-Type': 'application/xml; charset=utf-8' });
    } else if (stat.isStatic('mp4', req.url)) {
        stat.sendFile(req, res, { 'Content-Type': 'video/mp4' });
    } else {
        stat.writeHTTP404(res);
    }
};


let server = http.createServer();
server.listen(3000, () => {
    console.log('Server is running at http://localhost:3000/');
    console.log('http://localhost:3000/html/client.html');
});
server.on('request', http_handler);
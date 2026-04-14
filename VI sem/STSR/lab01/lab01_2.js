var http = require('http');

http.createServer(function (request, response) {
  var body = [];

  request.on('data', function(chunk) {
    body.push(chunk);
  });

  request.on('end', function() {
    body = Buffer.concat(body).toString();

    var html = '<!DOCTYPE html>' +
      '<html>' +
      '<head>' +
      '<meta charset="UTF-8">' +
      '<title>Информация</title>' +
      '<style>' +
      '* { margin: 0; padding: 0; box-sizing: border-box; }' +
      'body { font-family: system-ui, -apple-system, sans-serif; max-width: 1200px; margin: 40px auto; padding: 0 20px; background: #fafafa; }' +
      'h1 { font-size: 28px; font-weight: 400; margin-bottom: 30px; color: #111; letter-spacing: -0.5px; }' +
      'table { width: 100%; background: white; border-radius: 12px; box-shadow: 0 2px 8px rgba(0,0,0,0.04); }' +
      'td { padding: 16px; border-bottom: 1px solid #eee; }' +
      'tr:last-child td { border-bottom: none; }' +
      'td:first-child { width: 180px; font-weight: 500; color: #555; background: #f8f8f8; }' +
      'td:last-child { font-family: "SF Mono", Monaco, monospace; font-size: 14px; color: #333; }' +
      'pre { margin: 0; white-space: pre-wrap; word-break: break-word; }' +
      '</style>' +
      '</head>' +
      '<body>' +
      '<h1>Детали запроса</h1>' +
      '<table>' +
      '<tr><td>Метод</td><td>' + request.method + '</td></tr>' +
      '<tr><td>URL</td><td>' + request.url + '</td></tr>' +
      '<tr><td>HTTP версия</td><td>' + request.httpVersion + '</td></tr>' +
      '<tr><td>Заголовки</td><td><pre>' + JSON.stringify(request.headers, null, 2) + '</pre></td></tr>' +
      '<tr><td>Тело запроса</td><td><pre>' + (body || '(пусто)') + '</pre></td></tr>' +
      '</table>' +
      '</body>' +
      '</html>';

    response.writeHead(200, {'Content-Type': 'text/html'});
    response.end(html);
  });
}).listen(3000);

console.log('http://localhost:3000/');
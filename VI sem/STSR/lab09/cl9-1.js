const http = require('http');

const options = {
  host: 'localhost',
  path: '/mypath',
  port: 3000,
  method: 'GET',
};

const req = http.request(options, (res) => {
  console.log('http.request: method =', options.method);
  console.log('http.request: response =', res.statusCode);
  console.log('http.request: statusMessage =', res.statusMessage);
  console.log('http.request: socket.remoteAddress =', res.socket.remoteAddress);
  console.log('http.request: res.socket.remotePort =', res.socket.remotePort);
  console.log('http.request: res.headers =', res.headers);

  let data = '';

  res.on('data', (chunk) => {
    data += chunk.toString('utf8');
    console.log('http.request: data: body =', data);
  });

  res.on('end', () => {
    console.log('http.request: end: body =', data);
  });
});

req.on('error', (e) => {
  console.error('http.request: error:', e.message);
});

req.end();

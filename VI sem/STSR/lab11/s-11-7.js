const rpcWSS = require('rpc-websockets').Server;

let server = new rpcWSS({ port: 5000, host: 'localhost' });

server.register('A', () => {
  console.log('A');
}).public();

server.register('B', () => {
  console.log('B');
}).public();

server.register('C', () => {
    console.log('C');
  }).public();

console.log('WebSocket сервер запущен на порту 5000');

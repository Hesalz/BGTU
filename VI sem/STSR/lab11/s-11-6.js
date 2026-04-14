const rpcWSS = require('rpc-websockets').Server;
const readline = require('readline');

const rl = readline.createInterface({
    input: process.stdin,
    output: process.stdout
  });

let server = new rpcWSS({ port: 5000, host: 'localhost' });

server.event('A');
server.event('B');
server.event('C');

rl.on('line', (input) => {
    const event = input.trim().toUpperCase();
    if (['A', 'B', 'C'].includes(event)) {
        server.emit(event)
    } else {
      console.log('Введите символ A, B или C для генерации события');
    }
  });

console.log('WebSocket сервер запущен на порту 5000');

const rpcWSC = require('rpc-websockets').Client;
let ws = new rpcWSC('ws://localhost:5000');

ws.on('open', () => {
  ws.subscribe('B');

  ws.on('B', (p) => { console.log('B!'); });
});

console.log('Клиент подключен к серверу');

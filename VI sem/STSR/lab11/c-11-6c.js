const rpcWSC = require('rpc-websockets').Client;
let ws = new rpcWSC('ws://localhost:5000');

ws.on('open', () => {
  ws.subscribe('C');

  ws.on('C', (p) => { console.log('C!'); });
});

console.log('Клиент подключен к серверу');

const rpcWSC = require('rpc-websockets').Client;
let ws = new rpcWSC('ws://localhost:5000');

ws.on('open', () => {
  ws.subscribe('A');

  ws.on('A', (p) => { console.log('A!'); });
});

console.log('Клиент подключен к серверу');

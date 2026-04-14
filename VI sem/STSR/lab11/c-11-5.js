const WebSocket = require('ws');
const rpcWS = require('rpc-websockets').Client;

const ws = new rpcWS('ws://localhost:4000');

ws.on('open', () => {
  ws.call('square', [3]).then((r) => { console.log('square(3):', r); });
  ws.call('square', [5, 4]).then((r) => { console.log('square(5,4):', r); });
  ws.call('sum', [2]).then((r) => { console.log('sum(2):', r); });
  ws.call('sum', [2, 4, 6, 8, 10]).then((r) => { console.log('sum(2,4,6,8,10):', r); });
  ws.call('mul', [3]).then((r) => { console.log('mul(3):', r); });
  ws.call('mul', [3, 5, 7, 9, 11, 13]).then((r) => { console.log('mul(3,5,7,9,11,13):', r); });
  ws.call('fib', [1]).then((r) => { console.log('fib(1):', r); });
  ws.call('fib', [2]).then((r) => { console.log('fib(2):', r); });
  ws.call('fib', [7]).then((r) => { console.log('fib(7):', r); });
  ws.call('fact', [0]).then((r) => { console.log('fact(0):', r); });
  ws.call('fact', [5]).then((r) => { console.log('fact(5):', r); });
  ws.call('fact', [10]).then((r) => { console.log('fact(10):', r); });
});

ws.on('error', (e) => {
  console.error('Error:', e);
});

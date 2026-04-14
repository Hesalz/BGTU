const WebSocket = require('ws');
const readline = require('readline');

const ws = new WebSocket('ws://localhost:5000');

const rl = readline.createInterface({
  input: process.stdin,
  output: process.stdout
});

ws.on('open', () => {
  console.log('Connected to broadcast server');

  rl.on('line', (input) => {
    ws.send(input);
  });
});

ws.on('message', (data) => {
  console.log(data.toString());
});

ws.on('close', () => {
  console.log('Disconnected');
  rl.close();
});

ws.on('error', (err) => {
  console.error('Error:', err);
});

const WebSocket = require('ws');

const ws = new WebSocket('ws://localhost:4000');

let counter = 0;
let interval;

ws.on('open', () => {
  console.log('Connected to server');

  interval = setInterval(() => {
    counter++;
    const msg = `10-01-client: ${counter}`;
    ws.send(msg);
    console.log(`Sent: ${msg}`);
  }, 3000);

  setTimeout(() => {
    clearInterval(interval);
    ws.close();
    console.log('Connection closed by client');
  }, 25000);
});

ws.on('message', (data) => {
  console.log(`Received: ${data.toString()}`);
});

ws.on('close', () => {
  console.log('WebSocket connection closed');
});

ws.on('error', (err) => {
  console.error('Error:', err);
});

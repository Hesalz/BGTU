const WebSocket = require('ws');

const wss = new WebSocket.Server({ port: 5000 }, () => {
  console.log('Broadcast WS server running on ws://localhost:5000');
});

wss.on('connection', (ws) => {
  console.log('New client connected');

  ws.on('message', (message) => {
    console.log('Received:', message.toString());

    wss.clients.forEach((client) => {
      if (client !== ws && client.readyState === WebSocket.OPEN) {
        client.send(`Broadcast: ${message}`);
      }
    });
  });

  ws.on('close', () => {
    console.log('Client disconnected');
  });
});

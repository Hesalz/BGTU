const http = require('http');
const fs = require('fs');
const path = require('path');
const WebSocket = require('ws');

const server = http.createServer((req, res) => {
  if (req.method === 'GET' && req.url === '/start') {
    const filePath = path.join(__dirname, 'public', 'index.html');
    fs.readFile(filePath, (err, data) => {
      if (err) {
        res.writeHead(500);
        return res.end('Server error');
      }
      res.writeHead(200, { 'Content-Type': 'text/html' });
      res.end(data);
    });
  } else {
    res.writeHead(400);
    res.end('Bad Request');
  }
});

server.listen(3000, () => {
  console.log('HTTP server running on http://localhost:3000');
});

const wss = new WebSocket.Server({ port: 4000 }, () => {
  console.log('WebSocket server running on ws://localhost:4000');
});

wss.on('connection', (ws) => {
  console.log('Client connected to WebSocket');
  let clientMsgNumber = 0;
  let serverMsgNumber = 0;

  ws.on('message', (message) => {
    console.log('Received from client:', message.toString());
    const match = message.toString().match(/(\d+)$/);
    if (match) {
      clientMsgNumber = parseInt(match[1], 10);
    }
  });

  const interval = setInterval(() => {
    serverMsgNumber++;
    const msg = `10-01-server: ${clientMsgNumber}->${serverMsgNumber}`;
    ws.send(msg);
  }, 5000);

  ws.on('close', () => {
    console.log('Client disconnected');
    clearInterval(interval);
  });
});

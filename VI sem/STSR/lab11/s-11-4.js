const WebSocket = require('ws');
const wss = new WebSocket.Server({ port: 4000 });

let messageCount = 0;

wss.on('connection', (ws) => {
  console.log('Клиент подключился');

  ws.on('message', (data) => {
    let message = JSON.parse(data);
    console.log(`Получено сообщение от клиента:`, message);

    messageCount++;
    let response = {
      server: messageCount,
      client: message.client,
      timestamp: new Date().toISOString(),
    };

    ws.send(JSON.stringify(response)); 
  });
});

const WebSocket = require('ws');
const wss = new WebSocket.Server({ port: 4000 });

let clients = [];

wss.on('connection', (ws) => {
  console.log('Клиент подключился');
  clients.push(ws);

  ws.on('pong', () => {
    console.log('Получен pong от клиента');
  });

  ws.on('close', () => {
    clients = clients.filter(client => client !== ws);
  });
});

let messageCount = 0;
setInterval(() => {
  messageCount++;
  const message = `11-03-server: ${messageCount}`;
  
  clients.forEach((client) => {
    if (client.readyState === WebSocket.OPEN) {
      client.send(message);
    }
  });

  console.log(`Отправлено сообщение: ${message}`);
}, 15000); 

setInterval(() => {
  let activeClients = 0;
  clients.forEach((client) => {
    if (client.readyState === WebSocket.OPEN) {
      activeClients++;
      client.ping('client ping');
    }
  });
  
  console.log(`Количество активных соединений: ${activeClients}`);
}, 5000); 

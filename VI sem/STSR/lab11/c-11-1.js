const WebSocket = require('ws');
const fs = require('fs');
const path = require('path');
const readline = require('readline');

const rl = readline.createInterface({
  input: process.stdin,
  output: process.stdout
});

rl.question('Введите путь к файлу: ', (inputPath) => {
  const filePath = path.resolve(inputPath);

  if (!fs.existsSync(filePath)) {
    console.error('Файл не существует!');
    rl.close();
    return;
  }

  const fileName = path.basename(filePath);
  
  const ws = new WebSocket('ws://localhost:4000');

  ws.on('open', () => {
    console.log('Соединение установлено');
    ws.send(`FILENAME:${fileName}`);
    console.log(`Отправляем: ${fileName}`);

    const stream = fs.createReadStream(filePath);
    stream.on('data', (chunk) => {
      ws.send(chunk, { binary: true });
      console.log(`Отправлен фрагмент (${chunk.length} байт)`);
    });

    stream.on('end', () => {
      console.log('Отправка завершена');
      ws.close();
      rl.close();
    });
  });

  ws.on('error', (err) => {
    console.error('Ошибка WebSocket:', err);
    rl.close();
  });
});

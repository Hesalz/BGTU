const rpcWSC = require('rpc-websockets').Client;
let ws = new rpcWSC('ws://localhost:5000');
const readline = require('readline');

const rl = readline.createInterface({
    input: process.stdin,
    output: process.stdout
});

ws.on('open', () => {
    rl.on('line', (input) => {
        const event = input.trim().toUpperCase();
        if (['A', 'B', 'C'].includes(event)) {
            ws.notify(event);
        } else {
            console.log('Введите символ A, B или C для отправки');
        }
    });
});

console.log('Клиент подключен к серверу');

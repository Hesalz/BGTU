const rpcWS = require('rpc-websockets').Client;

const client = new rpcWS('ws://localhost:4000');
async function performExpressionCalculation() {
    try {
        await new Promise((resolve, reject) => {
            client.on('open', resolve);
            client.on('error', reject);
        });

        client.login({ login: "gdv", password: "28" })

        let [square3, square54, mulResult, fib7, mul246] = await Promise.all([
            client.call('square', [3]),
            client.call('square', [5, 4]),
            client.call('mul', [3, 5, 7, 9, 11, 13]),
            client.call('fib', [7]),
            client.call('mul', [2, 4, 6])
        ]);

        console.log('square(3):', square3);
        console.log('square(5,4):', square54);
        console.log('mul(3,5,7,9,11,13):', mulResult);
        console.log('fib(7):', fib7);
        console.log('mul(2,4,6):', mul246);

        let fib7Sum = fib7.reduce((acc, num) => acc + num, 0);

        let result = (square3 + square54 + mulResult) + (fib7Sum * mul246);

        console.log(`Результат выражения: ${result}`);
    } catch (e) {
        console.error('Error:', e);
    } finally {
        client.close();
    }
}

performExpressionCalculation();
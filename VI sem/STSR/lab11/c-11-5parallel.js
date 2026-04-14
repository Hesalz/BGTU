const rpcWS = require('rpc-websockets').Client;

const client = new rpcWS('ws://localhost:4000');

async function performRPCParallel() {
  try {
    await new Promise((resolve, reject) => {
      client.on('open', resolve);
      client.on('error', reject);
    });

    client.login({ login: "gdv", password: "28" })

    let [square1, square2, sum1, sum2, mul1, mul2, fib1, fib2, fib7, fact0, fact5, fact10] = await Promise.all([
      client.call('square', [3]),
      client.call('square', [5, 4]),
      client.call('sum', [2]),
      client.call('sum', [2, 4, 6, 8, 10]),
      client.call('mul', [3]),
      client.call('mul', [3, 5, 7, 9, 11, 13]),
      client.call('fib', [1]),
      client.call('fib', [2]),
      client.call('fib', [7]),
      client.call('fact', [0]),
      client.call('fact', [5]),
      client.call('fact', [10])
    ]);

    console.log('square(3):', square1);
    console.log('square(5,4):', square2);
    console.log('sum(2):', sum1);
    console.log('sum(2,4,6,8,10):', sum2);
    console.log('mul(3):', mul1);
    console.log('mul(3,5,7,9,11,13):', mul2);
    console.log('fib(1):', fib1);
    console.log('fib(2):', fib2);
    console.log('fib(7):', fib7);
    console.log('fact(0):', fact0);
    console.log('fact(5):', fact5);
    console.log('fact(10):', fact10);
  } catch (e) {
    console.error('Error:', e);
  } finally {
    client.close();
  }
}

performRPCParallel();

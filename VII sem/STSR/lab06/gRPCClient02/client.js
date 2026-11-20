const grpc = require('@grpc/grpc-js');
const protoLoader = require('@grpc/proto-loader');
const path = require('path');

const definition = protoLoader.loadSync(path.join(__dirname, 'calculator.proto'), {
  keepCase: true,
  longs: String,
  enums: String,
  defaults: true,
  oneofs: true
});

const calculator = grpc.loadPackageDefinition(definition).calculator;
const address = process.argv[2] || 'localhost:5001';
const client = new calculator.Calculator(address, grpc.credentials.createInsecure());

function execute(name, method, request) {
  return new Promise(resolve => {
    client[method](request, (error, response) => {
      if (error) {
        process.stdout.write(`${name}: gRPC error ${error.code} - ${error.details}\n`);
      } else {
        process.stdout.write(`${name}: ${response.result}\n`);
      }
      resolve();
    });
  });
}

async function main() {
  const request = { x: 20, y: 5 };
  await execute('SUM', 'SUM', request);
  await execute('SUB', 'SUB', request);
  await execute('MUL', 'MUL', request);
  await execute('DIV', 'DIV', request);
  await execute('FACT', 'FACT', { x: 5 });
  await execute('DIV by zero', 'DIV', { x: 10, y: 0 });
  await execute('FACT overflow', 'FACT', { x: 13 });
  client.close();
}

main().catch(error => {
  process.stderr.write(`${error.message}\n`);
  client.close();
  process.exitCode = 1;
});

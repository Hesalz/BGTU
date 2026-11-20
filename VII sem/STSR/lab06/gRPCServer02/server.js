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

function reply(result, callback) {
  callback(null, { result });
}

function invalid(message, callback) {
  callback({ code: grpc.status.INVALID_ARGUMENT, message });
}

function overflow(message, callback) {
  callback({ code: grpc.status.OUT_OF_RANGE, message });
}

const service = {
  SUM(call, callback) {
    reply(call.request.x + call.request.y, callback);
  },
  SUB(call, callback) {
    reply(call.request.x - call.request.y, callback);
  },
  MUL(call, callback) {
    reply(call.request.x * call.request.y, callback);
  },
  DIV(call, callback) {
    if (call.request.y === 0) {
      invalid('Division by zero is not allowed.', callback);
      return;
    }
    reply(call.request.x / call.request.y, callback);
  },
  FACT(call, callback) {
    const x = call.request.x;
    if (!Number.isInteger(x) || x < 0) {
      invalid('Factorial requires a non-negative integer.', callback);
      return;
    }
    let result = 1;
    for (let i = 2; i <= x; i++) {
      if (result > Math.floor(2147483647 / i)) {
        overflow('Factorial result exceeds the Int32 range.', callback);
        return;
      }
      result *= i;
    }
    reply(result, callback);
  }
};

const server = new grpc.Server();
server.addService(calculator.Calculator.service, service);
server.bindAsync('0.0.0.0:5001', grpc.ServerCredentials.createInsecure(), (error, port) => {
  if (error) {
    process.stderr.write(`${error.message}\n`);
    process.exit(1);
  }
  process.stdout.write(`gRPCServer02 listening on port ${port}\n`);
  server.start();
});

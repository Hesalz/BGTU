const rpcWS = require('rpc-websockets').Server;

let server = new rpcWS({ port: 4000, host: 'localhost' });

server.setAuth((l) => {
  return (l.login === 'gdv' && l.password === '28');
});

server.register('square', (params) => {
  if (params.length === 1) {
    let r = params[0];
    return Math.PI * r * r; 
  } else if (params.length === 2) {
    let a = params[0];
    let b = params[1];
    return a * b; 
  }
}).public();

server.register('sum', (params) => {
  return params.reduce((a, b) => a + b, 0);
}).public(); 

server.register('mul', (params) => {
  return params.reduce((a, b) => a * b, 1);
}).public(); 

server.register('fib', (params) => {
  let n = params[0];
  let fibSeq = [0, 1];
  for (let i = 2; i < n; i++) {
    fibSeq.push(fibSeq[i - 1] + fibSeq[i - 2]);
  }
  return fibSeq;
}).protected(); 

server.register('fact', (params) => {
  let n = params[0];
  if (n === 0 || n === 1) return 1;
  let fact = 1;
  for (let i = 2; i <= n; i++) {
    fact *= i;
  }
  return fact;
}).protected();

server.register('mod', (params) => {
  return params[0] % params[1];
}).public(); 

server.register('abs', (params) => {
  return Math.abs(params[0]);
}).public(); 

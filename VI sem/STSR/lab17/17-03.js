import { getClient } from './db.js';

const client = await getClient();

await client.set('incr', 0);

let start = Date.now();

const incrPromises = [];
for (let i = 1; i <= 10000; i++) {
    incrPromises.push(client.incr('incr'));
}
await Promise.all(incrPromises);

let incrTime = Date.now() - start;

start = Date.now();

const decrPromises = [];
for (let i = 1; i <= 10000; i++) {
    decrPromises.push(client.decr('incr'));
}
await Promise.all(decrPromises);

let decrTime = Date.now() - start;

console.table([
    { operation: "incr('incr')", time_ms: incrTime },
    { operation: "decr('incr')", time_ms: decrTime }
]);

await client.quit();
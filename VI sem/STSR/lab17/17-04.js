import { getClient } from './db.js';

const client = await getClient();

let start = Date.now();

const hsetPromises = [];
for (let n = 1; n <= 10000; n++) {
    hsetPromises.push(
        client.hSet(`hash:${n}`, {
            id: String(n),
            val: `val-${n}`
        })
    );
}
await Promise.all(hsetPromises);

let hsetTime = Date.now() - start;

start = Date.now();

const hgetPromises = [];
for (let n = 1; n <= 10000; n++) {
    hgetPromises.push(client.hGetAll(`hash:${n}`));
}
await Promise.all(hgetPromises);

let hgetTime = Date.now() - start;

console.table([
    { operation: "hset(n, {id:n, val:'val-n'})", time_ms: hsetTime },
    { operation: "hget(n)", time_ms: hgetTime }
]);

await client.quit();
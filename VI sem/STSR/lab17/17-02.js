import { getClient } from './db.js';

const client = await getClient();

let start = Date.now();

const setPromises = [];
for (let n = 1; n <= 10000; n++) {
    setPromises.push(client.set(String(n), `set${n}`));
}
await Promise.all(setPromises);

let setTime = Date.now() - start;

start = Date.now();

const getPromises = [];
for (let n = 1; n <= 10000; n++) {
    getPromises.push(client.get(String(n)));
}
await Promise.all(getPromises);

let getTime = Date.now() - start;

start = Date.now();

const delPromises = [];
for (let n = 1; n <= 10000; n++) {
    delPromises.push(client.del(String(n)));
}
await Promise.all(delPromises);

let delTime = Date.now() - start;

console.table([
    { operation: "set(n, 'setn')", time_ms: setTime },
    { operation: "get(n)", time_ms: getTime },
    { operation: "del(n)", time_ms: delTime }
]);

await client.quit();
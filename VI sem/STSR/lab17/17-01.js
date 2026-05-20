import { getClient } from './db.js';

const client = await getClient();

await client.set('test', 'Redis connection works');
const value = await client.get('test');

console.log('Соединение успешно');
console.log('test =', value);

await client.quit();
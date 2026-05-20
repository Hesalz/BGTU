import { getClient } from './db.js';

const client = await getClient();

const keys = await client.keys('*');
console.log('Количество ключей:', keys.length);

await client.flushDb();
console.log('Все данные из текущей базы Redis удалены');

await client.quit();
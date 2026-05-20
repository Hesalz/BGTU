import { getClient } from './db.js';

const publisher = await getClient();
const subscriber = await getClient();

await subscriber.subscribe('news', message => {
  console.log('Получено сообщение:', message);
});

setTimeout(async () => {
  await publisher.publish('news', 'Hello from Redis Pub/Sub');
}, 1000);

setTimeout(async () => {
  await subscriber.unsubscribe('news');
  await subscriber.quit();
  await publisher.quit();
}, 3000);
import 'dotenv/config';
import { createClient } from 'redis';

export async function getClient() {
  const client = createClient({
    url: process.env.REDIS_URL
  });

  client.on('error', err => {
    console.log('Redis Client Error:', err);
  });

  await client.connect();
  return client;
}
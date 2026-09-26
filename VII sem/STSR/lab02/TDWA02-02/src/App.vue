<template>
  <main class="shell">
    <header class="masthead">
      <h1>TDWA02-02</h1>
      <p class="sub">SPA · HTTP GET / POST / PUT / DELETE</p>
    </header>

    <section class="panel">
      <h2>01 — Параметры</h2>
      <div class="row">
        <label>
          <span>op</span>
          <select v-model="op">
            <option>add</option>
            <option>sub</option>
            <option>mul</option>
            <option>div</option>
          </select>
        </label>
        <label>
          <span>x</span>
          <input type="number" v-model.number="x">
        </label>
        <label>
          <span>y</span>
          <input type="number" v-model.number="y">
        </label>
      </div>
    </section>

    <section class="panel">
      <h2>02 — Действия</h2>
      <div class="row">
        <button @click="call('GET')">GET</button>
        <button @click="call('POST')">POST</button>
        <button @click="call('PUT')">PUT</button>
        <button @click="call('DELETE')">DELETE</button>
      </div>
    </section>

    <section class="panel">
      <h2>03 — Ответ</h2>
      <pre :class="['out', result?.ok ? 'ok' : 'err']">{{ pretty }}</pre>
    </section>
  </main>
</template>

<script setup>
import { ref, computed } from "vue";
import { send } from "./api.js";

const op = ref("add");
const x  = ref(10);
const y  = ref(5);
const result = ref(null);

const pretty = computed(() =>
    result.value ? JSON.stringify(result.value, null, 2) : "—"
);

async function call(method) {
    result.value = await send(method, { op: op.value, x: x.value, y: y.value });
}
</script>
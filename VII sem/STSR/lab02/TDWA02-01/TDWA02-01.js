const API = "/api/Save-JSON";

const opEl     = document.getElementById("op");
const xEl      = document.getElementById("x");
const yEl      = document.getElementById("y");
const outputEl = document.getElementById("output");

function show(data, ok = true) {
    outputEl.textContent = typeof data === "string"
        ? data
        : JSON.stringify(data, null, 4);
    outputEl.className = ok ? "ok" : "error";
}

function readParams() {
    return {
        op: opEl.value,
        x: Number(xEl.value),
        y: Number(yEl.value)
    };
}

async function handle(method) {
    const { op, x, y } = readParams();
    let url = API;
    const opts = { method, headers: {} };

    try {
        if (method === "GET") {
            const qs = new URLSearchParams({ op, x, y }).toString();
            url = `${API}?${qs}`;
        } else {
            opts.headers["Content-Type"] = "application/json";
            opts.body = JSON.stringify({ op, x, y });
        }

        const res = await fetch(url, opts);
        const text = await res.text();

        let data;
        try { data = JSON.parse(text); } catch { data = text; }

        show({ status: res.status, body: data }, res.ok);
    } catch (e) {
        show("Ошибка сети: " + e.message, false);
    }
}

document.getElementById("btn-get")   .addEventListener("click", () => handle("GET"));
document.getElementById("btn-post")  .addEventListener("click", () => handle("POST"));
document.getElementById("btn-put")   .addEventListener("click", () => handle("PUT"));
document.getElementById("btn-delete").addEventListener("click", () => handle("DELETE"));
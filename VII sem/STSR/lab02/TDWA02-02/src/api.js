const API = "/api/Save-JSON";

export async function send(method, { op, x, y }) {
    let url = API;
    const opts = { method, headers: {} };

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

    return { ok: res.ok, status: res.status, body: data };
}
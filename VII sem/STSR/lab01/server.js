const express = require("express");
const fs = require("fs");
const path = require("path");

const app = express();

const PORT = 40000;
const HOST = "0.0.0.0";

const DATA_FILE = path.join(__dirname, "requests.json");

app.use(express.json());

function loadRequests() {
    if (!fs.existsSync(DATA_FILE)) {
        return [];
    }

    try {
        const data = fs.readFileSync(DATA_FILE, "utf8");

        if (!data.trim()) {
            return [];
        }

        return JSON.parse(data);
    } catch (error) {
        console.error("Ошибка чтения requests.json:", error);
        return [];
    }
}


function saveRequests(requests) {
    fs.writeFileSync(
        DATA_FILE,
        JSON.stringify(requests, null, 4),
        "utf8"
    );
}


function validateRequest(data) {
    if (!data || typeof data !== "object") {
        return false;
    }

    if (!["add", "sub", "mul", "div"].includes(data.op)) {
        return false;
    }

    if (typeof data.x !== "number" || typeof data.y !== "number") {
        return false;
    }

    return true;
}

function calculateResult(data) {
    switch (data.op) {
        case "add":
            return data.x + data.y;

        case "sub":
            return data.x - data.y;

        case "mul":
            return data.x * data.y;

        case "div":
            if (data.y === 0) {
                return null;
            }

            return data.x / data.y;

        default:
            return null;
    }
}

function requestsEqual(a, b) {
    return (a.op === b.op && a.x === b.x && a.y === b.y);
}

app.get("/NGINX-test", (req, res) => {
    const requests = loadRequests();

    const op = req.query.op;
    const x = Number(req.query.x);
    const y = Number(req.query.y);

    const searchRequest = {
        op: op,
        x: x,
        y: y
    };

    const found = requests.find((item) =>
        requestsEqual(item, searchRequest)
    );

    if (!found) {
        return res.status(404).json({
            error: "JSON-запрос не найден"
        });
    }

    return res.status(200).json(found);
});

app.post("/NGINX-test", (req, res) => {
    const data = req.body;

    if (!validateRequest(data)) {
        return res.status(400).json({
            error: "Некорректный JSON-запрос"
        });
    }

    if (data.op === "div" && data.y === 0) {
        return res.status(400).json({
            error: "Деление на ноль невозможно"
        });
    }

    const requests = loadRequests();

    const found = requests.find((item) =>
        requestsEqual(item, data)
    );

    if (found) {
        return res.status(409).json({
            error: "Такой JSON-запрос уже существует",
            request: found
        });
    }

    const newRequest = {
        op: data.op,
        x: data.x,
        y: data.y,
        result: calculateResult(data)
    };

    requests.push(newRequest);

    saveRequests(requests);

    return res.status(200).json(newRequest);
});

app.put("/NGINX-test", (req, res) => {
    const data = req.body;

    if (!validateRequest(data)) {
        return res.status(400).json({
            error: "Некорректный JSON-запрос"
        });
    }

    if (data.op === "div" && data.y === 0) {
        return res.status(400).json({
            error: "Деление на ноль невозможно"
        });
    }

    const requests = loadRequests();

    const index = requests.findIndex((item) =>
        requestsEqual(item, data)
    );

    if (index === -1) {
        return res.status(404).json({
            error: "JSON-запрос не найден"
        });
    }

    requests[index] = {
        op: data.op,
        x: data.x,
        y: data.y,
        result: calculateResult(data)
    };

    saveRequests(requests);

    return res.status(200).json(requests[index]);
});

app.delete("/NGINX-test", (req, res) => {
    const data = req.body;

    if (!validateRequest(data)) {
        return res.status(400).json({
            error: "Некорректный JSON-запрос"
        });
    }

    const requests = loadRequests();

    const index = requests.findIndex((item) =>
        requestsEqual(item, data)
    );

    if (index === -1) {
        return res.status(404).json({
            error: "JSON-запрос не найден"
        });
    }

    const deletedRequest = requests[index];

    requests.splice(index, 1);

    saveRequests(requests);

    return res.status(200).json({
        message: "JSON-запрос удалён",
        request: deletedRequest
    });
});

app.get("/", (req, res) => {
    res.json({
        application: "TDWA01-01",
        status: "running",
        port: PORT,
        endpoint: "/NGINX-test"
    });
});

app.use((error, req, res, next) => {
    if (error instanceof SyntaxError && error.status === 400) {
        return res.status(400).json({
            error: "Некорректный JSON"
        });
    }

    next(error);
});

app.listen(PORT, HOST, () => {
    console.log(`http://localhost:${PORT}`);
});
const express = require("express");
const fs = require("fs");
const path = require("path");

const app = express();

const PORT = 40000;
const HOST = "0.0.0.0";

const DATA_FILE = path.join(__dirname, "requests.json");

app.use(express.json());

// Загружаем единственный запрос (или null, если его нет)
function loadRequest() {
    if (!fs.existsSync(DATA_FILE)) {
        return null;
    }

    try {
        const data = fs.readFileSync(DATA_FILE, "utf8");

        if (!data.trim()) {
            return null;
        }

        return JSON.parse(data);
    } catch (error) {
        console.error("Ошибка чтения requests.json:", error);
        return null;
    }
}

// Сохраняем единственный запрос (или удаляем файл, если null)
function saveRequest(request) {
    if (request === null) {
        if (fs.existsSync(DATA_FILE)) {
            fs.unlinkSync(DATA_FILE);
        }
        return;
    }

    fs.writeFileSync(
        DATA_FILE,
        JSON.stringify(request, null, 4),
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
    return a && b && a.op === b.op && a.x === b.x && a.y === b.y;
}

// GET — 404 если не найден; 200 если найден (вернуть с result)
app.get("/NGINX-test", (req, res) => {
    const stored = loadRequest();

    const searchRequest = {
        op: req.query.op,
        x: Number(req.query.x),
        y: Number(req.query.y)
    };

    if (!stored || !requestsEqual(stored, searchRequest)) {
        return res.status(404).json({
            error: "JSON-запрос не найден"
        });
    }

    return res.status(200).json({
        op: stored.op,
        x: stored.x,
        y: stored.y,
        result: stored.result
    });
});

// POST — 409 если ЛЮБОЙ запрос уже есть; 200 если нет (сохранить + result)
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

    const stored = loadRequest();

    // Если на сервере уже есть ЛЮБОЙ запрос — 409
    if (stored !== null) {
        return res.status(409).json({
            error: "JSON-запрос уже существует",
            request: stored
        });
    }

    const newRequest = {
        op: data.op,
        x: data.x,
        y: data.y,
        result: calculateResult(data)
    };

    saveRequest(newRequest);

    return res.status(200).json(newRequest);
});

// PUT — 404 если запроса нет; 200 если есть (ПОЛНОСТЬЮ заменить + result)
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

    const stored = loadRequest();

    // Если запроса нет вообще — 404
    if (stored === null) {
        return res.status(404).json({
            error: "JSON-запрос не найден"
        });
    }

    // Полностью заменяем существующий запрос новым
    const updatedRequest = {
        op: data.op,
        x: data.x,
        y: data.y,
        result: calculateResult(data)
    };

    saveRequest(updatedRequest);

    return res.status(200).json(updatedRequest);
});

// DELETE — 404 если не найден; 200 если найден (удалить)
app.delete("/NGINX-test", (req, res) => {
    const data = req.body;

    if (!validateRequest(data)) {
        return res.status(400).json({
            error: "Некорректный JSON-запрос"
        });
    }

    const stored = loadRequest();

    if (!stored || !requestsEqual(stored, data)) {
        return res.status(404).json({
            error: "JSON-запрос не найден"
        });
    }

    const deletedRequest = stored;
    saveRequest(null);

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
    console.log(`http://localhost:20000/TDWA02-01/`);
    console.log(`http://localhost:20000/TDWA02-02/`);
});
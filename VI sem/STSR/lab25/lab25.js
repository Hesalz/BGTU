const http = require('http');
const PORT = 3000;

const methods = {
    sum(params) {
        if (!Array.isArray(params)) {
            throw { code: -32602, message: 'Invalid params: array expected' };
        }
        return params.reduce((acc, val) => {
            if (typeof val !== 'number') throw { code: -32602, message: 'All parameters must be numbers' };
            return acc + val;
        }, 0);
    },

    mul(params) {
        if (!Array.isArray(params)) {
            throw { code: -32602, message: 'Invalid params: array expected' };
        }
        return params.reduce((acc, val) => {
            if (typeof val !== 'number') throw { code: -32602, message: 'All parameters must be numbers' };
            return acc * val;
        }, 1);
    },

    div(params) {
        if (!Array.isArray(params) || params.length !== 2) {
            throw { code: -32602, message: 'Invalid params: exactly two numbers required' };
        }
        const [x, y] = params;
        if (typeof x !== 'number' || typeof y !== 'number') {
            throw { code: -32602, message: 'Both parameters must be numbers' };
        }
        if (y === 0) {
            throw { code: -32000, message: 'Division by zero' };
        }
        return x / y;
    },

    proc(params) {
        if (!Array.isArray(params) || params.length !== 2) {
            throw { code: -32602, message: 'Invalid params: exactly two numbers required' };
        }
        const [x, y] = params;
        if (typeof x !== 'number' || typeof y !== 'number') {
            throw { code: -32602, message: 'Both parameters must be numbers' };
        }
        if (y === 0) {
            throw { code: -32000, message: 'Division by zero' };
        }
        return (x / y) * 100;
    }
};

function handleSingleRequest(requestObj) {
    const { jsonrpc, method, params, id } = requestObj;

    if (jsonrpc !== '2.0') {
        return createErrorResponse(-32600, 'Invalid Request: jsonrpc must be "2.0"', id);
    }

    if (typeof method !== 'string' || !(method in methods)) {
        return createErrorResponse(-32601, `Method not found: ${method}`, id);
    }

    try {
        const result = methods[method](params || []);
        if (id === undefined) return null;
        return {
            jsonrpc: '2.0',
            result,
            id
        };
    } catch (err) {
        if (id === undefined) return null;
        return createErrorResponse(err.code || -32603, err.message || 'Internal error', id);
    }
}

function createErrorResponse(code, message, id) {
    return {
        jsonrpc: '2.0',
        error: { code, message },
        id: id === undefined ? null : id
    };
}

function parseBody(req) {
    return new Promise((resolve, reject) => {
        let body = '';
        req.on('data', chunk => body += chunk);
        req.on('end', () => {
            try {
                resolve(JSON.parse(body));
            } catch (e) {
                reject({ code: -32700, message: 'Parse error' });
            }
        });
    });
}

const server = http.createServer(async (req, res) => {
    if (req.method !== 'POST') {
        res.writeHead(405, { 'Content-Type': 'application/json' });
        res.end(JSON.stringify({ error: 'Only POST method is allowed' }));
        return;
    }

    try {
        const payload = await parseBody(req);
        let response;


        if (Array.isArray(payload)) {
            const results = payload.map(handleSingleRequest).filter(r => r !== null);
            if (results.length === 0) {
                res.writeHead(204);
                res.end();
                return;
            }
            response = results;
        } else {
            response = handleSingleRequest(payload);
            if (response === null) {
                res.writeHead(204);
                res.end();
                return;
            }
        }

        res.writeHead(200, { 'Content-Type': 'application/json' });
        res.end(JSON.stringify(response));
    } catch (err) {
        res.writeHead(400, { 'Content-Type': 'application/json' });
        res.end(JSON.stringify({
            jsonrpc: '2.0',
            error: { code: err.code || -32700, message: err.message || 'Parse error' },
            id: null
        }));
    }
});

server.listen(PORT, () => {
    console.log(`http://localhost:${PORT}`);
});

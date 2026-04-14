var http = require('http');
var url = require('url');
var fs = require('fs');
var data = require('./db');

const PATH_DB = '/api/db';
const PATH_STATS = '/api/ss';

var db = new data.DB();

let statsActive = false;
let stats = { start: null, finish: null, requests: 0, commits: 0 };

let stopTimeout = null;
let commitInterval = null;
let statsTimer = null;

db.on('COMMIT', async () => {
    console.log('DB.COMMIT');
})

db.on('GET', (request, response) => {
    console.log('DB.GET');
    response.writeHead(200, { 'content-type': 'application/json;charset=utf-8' });
    response.end(JSON.stringify(db.select()))

});

db.on('POST', (request, response) => {
    console.log('DB.POST');
    let body = ''
    request.on('data', chunk => {
        body += chunk
    })
    request.on('end', () => {
        const row = JSON.parse(body);
        db.insert(row);
        response.writeHead(200, { 'content-type': 'application/json;charset=utf-8' });
        response.end(JSON.stringify(db.select()))
    })
});

db.on('PUT', (request, response) => {
    console.log('DB.PUT');
    let body = ''
    request.on('data', chunk => {
        body += chunk
    })
    request.on('end', () => {
        const row = JSON.parse(body)
        db.update(row);
        response.writeHead(200, { 'content-type': 'application/json;charset=utf-8' });
        response.end(JSON.stringify(db.select()))
    })
});

db.on('DELETE', (request, response) => {
    console.log('DB.DELETE');
    let parsedURL = url.parse(request.url, true);
    const id = parsedURL.query.id;

    if (!id) {
        response.writeHead(400, { 'content-type': 'text/html;charset=utf-8' });
        response.end('<h1>400 Bad request</h1>')
        return
    }

    const deletedRow = db.delete(id);
    response.writeHead(200, { 'content-type': 'application/json;charset=utf-8' });
    response.end(JSON.stringify(deletedRow));
});

const server = http.createServer(function (request, response) {
    const path = url.parse(request.url).pathname

    if (statsActive && (path === PATH_DB || path === PATH_STATS)) {
        stats.requests++
    }

    if (path === PATH_DB) {
        db.emit(request.method, request, response)
    } else if (path === PATH_STATS && request.method === 'GET') {
        console.log('GET /api/ss')
        response.writeHead(200, {
            'content-type': 'application/json;charset=utf-8',
        })
        response.end(JSON.stringify(stats))
    } else if (request.url === '/') {
        let html = fs.readFileSync('./lab05.html')
        response.writeHead(200, { 'content-type': 'text/html;charset=utf-8' })
        response.end(html)
    } else {
        response.writeHead(404, { 'content-type': 'text/plain; charset=utf-8' })
        response.end('Not found')
    }
}).listen(5000)

console.log(`http://localhost:5000`);


process.stdin.setEncoding("utf-8");
process.stdin.on("readable", () => {
    let chunk;
    while ((chunk = process.stdin.read()) !== null) {

        const input = chunk.trim();
        if (!input) continue;

        const [command, parameter] = input.split(' ');

        switch (command) {
            case 'sd': {
                if (stopTimeout) {
                    clearTimeout(stopTimeout);
                    stopTimeout = null;
                }

                if (parameter) {
                    const seconds = parseInt(parameter);
                    stopTimeout = setTimeout(() => {
                        server.close(() => console.log(`Server closed`));
                        process.exit(0);
                    }, seconds * 1000)
                }

                break;
            }

            case 'sc': {
                if (commitInterval) {
                    clearInterval(commitInterval);
                    commitInterval = null;
                    console.log('Interval commits stopped');
                }

                if (parameter) {
                    const seconds = parseInt(parameter);
                    commitInterval = setInterval(() => {
                        db.emit("COMMIT");
                        if (statsActive) stats.commits++;
                    }, seconds * 1000);
                    commitInterval.unref();
                    console.log(`Interval commit started`);
                }

                break;
            }

            case 'ss': {
                if (statsActive) {
                    statsActive = false;
                    stats.finish = new Date().toISOString();
                    if (statsTimer) clearTimeout(statsTimer);
                    console.log('Statistics collection stopped');
                    break;
                }

                if (parameter) {
                    statsActive = true;
                    stats.start = new Date().toISOString();
                    stats.finish = null;
                    stats.requests = 0;
                    stats.commits = 0;
                    const seconds = parseInt(parameter);
                    console.log(`Statistics collection started for ${seconds} seconds.`);
                    statsTimer = setTimeout(() => {
                        statsActive = false;
                        stats.finish = new Date().toISOString();
                        console.log('Statistics collection stopped');
                    }, seconds * 1000);
                    statsTimer.unref();

                }
                else {
                    console.log('No parameter for ss');
                }

                break;
            }

            default: {
                console.log('Invalid command.\n Valid options: sd [x], sc [x], ss [x] (x - seconds)');
                break;
            }
        }
    }
})
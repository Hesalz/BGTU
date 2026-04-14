const http = require('http');
let currentState = 'idle';
process.stdin.setEncoding('utf8');

const server = http.createServer((req, res) => {
    res.writeHead(200, { 'Content-Type': 'text/html; charset=utf-8' });
    res.end(`
        <html>
            <head>
            </head>
            <body>
                <h1>${currentState}</h1>
            </body>
        </html>
    `);
}).listen(5000);
console.log('http://localhost:5000/');

process.stdin.on('readable', () => {

    let chunk = null;
    
    while ((chunk = process.stdin.read()) !== null) {

        const input = chunk.trim();
        
        if (input === 'exit') {
            console.log('exit was inserted');
            process.exit();
        }
        else if (input === 'currstate') {
            console.log(`${currentState}`);
        }
        else if (['norm', 'stop', 'test', 'idle'].includes(input)) {
            console.log(`${currentState}->${input}`);
            currentState = input;
        } else {
            console.log(`something wrong: "${input}"`);
        }
    }
});
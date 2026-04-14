const http = require('http');
const fs = require('fs');

const file = fs.createWriteStream('newFile.bmp'); 

let options = {
    host: 'localhost',
    path: '/bmp/File.bmp', 
    port: 3000,
    method: 'GET'
};

let req = http.request(options, (res) => {
    if (res.statusCode === 200) {
        res.pipe(file); 
        file.on('finish', () => {
            file.close();
            console.log('File downloaded successfully.');
        });
    } else {
        console.log(`Failed to get file: ${res.statusCode}`);
    }
});

req.on('error', (e) => {
    console.log('HTTP request error:', e.message);
});
req.end();
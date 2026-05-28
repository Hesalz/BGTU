const express = require('express')
const https = require('https')
const fs = require('fs')
const path = require('path')

const app = express()

const options = {
    key: fs.readFileSync(path.join(__dirname, 'resource-key.pem')).toString(),
    cert: fs.readFileSync(path.join(__dirname, 'resource-cert.pem')).toString()
}

app.get('/', (req, res) => {
    res.status(200).send('secure data')
})

https.createServer(options, app).listen(3443, () => console.log(`https://localhost:${3443}`))
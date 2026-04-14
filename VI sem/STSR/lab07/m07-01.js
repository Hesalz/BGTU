let fs = require('fs');
let path = require('path');

function Stat(sfn = './static') {
    this.STATIC_FOLDER = sfn;

    let pathStatic = (fn) => {
        return path.join(this.STATIC_FOLDER, fn);
    }

    this.writeHTTP404 = (res) => {
        res.statusCode = 404;
        res.statusMessage = '404 Not Found';
        res.end('Not Found');
    }
    
    let pipeFile = (req, res, headers, urlPath = null) => { 
        res.writeHead(200, headers);
        const filePath = urlPath || req.url.split('?')[0];
        fs.createReadStream(pathStatic(filePath)).pipe(res);
    }

    this.isStatic = (extension, fn) => {
        const urlWithoutQuery = fn.split('?')[0];
        let reg = new RegExp(`^\/.+\.${extension}$`);
        return reg.test(urlWithoutQuery);
    }

    this.sendFile = (req, res, headers) => {
        const urlPath = req.url.split('?')[0];
        
        fs.access(pathStatic(urlPath), fs.constants.R_OK, err => {
            if (err) {
                this.writeHTTP404(res);
            } else {
                const extension = path.extname(urlPath).toLowerCase();
                const filename = path.basename(urlPath);
                
                const urlObj = new URL(req.url, `http://${req.headers.host}`);
                const isDownload = urlObj.searchParams.has('download');
                
                if (isDownload) {
                    headers['Content-Disposition'] = 'attachment; filename="' + filename + '"';
                } 
                else if (extension !== '.json' && extension !== '.xml' && filename !== 'client.html') {
                    headers['Content-Disposition'] = 'attachment; filename="' + filename + '"';
                }
                
                pipeFile(req, res, headers, urlPath);
            }
        });
    }
};

module.exports = (parm) => { return new Stat(parm); }
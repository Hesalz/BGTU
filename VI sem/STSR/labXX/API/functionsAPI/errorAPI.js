module.exports = (res, code, message) => {
    res.writeHead(code, message, { 'Content-Type': 'application/json; charset=utf-8' });
    res.end(`{"code": "${code}", "message": "${message}"}`);
};
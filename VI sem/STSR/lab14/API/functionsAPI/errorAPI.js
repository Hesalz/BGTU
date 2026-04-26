module.exports = (res, code, message) => {
    if (!res.headersSent) {
        try {
            res.writeHead(code, { 'Content-Type': 'application/json; charset=utf-8' });
            res.end(JSON.stringify({ code: code, message: message }));
        } catch (error) {
            console.error('Error in errorAPI:', error);
            try {
                res.end(JSON.stringify({ code: code, message: message }));
            } catch (err) {
                console.error('Failed to send error response:', err);
            }
        }
    } else {
        try {
            res.end(JSON.stringify({ code: code, message: message }));
        } catch (error) {
            console.error('Failed to send error response (headers already sent):', error);
        }
    }
};
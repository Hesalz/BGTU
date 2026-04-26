const url = require('url');
const errorHandler = require('./errorAPI');
const { Subject } = require('../../index');

function addSubject(request, response, body) {
    Subject.create({
        subject: body.subject,
        subject_name: body.subject_name,
        pulpit: body.pulpit
    }).then(result => {
        response.end(JSON.stringify(result));
    }).catch(error => errorHandler(response, 500, error.message));
}

function updateSubject(request, response, body) {
    Subject.update({
        subject_name: body.subject_name
    },
        { where: { subject: body.subject } })
        .then(result => {
            if (result == 0) {
                throw new Error('Subject not exists');
            }
            else {
                response.end(JSON.stringify(body))
            }
        }).catch(error => errorHandler(response, 500, error.message));
}

module.exports = function (request, response) {
    response.writeHead(200, { 'Content-Type': 'application/json; charset=utf-8' });

    switch (request.method) {

        //---------------------GET----------------------------
        case "GET":
            {
                Subject.findAll()
                    .then(result => {
                        response.end(JSON.stringify(result))
                    })
                    .catch(error => errorHandler(response, 500, error.message));

                break;
            }

        //---------------------POST----------------------------
        case "POST":
            {
                let body = '';
                if (url.parse(request.url).pathname === '/api/subjects') {
                    request.on('data', function (data) {
                        body += data.toString();
                    });
                    request.on('end', function () {
                        body = JSON.parse(body);
                        addSubject(request, response, body);
                    });

                    break;
                }
            }

        //---------------------PUT----------------------------
        case "PUT":
            {
                let body = '';
                if (url.parse(request.url).pathname === '/api/subjects') {
                    request.on('data', function (data) {
                        body += data.toString();
                    });
                    request.on('end', function () {
                        body = JSON.parse(body);
                        updateSubject(request, response, body);
                    });

                    break;
                }
            }

        //---------------------DELETE----------------------------
        case "DELETE":
            {
                Subject.findByPk(request.url.split('/')[3])
                    .then(result => {
                        Subject.destroy({ where: { subject: request.url.split('/')[3] } })
                            .then(resultD => {
                                if (resultD == 0) {
                                    throw new Error('Subject not exists');
                                }
                                else {
                                    response.end(JSON.stringify(result))
                                }
                            }).catch(error => errorHandler(response, 500, error.message));;
                    }).catch(error => errorHandler(response, 500, error.message));

                break;
            }

        default:
            {
                errorHandler(response, 405, 'Method Not Allowed');
                break;
            }
    }
}
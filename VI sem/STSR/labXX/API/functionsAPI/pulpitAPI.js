const url = require('url');
const errorHandler = require('./errorAPI');
const { Pulpit } = require('../../index');

function addPulpit(request, response, body) {
    Pulpit.create({
        pulpit: body.pulpit,
        pulpit_name: body.pulpit_name,
        faculty: body.faculty
    }).then(result => {
        response.end(JSON.stringify(result));
    }).catch(error => errorHandler(response, 500, error.message));
}

function updatePulpit(request, response, body) {
    Pulpit.update({
        pulpit_name: body.pulpit_name
    },
        { where: { pulpit: body.pulpit } })
        .then(result => {
            if (result == 0) {
                throw new Error('Pulpit not exists');
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
                Pulpit.findAll()
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
                if (url.parse(request.url).pathname === '/api/pulpits') {
                    request.on('data', function (data) {
                        body += data.toString();
                    });
                    request.on('end', function () {
                        body = JSON.parse(body);
                        addPulpit(request, response, body);
                    });

                    break;
                }
            }

        //---------------------PUT----------------------------
        case "PUT":
            {
                let body = '';
                if (url.parse(request.url).pathname === '/api/pulpits') {
                    request.on('data', function (data) {
                        body += data.toString();
                    });
                    request.on('end', function () {
                        body = JSON.parse(body);
                        updatePulpit(request, response, body);
                    });

                    break;
                }
            }

        //---------------------DELETE----------------------------
        case "DELETE":
            {
                Pulpit.findByPk(request.url.split('/')[3])
                    .then(result => {
                        Pulpit.destroy({ where: { pulpit: request.url.split('/')[3] } })
                            .then(resultD => {
                                if (resultD == 0) {
                                    throw new Error('This pulpit not exists');
                                }
                                else {
                                    response.end(JSON.stringify(result));
                                }
                            })
                            .catch(error => errorHandler(response, 500, error.message));
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
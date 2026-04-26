const url = require('url');
const errorHandler = require('./errorAPI');
const { Teacher } = require('../../index');

function addTeacher(request, response, body) {
    Teacher.create({
        teacher: body.teacher,
        teacher_name: body.teacher_name,
        pulpit: body.pulpit
    }).then(result => {
        response.end(JSON.stringify(result));
    }).catch(error => errorHandler(response, 500, error.message));
}

function updateTeacher(request, response, body) {
    Teacher.update({
        teacher_name: body.teacher_name,
        pulpit: body.pulpit
    },
        { where: { teacher: body.teacher } })
        .then(result => {
            if (result == 0) {
                throw new Error('Teacher not exists');
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
                Teacher.findAll()
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
                if (url.parse(request.url).pathname === '/api/teachers') {
                    request.on('data', function (data) {
                        body += data.toString();
                    });
                    request.on('end', function () {
                        body = JSON.parse(body);
                        addTeacher(request, response, body);
                    });

                    break;
                }
            }

        //---------------------PUT----------------------------
        case "PUT":
            {
                let body = '';
                if (url.parse(request.url).pathname === '/api/teachers') {
                    request.on('data', function (data) {
                        body += data.toString();
                    });
                    request.on('end', function () {
                        body = JSON.parse(body);
                        updateTeacher(request, response, body);
                    });

                    break;
                }
            }

        //---------------------DELETE----------------------------
        case "DELETE":
            {
                Teacher.findByPk(request.url.split('/')[3])
                    .then(result => {
                        Teacher.destroy({ where: { teacher: request.url.split('/')[3] } })
                            .then(resultD => {
                                if (resultD == 0) {
                                    throw new Error('This teacher not exists');
                                }
                                else {
                                    response.end(JSON.stringify(result))
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
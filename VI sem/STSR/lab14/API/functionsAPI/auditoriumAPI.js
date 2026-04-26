const { Sequelize } = require('sequelize');
const url = require('url');
const sequelize = require('../../index');
const errorHandler = require('./errorAPI');
const { Auditorium } = require('../../index');

function addAuditorium(request, response, body) {
    Auditorium.create({
        auditorium: body.auditorium,
        auditorium_name: body.auditorium_name,
        auditorium_type: body.auditorium_type,
        auditorium_capacity: body.auditorium_capacity
    }).then(result => {
        response.end(JSON.stringify(result));
    }).catch(error => errorHandler(response, 500, error.message));
}

function updateAuditorium(request, response, body) {
    Auditorium.update({
        auditorium_name: body.auditorium_name,
        auditorium_type: body.auditorium_type,
        auditorium_capacity: body.auditorium_capacity
    },
        { where: { auditorium: body.auditorium } })
        .then(result => {
            if (result == 0) {
                throw new Error('Auditorium not exists');
            }
            else {
                response.end(JSON.stringify(body))
            }
        }).catch(error => errorHandler(response, 500, error.message));
}

module.exports = async function (request, response) {
    response.writeHead(200, { 'Content-Type': 'application/json; charset=utf-8' });

    switch (request.method) {

        //---------------------GET----------------------------
        case "GET":
            {
                const path = request.url;
                if (/api\/auditoriumswithscope/.test(path)) {
                    let auditoriums = Auditorium.scope('capacityRange').findAll();
                    auditoriums.then(result => {
                        response.end(JSON.stringify(result));
                    }).catch(error => errorHandler(response, 500, error.message));
                }
                else if (/api\/auditoriumstransaction/.test(path)) {
                    const transaction = await sequelize.transaction({
                        isolationLevel: Sequelize.Transaction.ISOLATION_LEVELS.READ_UNCOMMITTED
                    });
                    try {
                        const auditoriums = await Auditorium.update({
                            auditorium_capacity: 0
                        },
                            {
                                where: { auditorium_capacity: { [Sequelize.Op.gt]: 0 } },
                                transaction: transaction,
                            });

                        console.log(await Auditorium.findAll({ transaction: transaction, attributes: ['auditorium_capacity'] }));
                        response.end(JSON.stringify(await Auditorium.findAll({ transaction: transaction, attributes: ['auditorium_capacity'] })));

                        setTimeout(async () => {
                            await transaction.rollback();
                            console.log(
                                await Auditorium.findAll({ attributes: ['auditorium_capacity'] })
                            );
                            //response.end(JSON.stringify(await Auditorium.findAll({attributes: ['auditorium_capacity']})));
                        }, 10000);
                    }
                    catch (error) {
                        await transaction.rollback();
                        throw error;
                    }
                }
                else {
                    Auditorium.findAll()
                        .then(result => {
                            response.end(JSON.stringify(result))
                        }).catch(error => errorHandler(response, 500, error.message));
                }
                break;
            }

        //---------------------POST----------------------------
        case "POST":
            {
                let body = '';
                if (url.parse(request.url).pathname === '/api/auditoriums') {
                    request.on('data', data => {
                        body += data.toString();
                    });
                    request.on('end', () => {
                        body = JSON.parse(body);
                        addAuditorium(request, response, body);
                    });

                    break;
                }
            }

        //---------------------PUT----------------------------
        case "PUT":
            {
                let body = '';
                if (url.parse(request.url).pathname === '/api/auditoriums') {
                    request.on('data', data => {
                        body += data.toString();
                    });
                    request.on('end', () => {
                        body = JSON.parse(body);
                        updateAuditorium(request, response, body);
                    });

                    break;
                }
            }

        //---------------------DELETE----------------------------
        case "DELETE":
            {
                Auditorium.findByPk(request.url.split('/')[3])
                    .then(result => {
                        Auditorium.destroy({ where: { auditorium: request.url.split('/')[3] } })
                            .then(resultD => {
                                if (resultD == 0) {
                                    throw new Error('Auditorium not exists');
                                }
                                else {
                                    response.end(JSON.stringify(result))
                                }
                            }).catch(error => errorHandler(response, 500, error.message));
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
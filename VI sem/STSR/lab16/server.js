const http = require('http');
const { graphql, buildSchema } = require('graphql');
const fs = require('fs');
const { DB } = require('./db_module');
const { handleResponse, handleError } = require('./response_handler');
const resolver = require('./resolver');

let schema;
try {
    const schemaSource = fs.readFileSync('./schema.graphql', 'utf8');
    schema = buildSchema(schemaSource.toString());
} catch (e) {
    console.error('ОШИБКА ЗАГРУЗКИ СХЕМЫ:', e.message);
    process.exit(1); 
}

const server = http.createServer();
const context = {};

const http_handler = (req, res) => {
    if (req.method === 'POST') {
        let reqData = '';
        req.on('data', chunk => { reqData += chunk; });

        req.on('end', () => {
            try {
                let json = JSON.parse(reqData);
                let gqlRequest = json.query || json.mutation; 
                let variables = json.variables ? json.variables : {};

                if (gqlRequest) {
                    graphql({
                        schema: schema,
                        source: gqlRequest,
                        rootValue: resolver,
                        contextValue: context,
                        variableValues: variables
                    })
                    .then(result => {
                        if (result.errors) {
                            let jsonError = JSON.stringify({ errorMessage: result.errors[0].message, errors: result.errors }, null, 4);
                            handleError(res, '\nОшибка GraphQL:\n', jsonError);
                        } else if (result.data) {
                            let jsonResult = JSON.stringify(result.data, null, 4);
                            handleResponse(res, '\nРезультат:\n', jsonResult);
                        }
                    })
                    .catch(err => {
                        handleError(res, '\nОшибка выполнения GraphQL:\n', JSON.stringify({ errorMessage: err.message }));
                    });
                } else {
                    handleError(res, '\nОшибка\n', JSON.stringify({ errorMessage: 'Invalid JSON request. Enter query or mutation' }));
                }
            }
            catch (err) {
                handleError(res, '\nОшибка\n', JSON.stringify({ errorMessage: `Request error: ${err.message}` }));
            }
        });
    }
    else {
        handleError(res, '\nОшибка\n', JSON.stringify({ errorMessage: `Incorrect method. Only POST is supported for GraphQL.` }));
    }
}

DB((err, dbContext) => { 
    if (err) {
        console.error('Ошибка: Невозможно подключиться к базе данных.');
    } else {
        Object.assign(context, dbContext); 
        console.log('\nSuccesfully connected to database.');

        server.listen(3000, () => { 
            console.log('Server running at http://localhost:3000'); 
        })
        .on('error', err => { 
            console.log('Ошибка сервера:', err.code); 
        })
        .on('request', http_handler);
    }
});
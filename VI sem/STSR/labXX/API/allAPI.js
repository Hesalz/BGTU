const pulpitHandler = require("./functionsAPI/pulpitAPI");
const auditoriumtypeHandler = require("./functionsAPI/auditoriumTypeAPI");
const subjectHandler = require("./functionsAPI/subjectAPI");
const auditoriumHandler = require("./functionsAPI/auditoriumAPI");
const teacherHandler = require("./functionsAPI/teacherAPI");
const facultyHandler = require("./functionsAPI/facultyAPI");
const getHandler = require('./functionsAPI/pageAPI');
const errorHandler = require('./functionsAPI/errorAPI');

module.exports=(request, response) => 
{
    const path=request.url;

    switch  (true) 
    {
        case (path === '/'):
        {
            getHandler(request,response); break;
        }
        case (/\/api\/faculties/).test(path):
        {
            facultyHandler(request,response); break;
        }
        case (/\/api\/pulpits/).test(path):
        {
            pulpitHandler(request, response); break;
        }
        case (/\/api\/subjects/).test(path):
        {
            subjectHandler(request, response); break;
        }
        case (/\/api\/auditoriums/).test(path):
        {
            auditoriumHandler(request, response); break;
        }   
        case (/\/api\/auditoriumtypes/).test(path):
        {
            auditoriumtypeHandler(request, response); break;          
        }
        case (/\/api\/teachers/).test(path):
        {
            teacherHandler(request, response); break;
        }
        default:       
        {
            errorHandler(response, 404, 'Not found');
        }
    }
}
const mssql = require('mssql');

let config = {
    user: 'NodeUser', 
    password: '12345', 
    server: 'DESKTOP-DMJJTUE',
    database: 'HKV',
    pool: {max: 10, min: 4},
    options: {
        trustServerCertificate: true,
        enableArithAbort: true,
    },
    connectionTimeout: 4500
};

function DB(callBack) {

    this.getFaculties = (args, context) => {
        return (new mssql.Request())
            .query('select * from faculty')
            .then(record => record.recordset);
    };

    this.getPulpits = (args, context) => {
        return (new mssql.Request())
            .query('select * from pulpit')
            .then(record => record.recordset);
    };

    this.getSubjects = (args, context) => {
        return (new mssql.Request())
            .query('select * from subject')
            .then(record => record.recordset);
    };

    this.getTeachers = (args, context) => {
        return (new mssql.Request())
            .query('select * from teacher')
            .then(record => record.recordset);
    };

    this.getFaculty = (args, context) => {
        return (new mssql.Request())
            .input('faculty', mssql.NVarChar, args.FACULTY)
            .query('select top(1) * from faculty where faculty = @faculty')
            .then(record => record.recordset);
    };

    this.getPulpit = (args, context) => {
        return (new mssql.Request())
            .input('pulpit', mssql.NVarChar, args.PULPIT)
            .query('select top(1) * from pulpit where pulpit = @pulpit')
            .then(record => record.recordset);
    };

    this.getSubject = (args, context) => {
        return (new mssql.Request())
            .input('subject', mssql.NVarChar, args.SUBJECT)
            .query('select top(1) * from subject where subject = @subject')
            .then(record => record.recordset);
    };

    this.getTeacher = (args, context) => {
        return (new mssql.Request())
            .input('teacher', mssql.NVarChar, args.TEACHER)
            .query('select top(1) * from teacher where teacher = @teacher')
            .then(record => record.recordset);
    };


    this.getTeachersByFaculty = (args, context) => {
        return (new mssql.Request())
            .input('faculty', mssql.NVarChar, args.FACULTY)
            .query('select teacher.TEACHER, teacher.TEACHER_NAME, teacher.PULPIT, pulpit.FACULTY from teacher ' +
                'join pulpit on teacher.pulpit = pulpit.pulpit ' +
                'where pulpit.faculty = @faculty')
            .then(record => {
                let rc = [];
                record.recordset.forEach(el => {
                    if (rc.length === 0 || rc[rc.length - 1].FACULTY !== el.FACULTY) {
                        rc.push({
                            FACULTY: el.FACULTY,
                            TEACHERS: []
                        });
                    }
                    rc[rc.length - 1].TEACHERS.push({
                        TEACHER: el.TEACHER,
                        TEACHER_NAME: el.TEACHER_NAME,
                        PULPIT: el.PULPIT
                    });
                });
                return rc;
            });
    };


    this.getSubjectsByFaculties = (args, context) => {
        return (new mssql.Request())
            .input('faculty', mssql.NVarChar, args.FACULTY)
            .query('select subject.SUBJECT, subject.SUBJECT_NAME, subject.PULPIT, pulpit.PULPIT_NAME, pulpit.FACULTY from subject ' +
                'join pulpit on subject.pulpit = pulpit.pulpit ' +
                'where pulpit.faculty = @faculty ORDER BY pulpit.PULPIT')
            .then(record => {
                let rc = [];
                record.recordset.forEach(el => {
                    let lastPulpit = rc.length > 0 ? rc[rc.length - 1] : null;

                    if (!lastPulpit || lastPulpit.PULPIT !== el.PULPIT) {
                        rc.push({
                            PULPIT: el.PULPIT,
                            PULPIT_NAME: el.PULPIT_NAME,
                            FACULTY: el.FACULTY,
                            SUBJECTS: []
                        });
                        lastPulpit = rc[rc.length - 1];
                    }
                    lastPulpit.SUBJECTS.push({
                        SUBJECT: el.SUBJECT,
                        SUBJECT_NAME: el.SUBJECT_NAME,
                        PULPIT: el.PULPIT
                    });
                });
                return rc;
            });
    };

    this.insertFaculty = (args, context) => {
        return (new mssql.Request())
            .input('faculty', mssql.NVarChar, args.FACULTY)
            .input('faculty_name', mssql.NVarChar, args.FACULTY_NAME)
            .query('insert faculty(faculty, faculty_name) values (@faculty, @faculty_name)')
            .then(() => args);
    };

    this.insertPulpit = (args, context) => {
        return (new mssql.Request())
            .input('pulpit', mssql.NVarChar, args.PULPIT)
            .input('pulpit_name', mssql.NVarChar, args.PULPIT_NAME)
            .input('faculty', mssql.NVarChar, args.FACULTY)
            .query('insert pulpit(pulpit, pulpit_name, faculty) values (@pulpit, @pulpit_name, @faculty)')
            .then(() => args);
    };

    this.insertSubject = (args, context) => {
        return (new mssql.Request())
            .input('subject', mssql.NVarChar, args.SUBJECT)
            .input('subject_name', mssql.NVarChar, args.SUBJECT_NAME)
            .input('pulpit', mssql.NVarChar, args.PULPIT)
            .query('insert subject(subject, subject_name, pulpit) values (@subject, @subject_name, @pulpit)')
            .then(() => args);
    };

    this.insertTeacher = (args, context) => {
        return (new mssql.Request())
            .input('teacher', mssql.NVarChar, args.TEACHER)
            .input('teacher_name', mssql.NVarChar, args.TEACHER_NAME)
            .input('pulpit', mssql.NVarChar, args.PULPIT)
            .query('insert teacher(teacher, teacher_name, pulpit) values (@teacher, @teacher_name, @pulpit)')
            .then(() => args);
    };

    this.updateFaculty = (args, context) => {
        return (new mssql.Request())
            .input('faculty', mssql.NVarChar, args.FACULTY)
            .input('faculty_name', mssql.NVarChar, args.FACULTY_NAME)
            .query('update faculty set faculty_name = @faculty_name where faculty = @faculty')
            .then(record => (record.rowsAffected.length > 0 && record.rowsAffected[0] > 0) ? args : null);
    };

    this.updatePulpit = (args, context) => {
        return (new mssql.Request())
            .input('pulpit', mssql.NVarChar, args.PULPIT)
            .input('pulpit_name', mssql.NVarChar, args.PULPIT_NAME)
            .input('faculty', mssql.NVarChar, args.FACULTY)
            .query('update pulpit set pulpit_name = @pulpit_name, faculty = @faculty where pulpit = @pulpit')
            .then(record => (record.rowsAffected.length > 0 && record.rowsAffected[0] > 0) ? args : null);
    };

    this.updateSubject = (args, context) => {
        return (new mssql.Request())
            .input('subject', mssql.NVarChar, args.SUBJECT)
            .input('subject_name', mssql.NVarChar, args.SUBJECT_NAME)
            .input('pulpit', mssql.NVarChar, args.PULPIT)
            .query('update subject set subject_name = @subject_name, pulpit = @pulpit where subject = @subject')
            .then(record => (record.rowsAffected.length > 0 && record.rowsAffected[0] > 0) ? args : null);
    };

    this.updateTeacher = (args, context) => {
        return (new mssql.Request())
            .input('teacher', mssql.NVarChar, args.TEACHER)
            .input('teacher_name', mssql.NVarChar, args.TEACHER_NAME)
            .input('pulpit', mssql.NVarChar, args.PULPIT)
            .query('update teacher set teacher_name = @teacher_name, pulpit = @pulpit where teacher = @teacher')
            .then(record => (record.rowsAffected.length > 0 && record.rowsAffected[0] > 0) ? args : null);
    };

   this.delFaculty = (args, context) => {
        return (new mssql.Request())
            .input('faculty', mssql.NVarChar, args.FACULTY)
            .query('delete from faculty where faculty = @faculty')
            .then(record => ({ success: record.rowsAffected[0] > 0 }));
    };

    this.delPulpit = (args, context) => {
        return (new mssql.Request())
            .input('pulpit', mssql.NVarChar, args.PULPIT)
            .query('delete from pulpit where pulpit = @pulpit')
            .then(record => ({ success: record.rowsAffected[0] > 0 }));
    };

    this.delSubject = (args, context) => {
        return (new mssql.Request())
            .input('subject', mssql.NVarChar, args.SUBJECT)
            .query('delete from subject where subject = @subject')
            .then(record => ({ success: record.rowsAffected[0] > 0 }));
    };

    this.delTeacher = (args, context) => {
        return (new mssql.Request())
            .input('teacher', mssql.NVarChar, args.TEACHER)
            .query('delete from teacher where teacher = @teacher')
            .then(record => ({ success: record.rowsAffected[0] > 0 }));
    };

    this.connect = mssql.connect(config, err => {
        err ? callBack(err, null) : callBack(null, this);
    });
}

exports.DB = callBack => new DB(callBack);
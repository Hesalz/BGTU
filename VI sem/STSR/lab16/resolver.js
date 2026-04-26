const resolver = {
    getFaculties: async (args, context) => (args.FACULTY) ? await context.getFaculty(args, context) : await context.getFaculties(args, context),
    getPulpits: async (args, context) => (args.PULPIT) ? await context.getPulpit(args, context) : await context.getPulpits(args, context),
    getSubjects: async (args, context) => (args.SUBJECT) ? await context.getSubject(args, context) : await context.getSubjects(args, context),
    getTeachers: async (args, context) => (args.TEACHER) ? await context.getTeacher(args, context) : await context.getTeachers(args, context),
    getTeachersByFaculty: async (args, context) => await context.getTeachersByFaculty(args, context),
    getSubjectsByFaculties: async (args, context) => await context.getSubjectsByFaculties(args, context),

    setFaculty: async (args, context) => {
        let res = await context.updateFaculty(args, context);
        return (res == null) ? await context.insertFaculty(args, context) : res;
    },
    setPulpit: async (args, context) => {
        let res = await context.updatePulpit(args, context);
        return (res == null) ? await context.insertPulpit(args, context) : res;
    },
    setSubject: async (args, context) => {
        let res = await context.updateSubject(args, context);
        return (res == null) ? await context.insertSubject(args, context) : res;
    },
    setTeacher: async (args, context) => {
        let res = await context.updateTeacher(args, context);
        return (res == null) ? await context.insertTeacher(args, context) : res;
    },

    delFaculty: async (args, context) => {
        return await context.delFaculty(args, context);
    },
    delPulpit: async (args, context) => {
        return await context.delPulpit(args, context);
    },
    delSubject: async (args, context) => {
        return await context.delSubject(args, context);
    },
    delTeacher: async (args, context) => {
        return await context.delTeacher(args, context);
    }
};

module.exports = resolver;
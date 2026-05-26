const express = require('express');
const passport = require('passport');
const BasicStrategy = require('passport-http').BasicStrategy;
const Users = require('./users.json');
const session = require('express-session')(
    {
        resave: false,
        saveUninitialized: false,
        secret: '1111'
    }
);

const getCredential = (user) => {
    let u = Users.find((e) => {return e.user.toUpperCase() == user.toUpperCase();});
    return u;
};

const verPassword = (pass1, pass2) => {return pass1 == pass2};


const app = express();
app.use(session);
app.use(passport.initialize());

passport.use(new BasicStrategy((user, password, done) => {
    console.log('passport.use', user, password);
    let rc = null;
    let cr = getCredential(user);
    if (!cr) {
        rc = done(null, false, {message: 'Incorrect username'});
    } else if (!verPassword(cr.password, password)) {
        rc = done(null, false, {message: 'Incorrect password'});
    } else {
        rc = done(null, user);
    }

    return rc;
}))

app.get('/login',
    (req, res, next) => {
        console.log('preAuth');
        if (req.session.logout) {
            req.session.logout = false;
            delete req.headers['authorization'];
        }
        next();
    }, 
    passport.authenticate('basic', { session: false }),
    (req, res, next) => {
        res.redirect('/resource')
        next();
    }
);

app.get('/logout', (req, res) => {
    console.log('Logout');
    req.session.logout = true;
    res.redirect('/login');
});

app.get('/resource', 
    passport.authenticate('basic', { session: false }),
    (req, res) => {
        res.send('RESOURCE');
});

app.use((req, res) => {
    console.log('Error 404');
    res.status(404).send('Сообщение со статусом 404');
});

app.listen(3000, () => console.log('Сервер слушает порт 3000'));
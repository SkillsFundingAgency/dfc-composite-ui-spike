'use strict';
const debug = require('debug');
const express = require('express');
const path = require('path');
const logger = require('morgan');
const cookieParser = require('cookie-parser');
const bodyParser = require('body-parser');
const passport = require('passport');
const googleStrategy = require('passport-google-oauth20');
const cookieSession = require('cookie-session');
const configService = require('./services/configService');
const correlationIdMiddleware = require('./middleware/correlationIdMiddleware');

var app = express();

//config settings
var appConfig = new configService();
var googleConfig = appConfig.getGoogle();

//cookies
app.use(cookieSession({
    maxAge: 24 * 60 * 60 * 1000, // One day in milliseconds
    keys: ['randomstringhere']
}));

//passport
app.use(passport.initialize());
app.use(passport.session());

passport.serializeUser((user, done) => {
    done(null, user);
});

passport.deserializeUser((user, done) => {
    done(null, user);
});

passport.use(new googleStrategy({
    clientID: googleConfig.clientId,
    clientSecret: googleConfig.clientSecret,
    callbackURL: googleConfig.callbackURL
},
    function (accessToken, refreshToken, profile, done) {
        if (!profile.email && profile.emails.length > 0) {
            profile.email = profile.emails[0].value;
        }
        return done(null, profile);
    }
));

//prompts user to sign in to google
app.get('/auth/google', passport.authenticate('google', {
    scope: [
        'https://www.googleapis.com/auth/userinfo.profile',
        'https://www.googleapis.com/auth/userinfo.email'
    ]
}));

//handles the sign in from google
app.get('/auth/google/callback',
    passport.authenticate('google', { failureRedirect: '/login' }),
    function (req, res) {
        res.redirect('/');
    });

// view engine setup
app.set('views', path.join(__dirname, 'views'));
app.set('view engine', 'raz');

// uncomment after placing your favicon in /public
app.use(logger('dev'));
app.use(bodyParser.json());
app.use(bodyParser.urlencoded({ extended: false }));
app.use(cookieParser());
app.use(express.static(path.join(__dirname, 'public')));
app.use(correlationIdMiddleware.correlationIdProvider);

//override render to add access to data neeed in razor views
app.use(function (req, res, next) {
    var render = res.render;
    res.render = function (view, options, fn) {
        var self = this,
            options = options || {},
            req = this.req,
            app = req.app,
            defaultFn;

        if ('function' == typeof options) {
            fn = options, options = {};
        }

        //Add the req to the options as well the app config settings
        options.applications = appConfig.getApplications();
        if (req.session != null && req.session.passport != null) {
            options.user = req.session.passport.user;
        }

        //Add the details of the current app
        var applicationId = req.params.applicationId;
        var application = appConfig.getApplication(applicationId);
        if (application == null) {
            application = appConfig.getShell();
        }
        options.currentApplication = application;

        defaultFn = function (err, str) {
            if (err) return req.next(err);
            self.send(str);
        };

        if ('function' != typeof fn) {
            fn = defaultFn;
        }

        render.call(self, view, options, function (err, str) {
            fn(err, str);
        });
    };
    next();
});

//routes
var routes = require('./routes/index');
var routesApplication = require('./routes/application');
var routesAccount = require('./routes/account');
app.use('/', routes);
app.use('/application', routesApplication);
app.use('/account', routesAccount);

// catch 404 and forward to error handler
app.use(function (req, res, next) {
    var err = new Error('Not Found');
    err.status = 404;
    next(err);
});

// error handling
// development error handler
// will print stacktrace
if (app.get('env') === 'development') {
    app.use(function (err, req, res, next) {
        res.status(err.status || 500);
        res.render('error', {
            message: err.message,
            error: err
        });
    });
}
// production error handler
// no stacktraces leaked to user
app.use(function (err, req, res, next) {
    res.status(err.status || 500);
    res.render('error', {
        message: err.message,
        error: {}
    });
});

app.set('port', process.env.PORT || 3000);

var server = app.listen(app.get('port'), function () {
    debug('Express server listening on port ' + server.address().port);
});

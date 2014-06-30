require.config({    
    paths: {
        'angular': '../Scripts/angular',
        'angular-route': '../Scripts/angular-route',
        'angular-sanitize': '../Scripts/angular-sanitize',
        'infinite-scroll': '../Scripts/ng-infinite-scroll',
        'bind-once': '../Scripts/bindonce',
        'jquery': '../Scripts/jquery-2.1.0',
        'boots': '../Scripts/ui-bootstrap-tpls-0.11.0',
        'modernizr': '../Scripts/Modernizr',
        'file-upload': '../Scripts/angular-file-upload'

    },
    shim: {
        'app': {
            deps: ['angular-route', 'angular-sanitize', 'infinite-scroll', 'boots', 'bind-once','file-upload','modernizr']
        },
        'angular-route': {
            deps: ['angular']
        },
        'bind-once': {
            deps: ['angular']
        },
        'infinite-scroll': {
            deps: ['angular']
        },       
        'angular-sanitize': {
            deps: ['angular']
        },
        'angular': {
            deps: ['jquery']
        },
        'boots': {
            deps: ['angular']
        },
        'file-upload': {
            deps: ['angular']
        }
    }
});

require
(
    ['app'],
    function (app) {
        angular.bootstrap(document, ['app']);
    }
);
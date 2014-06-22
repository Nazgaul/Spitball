require.config({    
    paths: {
        'angular': '../Scripts/angular',
        'angular-route': '../Scripts/angular-route',
        'jquery': '../Scripts/jquery-2.1.0',
        'boots': '../Scripts/ui-bootstrap-tpls-0.11.0'
        
    },
    shim: {
        'app': {
            deps: ['angular', 'angular-route', 'boots']
        },
        'angular-route': {
            deps: ['angular']
        },
        'angular': {
            deps: ['jquery']
        },
        'boots': {
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
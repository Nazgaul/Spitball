require.config({    
    paths: {
        'angular': '../Scripts/angular',
        'angular-route': '../Scripts/angular-route',
        'angular-sanitize': '../Scripts/angular-sanitize',
        'infinite-scroll': '../Scripts/ng-infinite-scroll',
        'bind-once': '../Scripts/bindonce',
        'jquery': '../Scripts/jquery-2.1.0',
        'boots': '../Scripts/ui-bootstrap-tpls-0.11.0'
        
    },
    shim: {
        'app': {
            deps: ['angular', 'angular-route','angular-sanitize', 'infinite-scroll', 'boots','bind-once']
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
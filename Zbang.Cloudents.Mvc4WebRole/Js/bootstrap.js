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
        'knockout': '../Scripts/knockout-3.0.0.debug',
        'utils': '../Js/Utils',
        'pubsub': '../Js/pubsub',        
    },
    shim: {
        'app': {
            deps: ['angular-route', 'angular-sanitize', 'infinite-scroll', 'boots', 'bind-once']
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
        'user-details': {
            deps: ['angular']
        },
        'knockout': {
            deps: ['jquery']
        },
        'utils': {
            deps: ['jquery']
        }
    }
});

require
(
    ['app','jquery','knockout','modernizr','utils','pubsub'],
    function (app, jQuery, ko) {
        window.ko = ko;
        angular.bootstrap(document, ['app']);
    }
);
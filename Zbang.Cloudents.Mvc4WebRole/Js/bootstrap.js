require.config({    
    paths: {
        'app': '{R_App}',
        'angular': '../Scripts/angular',
        'angular-route': '../Scripts/angular-route',
        'angular-sanitize': '../Scripts/angular-sanitize',
        'infinite-scroll': '../Scripts/ng-infinite-scroll',
        'bind-once': '../Scripts/bindonce',
        'jquery': '../Scripts/jquery-2.1.0',
        'boots': '../Scripts/ui-bootstrap-tpls-0.11.0',
        'modernizr': '../Scripts/Modernizr',
        'file-upload': '../Scripts/angular-file-upload',
        'Knockout': '../Scripts/knockout-3.0.0',


        'box': '{R_Dashboard-/Js/services/box}', // with box


        'dashboard':  '{R_Dashboard-/Js/services/dashboard}',
       
        'user': '{R_Dashboard-/Js/services/user}', // with user
        'newUpdates': '{R_Dashboard-/Js/services/newUpdates}', // with box
        'ngPlaceholder': '{R_Dashboard-/Js/directives/ngPlaceholder}', // with all
        'actionText': '{R_Dashboard-/Js/filters/actionText}',
        'orderby': '{R_Dashboard-/Js/filters/orderBy}',
        'CreateBoxCtrl': '{R_Dashboard-/Js/controllers/dashboard/createBoxCtrl}',
        'ShowFriendsCtrl': '{R_Dashboard-/Js/controllers/dashboard/showFriendsCtrl}',
        'DashboardCtrl': '{R_Dashboard-/Js/controllers/dashboard/dashboardCtrl}',

        //library
        'extScriptLdr': '{R_Library-/Scripts/externalScriptLoader}',
        'Utils': '{R_Library-/Js/Utils}',
        'Pubsub': '{R_Library-/Js/pubsub}',
        'DataContext': '{R_Library-/Js/DataContext}',
        'Dialog': '{R_Library-/Js/Dialog}',
        'GenericEvents': '{R_Library-/Js/GenericEvents}',
        //'library            '/Js/services/library.js',           
        'LibraryCtrl': '{R_Library-/Js/controllers/library/libraryCtrl}',
        'Library': '{R_Library-/Js/Library}',
        'Cache': '{R_Library-/Js/Cache}'

    },
    shim: {
        'app': {
            deps: ['angular-route', 'angular-sanitize', 'infinite-scroll', 'boots', 'bind-once', 'file-upload', 'modernizr']
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
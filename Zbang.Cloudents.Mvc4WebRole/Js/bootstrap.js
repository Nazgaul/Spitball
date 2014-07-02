require.config({    
    paths: {
        'app': '{R_All-/js/app}',
        'angular': '../Scripts/angular',
        'angular-route': '../Scripts/angular-route',
        'angular-sanitize': '../Scripts/angular-sanitize',
        'infinite-scroll': '../Scripts/ng-infinite-scroll',
        'bind-once': '../Scripts/bindonce',
        'jquery': '../Scripts/jquery-2.1.0',
        'boots': '../Scripts/ui-bootstrap-tpls-0.11.0',
        'modernizr': '../Scripts/Modernizr',       
        'Knockout': '../Scripts/knockout-3.0.0',
        'mLoader': '{R_All-/js/directives/mLoader}',

        'dropbox': '/js/services/dropbox',

        'routes': '{R_All-/js/routes}',
        'dependencyResolverFor': '{R_All-/js/services/dependencyResolverFor}',
        'ngPlaceholder': '{R_All-/js/directives/ngPlaceholder}', 

        'box': '{R_Dashboard-/js/services/box}', // with box


        'dashboard':  '{R_Dashboard-/js/services/dashboard}',
       
        'user': '{R_Dashboard-/js/services/user}', // with user
        'newUpdates': '{R_Dashboard-/js/services/newUpdates}', // with box
       
        'actionText': '{R_Dashboard-/js/filters/actionText}',
        'orderby': '{R_Dashboard-/js/filters/orderBy}',
        'CreateBoxCtrl': '{R_Dashboard-/js/controllers/dashboard/createBoxCtrl}',
        'ShowFriendsCtrl': '{R_Dashboard-/js/controllers/dashboard/showFriendsCtrl}',
        'DashboardCtrl': '{R_Dashboard-/js/controllers/dashboard/dashboardCtrl}',

        //library
        'extScriptLdr': '{R_Library-/Scripts/externalScriptLoader}',
        'Utils': '{R_Library-/js/Utils}',
        'Pubsub': '{R_Library-/js/pubsub}',
        'DataContext': '{R_Library-/js/DataContext}',
        'Dialog': '{R_Library-/js/Dialog}',
        'GenericEvents': '{R_Library-/js/GenericEvents}',
        //'library            '/js/services/library.js',           
        'LibraryCtrl': '{R_Library-/js/controllers/library/libraryCtrl}',
        'Library': '{R_Library-/js/Library}',
        'Cache': '{R_Library-/js/Cache}'

    },
    shim: {
        'app': {
            deps: ['angular-route', 'angular-sanitize', 'infinite-scroll', 'boots', 'bind-once', 'modernizr']
        },
        'mLoader': {
            deps : ['app']
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
        'mLoader': {
            deps: ['angular']
        }
    }
});

require
(
    ['app','mLoader'],
    function () {
        angular.bootstrap(document, ['app']);
    }
);
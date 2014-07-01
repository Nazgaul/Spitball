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



        'dashboard':  '{R_Dashboard-/Js/services/dashboard}',
        'box': '{R_Dashboard-/Js/services/box}',
        'user': '{R_Dashboard-/Js/services/user}',
        'newUpdates': '{R_Dashboard-/Js/services/newUpdates}',
        'ngPlaceholder': '{R_Dashboard-/Js/directives/ngPlaceholder}',
        'actionText': '{R_Dashboard-/Js/filters/actionText}',
        'orderby': '{R_Dashboard-/Js/filters/orderBy}',
        'createBoxCtrl': '{R_Dashboard-/Js/controllers/dashboard/createBoxCtrl}',
        'ShowFriendsCtrl': '{R_Dashboard-/Js/controllers/dashboard/ShowFriendsCtrl}',
        'DashboardCtrl': '{R_Dashboard-/Js/controllers/dashboard/DashboardCtrl}'
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
require.config({
    paths: {
        angular: 'https://ajax.googleapis.com/ajax/libs/angularjs/1.2.18/angular.min',
        'angular-route': 'https://ajax.googleapis.com/ajax/libs/angularjs/1.2.18/angular-route.min',
        'angular-sanitize': 'https://ajax.googleapis.com/ajax/libs/angularjs/1.2.18/angular-sanitize.min',
        app: '{R_All-/js/app}',
        'infinite-scroll': '/Scripts/ng-infinite-scroll',
        'bind-once': '/Scripts/bindonce',
        jquery: 'https://ajax.googleapis.com/ajax/libs/jquery/2.1.0/jquery.min',
        boots: '/Scripts/uiBootstrapTpls0.11.0',
        modernizr: '/Scripts/Modernizr',
        Knockout: '{R_OldAll-/Scripts/knockout-3.0.0}',

        'mLoader': '{R_All-/js/directives/mLoader}',

        dropbox: '{R_Box-/js/services/dropbox}', 
        'draganddrop': '{R_Box-/Scripts/draganddrop}', 
        'trustedHtml': '{R_Box-/js/filters/trustedHtml}', 

        'item': '{R_BoxItem-/js/services/item}',
        'quiz': '{R_BoxQuiz-/js/services/quiz}', 
        'qna': '{R_Box-/js/services/qna}', 
        'upload': '{R_Box-/js/services/upload}',

        'boxCtrl': '{R_Box-/js/controllers/box/boxCtrl}', 


        tabCtrl: '{R_Box-/js/controllers/box/tabCtrl}',
        qnaCtrl: '{R_Box-/js/controllers/box/qnaCtrl}',
        uploadCtrl: '{R_Box-/js/controllers/box/uploadCtrl}',
        manageCtrl: '{R_Box-/js/controllers/box/manageCtrl}',
        selectOnClick: '{R_Box-/js/directives/selectOnClick}',

        'routes': '{R_All-/js/routes}',
        'slimscroll': '{R_OldAll-/Scripts/jquery.slimscroll}',
        'dependencyResolverFor': '{R_All-/js/services/dependencyResolverFor}',
        'ngPlaceholder': '{R_All-/js/directives/ngPlaceholder}', 

        'box': '{R_DashboardBox-/js/services/box}', 


        'dashboard':  '{R_Dashboard-/js/services/dashboard}',

        'user': '{R_Dashboard-/js/services/user}', // dashboard user
        'newUpdates': '{R_DashboardBox-/js/services/newUpdates}', 

        'actionText': '{R_Dashboard-/js/filters/actionText}',
        'orderby': '{R_Dashboard-/js/filters/orderBy}',
        'CreateBoxCtrl': '{R_Dashboard-/js/controllers/dashboard/createBoxCtrl}',
        'ShowFriendsCtrl': '{R_Dashboard-/js/controllers/dashboard/showFriendsCtrl}',
        'DashboardCtrl': '{R_Dashboard-/js/controllers/dashboard/dashboardCtrl}',
        

        'extScriptLdr': '{R_OldAll-/Scripts/externalScriptLoader}',
        'Utils': '{R_OldAll-/js/Utils}',
        'Pubsub': '{R_OldAll-/js/pubsub}',
        'DataContext': '{R_OldAll-/js/DataContext}',
        'Dialog': '{R_OldAll-/js/Dialog}',
        'GenericEvents': '{R_OldAll-/js/GenericEvents}',
        'ElasticTextBox': '{R_OldAll-/Scripts/elasticTextBox}',
        'Cache': '{R_OldAll-/js/Cache}',

        //library
        'LibraryCtrl': '{R_Library-/js/controllers/library/libraryCtrl}',
        'Library': '{R_Library-/js/Library}',              

        //User
        'UserCtrl': '{R_User-/js/controllers/user/userCtrl}',
        'UserTemp': '{R_User-/js/User}',
        CountUp: '{R_User-/scripts/CountUp}',

        //Item         
        'ItemCtrl': '{R_Item-/js/controllers/item/itemCtrl}',
        'ItemViewModel': '{R_Item-/js/ItemViewModel4}',

        //quiz
        'QuizCtrl': '{R_Quiz-/js/controllers/quiz/quizCtrl}',
        'QuizViewModel': '{R_Quiz-/js/QuizViewModel}',
        'stopwatch': '{R_Quiz-/Scripts/stopwatch}'

    },
    shim: {
        'angular': {
            deps: ['jquery'],
            exports: 'angular'
        },
        'app': {
            deps: ['angular','angular-route', 'angular-sanitize', 'infinite-scroll', 'boots', 'bind-once', 'modernizr']
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
       
        'boots': {
            deps: ['angular']
        },
        'mLoader': {
            deps: ['angular', 'app']
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
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
        Knockout: '/Scripts/knockout-3.0.0',

        'mLoader': '{R_All-/js/directives/mLoader}',

        dropbox: '{R_Box-/js/services/dropbox}',
        googleDrive: '{R_Box-/js/services/googleDrive}',

        'draganddrop': '{R_Box-/Scripts/draganddrop}', 
        'trustedHtml': '{R_Box-/js/filters/trustedHtml}', 

        'item': '{R_BoxItem-/js/services/item}',
        'quiz': '{R_BoxQuiz-/js/services/quiz}', 
        'qna': '{R_Box-/js/services/qna}', 
        'upload': '{R_Box-/js/services/upload}',
        'facebook': '{R_Box-/js/services/facebook}',

        'boxCtrl': '{R_Box-/js/controllers/box/boxCtrl}', 
        
        'Plupload1': '/Scripts/plupload/plupload',
        'Plupload2': '/Scripts/plupload/plupload.html4',
        'Plupload3': '/Scripts/plupload/plupload.html5',
        'Plupload4': '/Scripts/plupload/plupload.flash',
        'Upload2': '/Js/Upload2',


        tabCtrl: '{R_Box-/js/controllers/box/tabCtrl}',
        qnaCtrl: '{R_Box-/js/controllers/box/qnaCtrl}',
        uploadCtrl: '{R_Box-/js/controllers/box/uploadCtrl}',
        manageCtrl: '{R_Box-/js/controllers/box/manageCtrl}',
        selectOnClick: '{R_Box-/js/directives/selectOnClick}',

        'routes': '{R_All-/js/routes}',
        'slimscroll': '/Scripts/jquery.slimscroll',
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
        

        'extScriptLdr': '/Scripts/externalScriptLoader',
        'Utils': '{R_OldAll-/js/Utils}',
        'Pubsub': '{R_OldAll-/js/pubsub}',
        'DataContext': '{R_OldAll-/js/DataContext}',
        'Dialog': '{R_OldAll-/js/Dialog}',
        'GenericEvents': '{R_OldAll-/js/GenericEvents}',
        'ElasticTextBox': '/Scripts/elasticTextBox',
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
            deps: ['angular', 'angular-route', 'angular-sanitize',
                'infinite-scroll', 'boots', 'bind-once', 'modernizr']
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
        },
        'Plupload2' : {
            deps: ['Plupload1']
        },
        'Plupload3': {
            deps: ['Plupload1']
        },
        'Plupload4': {
            deps: ['Plupload1']
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
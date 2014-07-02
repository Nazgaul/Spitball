define('routes', [], function () {
    return {
        defaultRoutePath: '/dashboard',
        routes: {
            '/dashboard/': {
                params: {
                    type: 'dashboard'
                },
                templateUrl: '/dashboard/',
                dependencies: [
                   "dashboard",
                    "box",
                    "user",
                    "newUpdates",
                    "ngPlaceholder",
                    "actionText",
                    "orderby",
                    "CreateBoxCtrl",
                    "ShowFriendsCtrl",
                    "DashboardCtrl"
                ]
            },
            '/box/my/:boxId/:boxName/': {
                params: {
                    type: 'box'
                },
                templateUrl: function (params) { return '/box/my/' + params.boxId + '/' + encodeURIComponent(params.boxName); },
                dependencies: [
                    'draganddrop',
                    'trustedHtml',
                    'box',
                    'item',
                    'quiz',
                    'qna',
                    'upload',
                    'newUpdates',
                    'ngPlaceholder',
                    'boxCtrl',
                    'tabCtrl',
                    'qnaCtrl',
                    'uploadCtrl',
                    'manageCtrl'
                ]
            },
            '/course/:uniName/:boxId/:boxName/': {
                params: {
                    type: 'box'
                },
                templateUrl: function (params) { return '/course/' + encodeURIComponent(params.uniName) + '/' + params.boxId + '/' + encodeURIComponent(params.boxName); },
                dependencies: [
                    'draganddrop',
                    'trustedHtml',
                    'box',
                    'item',
                    'quiz',
                    'qna',
                    'upload',
                    'newUpdates',
                    'ngPlaceholder',
                    'boxCtrl',
                    'tabCtrl',
                    'qnaCtrl',
                    'uploadCtrl',
                    'manageCtrl'
                ]
            },
            '/item/:uniName/:boxId/:boxName/:itemId/:itemName/': {
                params: {
                    type: 'item'
                },
                templateUrl: function (params) {
                    return '/item/' + encodeURIComponent(params.uniName) + '/' + params.boxId + '/'
                        + encodeURIComponent(params.boxName) + '/' + params.itemId + '/' + encodeURIComponent(params.itemName);
                },
                dependencies: [
                   'item',
                   'ngPlaceholder'
                   //'/Js/controllers/item/itemCtrl.js' - this is empty
                ]
            },
            '/quiz/:uniName/:boxId/:boxName/:quizId/:quizName/': {
                params: {
                    type: 'quiz'
                },
                templateUrl: function (params) {
                    return '/quiz/' + encodeURIComponent(params.uniName) + '/' + params.boxId + '/'
                        + encodeURIComponent(params.boxName) + '/' + params.quizId + '/' + encodeURIComponent(params.quizName);
                },
                dependencies: [
                   'quiz',
                   'ngPlaceholder'
                   //'/Js/controllers/quiz/quizCtrl.js'  - this is empty
                ]
            },
            '/library/:libraryId/:libraryName/': {
                params: {
                    type: 'library'
                },
                templateUrl: function (params) { return '/library/' + params.libraryId + '/' + encodeURIComponent(params.libraryName); },
                dependencies: [
                                'extScriptLdr',
                    'Dialog',
                    'Utils',
                    'Pubsub',
                    'DataContext',

                    'GenericEvents',
                    //'library            '/Js/services/library.js',           
                    'LibraryCtrl',
                    'Cache',
                    'ngPlaceholder',
                    'Library'

                ]
            },
            '/library/': {
                params: {
                    type: 'library'
                },
                templateUrl: '/library/',
                dependencies: [
                    'extScriptLdr',
                    'Dialog',
                    'Utils',
                    'Pubsub',
                    'DataContext',

                    'GenericEvents',
                    //'library            '/Js/services/library.js',           
                    'LibraryCtrl',
                    'Cache',
                    'ngPlaceholder',
                    'Library'

                ]
            },
            '/user/:userId/:userName': {
                params: {
                    type: 'user'
                },
                templateUrl: function (params) { return '/user/' + params.userId + '/' + params.userName; },
                dependencies: [
                    'user',
                    'ngPlaceholder'
                    //'/Js/controllers/library/userCtrl.js'
                ]
            }


        }
    };
});
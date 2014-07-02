define('/js/routes.js', [], function () {
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
                    '/Scripts/draganddrop.js',
                    '/Js/filters/trustedHtml.js',
                    'box',
                    '/Js/services/item.js',
                    '/Js/services/quiz.js',
                    '/Js/services/qna.js',
                    '/Js/services/upload.js',
                    'newUpdates',
                    'ngPlaceholder',
                    '/Js/controllers/box/boxCtrl.js',
                    '/Js/controllers/box/tabCtrl.js',
                    '/Js/controllers/box/qnaCtrl.js',
                    '/Js/controllers/box/uploadCtrl.js',
                    '/Js/controllers/box/manageCtrl.js'
                ]
            },
            '/course/:uniName/:boxId/:boxName/': {
                params: {
                    type: 'box'
                },
                templateUrl: function (params) { return '/course/' + encodeURIComponent(params.uniName) + '/' + params.boxId + '/' + encodeURIComponent(params.boxName); },
                dependencies: [
                    '/Scripts/draganddrop.js',
                    '/Js/filters/trustedHtml.js',
                    'box',
                    '/Js/services/item.js',
                    '/Js/services/quiz.js',
                    '/Js/services/qna.js',
                    '/Js/services/upload.js',
                    'newUpdates',
                    'ngPlaceholder',
                    '/Js/controllers/box/boxCtrl.js',
                    '/Js/controllers/box/tabCtrl.js',
                    '/Js/controllers/box/qnaCtrl.js',
                    '/Js/controllers/box/uploadCtrl.js',
                    '/Js/controllers/box/manageCtrl.js'
                ]
            },
            '/item/:uniName/:boxId/:boxName/:itemId/:itemName/': {
                params: {
                    type: 'item'
                },
                templateUrl: function (params) {
                    return '/item/' + encodeURIComponent(params.uniName) + '/' + params.boxId + '/'
                        + encodeURIComponent(params.boxName) + '/' + params.itemId + '/' + encodeURIComponent(params.itemName)
                },
                dependencies: [
                   '/Js/services/item.js',
                   'ngPlaceholder',
                   '/Js/controllers/item/itemCtrl.js'
                ]
            },
            '/quiz/:uniName/:boxId/:boxName/:quizId/:quizName/': {
                params: {
                    type: 'quiz'
                },
                templateUrl: function (params) {
                    return '/quiz/' + encodeURIComponent(params.uniName) + '/' + params.boxId + '/'
                        + encodeURIComponent(params.boxName) + '/' + params.quizId + '/' + encodeURIComponent(params.quizName)
                },
                dependencies: [
                   '/Js/services/quiz.js',
                   'ngPlaceholder',
                   '/Js/controllers/quiz/quizCtrl.js'
                ]
            },
            '/library/': {
                params: {
                    type: 'library'
                },
                templateUrl: '/library/',
                dependencies: [
                    'extScriptLdr',
                    'Utils',
                    'Pubsub',
                    'DataContext',
                    'Dialog',
                    'GenericEvents',
                    //'library            '/Js/services/library.js',           
                    'LibraryCtrl',
                    'Cache',
                    'ngPlaceholder',
                    'Library'

                ]
            },
            '/library/:libraryId/:libraryName/': {
                params: {
                    type: 'library'
                },
                templateUrl: function (params) { return '/library/' + params.libraryId },
                dependencies: [
                    '/Scripts/externalScriptLoader.js',
                    '/Js/DataContext.js',
                    '/Js/Dialog.js',
                    '/Js/GenericEvents.js',
                    '/Js/services/library.js',
                    'ngPlaceholder',
                    '/Js/controllers/library/libraryCtrl.js'
                ]
            },
            '/user/:userId/:userName': {
                params: {
                    type: 'user'
                },
                templateUrl: function (params) { return '/user/' + params.userId + '/' + params.userName },
                dependencies: [
                    'user',
                    'ngPlaceholder',
                    '/Js/controllers/library/userCtrl.js'
                ]
            }


        }
    };
});
define([], function () {
    return {
        defaultRoutePath: '/dashboard',
        routes: {
            '/dashboard/': {
                params: {
                    type: 'dashboard'
                },
                templateUrl: '/dashboard/',
                dependencies: [
                    '/Js/services/dashboard.js',
                    '/Js/services/box.js',
                    '/Js/services/user.js',
                    '/Js/services/newUpdates.js',
                    '/Js/filters/actionText.js',
                    '/Js/filters/orderBy.js',
                    '/Js/controllers/dashboard/createBoxCtrl.js',
                    '/Js/controllers/dashboard/showFriendsCtrl.js',
                    '/Js/controllers/dashboard/dashboardCtrl.js'
                ]
            },
            '/box/my/:boxId/:boxName/': {
                params: {
                    type: 'box'
                },
                templateUrl: function (params) { return '/box/my/' + +params.boxId + '/' + encodeURIComponent(params.boxName); },
                dependencies: [
                    '/Js/filters/trustedHtml.js',
                    '/Js/services/box.js',
                    '/Js/services/newUpdates.js',
                    '/Js/controllers/box/boxCtrl.js',
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
                    '/Js/filters/trustedHtml.js',
                    '/Js/services/box.js',
                    '/Js/services/newUpdates.js',
                    '/Js/controllers/box/boxCtrl.js',
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
                   '/Js/controllers/item/itemCtrl.js',
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
                   '/Js/controllers/quiz/quizCtrl.js',
                ]
            },
            '/library/': {
                params: {
                    type: 'quiz'
                },
                templateUrl: '/library/',
                deps: [
                    '/Js/services/library.js',
                    '/Js/controllers/library/libraryCtrl.js'
                ]
            },
            '/library/:libraryId/:libraryName/': {
                params: {
                    type: 'quiz'
                },
                templateUrl: function (params) { return '/library/' + params.libraryId },
                deps: [
                    '/Js/services/library.js',
                    '/Js/controllers/library/libraryCtrl.js'
                ]
            },
            '/user/:userId/:userName': {
                params: {
                    type: 'user'
                },
                templateUrl: function (params) { return '/user/' + params.userId + '/' + params.userName },
                deps: [
                    '/Js/services/user.js',
                    '/Js/controllers/library/userCtrl.js'
                ]
            }


        }
    };
});
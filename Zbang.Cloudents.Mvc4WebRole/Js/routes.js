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
                templateUrl: function (params) { return '/box/my/' + +params.boxId + '/' + params.boxName },
                dependencies: [
                    '/Js/filters/trustedHtml.js',
                    '/Js/services/box.js',
                    '/Js/services/newUpdates.js',
                    '/Js/controllers/box/boxCtrl.js'
                ]
            },
            '/course/:uniName/:boxId/:boxName/': {
                params: {
                    type: 'box'
                },
                templateUrl: function (params) { return '/course/' + params.uniName + '/' + +params.boxId + '/' + params.boxName },
                dependencies: [
                    '/Js/services/box.js',
                    '/Js/services/newUpdates.js',
                    '/Js/controllers/box/boxCtrl.js',
                    '/Js/controllers/box/qnaCtrl.js',
                    '/Js/controllers/box/uploadCtrl.js',
                    '/Js/controllers/box/manageCtrl.js'
                ]
            }

        }
    };
});
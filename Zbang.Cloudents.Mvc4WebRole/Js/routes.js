define([], function () {
    return {
        defaultRoutePath: '/dashboard/',
        routes: {
            '/dashboard/': {
                templateUrl: '/dashboard/',
                dependencies: [
                    '/Js/services/dashboard.js',
                    '/Js/services/box.js',
                    '/Js/services/user.js',
                    '/Js/services/newUpdates.js',
                    '/Js/filters/actionText.js',
                    '/Js/controllers/dashboard/createBoxCtrl.js',
                    '/Js/controllers/dashboard/showFriendsCtrl.js',
                    '/Js/controllers/dashboard/dashboardCtrl.js'
                ]
            }            
        }
    };
});
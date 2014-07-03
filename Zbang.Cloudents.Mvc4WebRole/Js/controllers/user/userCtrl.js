define('UserCtrl', ['app'], function (app) {
    app.controller('UserCtrl',
        ['$scope',
        function ($scope) {
            cd.pubsub.publish('initUser');
            cd.pubsub.publish('user');
            //todo proper return;
        }
        ]);
});
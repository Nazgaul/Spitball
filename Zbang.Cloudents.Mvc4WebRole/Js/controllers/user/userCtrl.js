define('UserCtrl', ['app'], function (app) {
    app.controller('UserCtrl',
        ['$scope',
        function ($scope) {
            cd.pubsub.publish('initUser');

            //todo proper return;
        }
        ]);
});
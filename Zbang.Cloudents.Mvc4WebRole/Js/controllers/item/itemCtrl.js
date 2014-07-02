define('ItemCtrl', ['app'], function (app) {
    app.controller('ItemCtrl',
        ['$scope',
        function ($scope) {
            cd.pubsub.publish('initItem');

            //todo proper return;
        }
        ]);
});
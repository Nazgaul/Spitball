define(['app'], function (app) {
    app.controller('LibraryCtrl',
        ['$scope',
        function ($scope) {
            cd.pubsub.publish('initLibrary');
        }
    ]);
});
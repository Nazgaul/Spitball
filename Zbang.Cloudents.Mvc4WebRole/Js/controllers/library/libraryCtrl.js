var mLibrary = angular.module('mLibrary', []);
mLibrary.controller('LibraryCtrl',
        ['$scope',
        function ($scope) {
            cd.pubsub.publish('initLibrary');

            //todo proper return;
        }
    ]);

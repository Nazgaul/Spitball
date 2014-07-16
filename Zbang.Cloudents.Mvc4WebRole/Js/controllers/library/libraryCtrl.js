var mLibrary = angular.module('mLibrary', []);
mLibrary.controller('LibraryCtrl',
        ['$scope',
        function ($scope) {
            cd.pubsub.publish('initLibrary');
            cd.pubsub.publish('lib_nodes');//statistics
            //todo proper return;
        }
    ]);

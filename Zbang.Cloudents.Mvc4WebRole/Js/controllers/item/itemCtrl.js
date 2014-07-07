var mItem = angular.module('mItem', []);
mItem.controller('ItemCtrl',
        ['$scope',
        function ($scope) {
            cd.pubsub.publish('initItem');

            //todo proper return;
        }
        ]);

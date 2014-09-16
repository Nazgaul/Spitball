var mItem = angular.module('mItem', []);
mItem.controller('ItemCtrl',
        ['$scope','$routeParams',
        function ($scope,$routeParams) {
            //cd.pubsub.publish('initItem');
            cd.pubsub.publish('item',$routeParams.itemId); //statistics
            //todo proper return;
        }
        ]);

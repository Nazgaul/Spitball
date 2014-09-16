var mItem = angular.module('mItem', []);
mItem.controller('ItemCtrl',
        ['$scope', '$routeParams', 'sItem',
        function ($scope, $routeParams, sItem) {
            cd.pubsub.publish('initItem');

            $scope.navigation = {};
            sItem.nav({ id: $routeParams.itemId, boxId: $routeParams.boxId }).then(
                function(response) {
                    var data = response.success ? response.payload : [];
                    $scope.navigation.previous = data.prev;
                    $scope.navigation.next = data.next;
                }
            );

          

            cd.pubsub.publish('item', $routeParams.itemId); //statistics
            //todo proper return;
        }
        ]);

mBox.controller('BoxInviteCtrl',
        ['$scope','$modalInstance', 'data',
        function ($scope,$modalInstance, data) {

            $scope.box = data;

            $scope.next = function () {
                $modalInstance.close();
            };

        }
]);
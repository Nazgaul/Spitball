
mAccount.controller('SpitballCtrl',
    ['$scope', '$modalInstance',
        function ($scope, $modalInstance) {
            "use strict";



            $scope.cancel = function () {
                $modalInstance.dismiss();

            };
        }

    ]);


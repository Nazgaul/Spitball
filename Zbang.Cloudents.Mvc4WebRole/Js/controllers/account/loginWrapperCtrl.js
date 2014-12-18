mAccount.controller('LoginWrapperCtrl',
    ['$scope', '$modalInstance', 'data',
        function ($scope, $modalInstance, data) {
            "use strict";

            $scope.close = function () {
                $modalInstance.dismiss();
            };

            $scope.data = {
                state: data.state,
                formData: data.formData
            };
        }]);

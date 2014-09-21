mItem.controller('itemFlagCtrl',
[
    '$scope', '$modalInstance', 'data', 'sItem',
function ($scope, $modalInstance, data, sItem) {

    $scope.secondStep = false;
    $scope.disable = false;
    $scope.formdata = {};
    $scope.formdata.id = data.id;

    $scope.flagItem = function () {
        $scope.disable = true;
        sItem.flag($scope.formdata).then(function (response) {
            if (!response.success) {
                alert(response.payload);
                return;
            }
            $scope.secondStep = true;
        });
       
    }
    $scope.cancel = function () {
        $modalInstance.dismiss();
    };
}
]);
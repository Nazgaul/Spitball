mItem.controller('itemRenameCtrl',
[
    '$scope', '$modalInstance','data','sItem',
function ($scope, $modalInstance, data, sItem) {
    $scope.formdata= {}
    $scope.formdata.newName = data.name;
    $scope.formdata.id = data.id;

    $scope.disable = false;

    $scope.renameItem = function () {
        console.log($scope.formdata);
        $scope.disable = true;

        sItem.rename($scope.formdata).then(function (response) {
            $scope.disable = false;
            if (!response.success) {
                alert(response.payload);
                return;
            }
            $modalInstance.close(response.payload.name);
        });
       
     };

    $scope.cancel = function () {
        $modalInstance.dismiss();
    };
}

]);
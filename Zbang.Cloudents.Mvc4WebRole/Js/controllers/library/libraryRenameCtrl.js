mLibrary.controller('libraryRenameCtrl',
['$scope', '$modalInstance', 'data','sUserDetails',
function ($scope, $modalInstance, data, sUserDetails) {
    $scope.formdata = {}
    $scope.formdata.newName = data.name;

    $scope.canDelete = sUserDetails.getDetails().isAdmin && data.canDelete;
    $scope.disable = function() {
        return data.name === $scope.formdata.newName;
    };

    $scope.disableDelete = false;

    $scope.renameDepartment = function () {
        $modalInstance.close($scope.formdata);
        //$scope.disable = true;

        //sItem.rename($scope.formdata).then(function (response) {
        //    $scope.disable = false;
        //    if (!response.success) {
        //        alert(response.payload);
        //        return;
        //    }
            
        //});
    };
    $scope.deleteDepartment = function() {
        $modalInstance.close('delete');
    };
    $scope.cancel = function () {
        $modalInstance.dismiss();
    };
}]);
libChoose.controller('restrictionPopUpCtrl',
[
    '$scope',
    '$modalInstance',
    'data',
    'sLibrary',
    function ($scope, $modalInstance, data, sLibrary) {
        //$scope.formData = {
        //    parentId: parentId
        //};
        $scope.formData = {
            UniversityId: data.university.id
        };

        $scope.create = function (isValid) {
            if (!isValid) {
                return;
            }
            console.log($scope.formData);
            sLibrary.updateUniversity($scope.formData).then(function (response) {
                if (!response.success) {
                    alert(response.payload[0].value[0]);
                    return;
                }
                
                 $modalInstance.close();
            });
            //    sBox.createAcademic($scope.formData).then(function (response) {
            //        if (!response.success) {
            //            alert(response.payload || response.Payload);
            //            return;
            //        }
            //        $modalInstance.close(response.payload || response.Payload);
            //    },
            //    function () {
            //        alert('error creating box');
            //    }
            //    );
        };

        $scope.cancel = function () {
            $modalInstance.dismiss();
        };
    }
]);

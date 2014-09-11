var mWizardBoxCreate = mDashboard.controller('CreateBoxWizardCtrl',
     ['$scope',  'sLibrary', 'sBox',  '$location',
         '$filter', 'debounce', '$modalInstance', 'sUserDetails',
    function ($scope,  sLibrary, sBox,  $location, $filter, debounce,
          $modalInstance, sUserDetails) {

        $scope.box = {
            // id: 0
        
        };
        console.log(sUserDetails.getDetails().department.id);
        $scope.formData = {
            academicBox: {
                departmentId: sUserDetails.getDetails().department.id
            }
        };
        //$scope.params = {
        //    changeDepartment: false,
            
        //};
        //$scope.isAcademicBox=true;

        $scope.display = { createDep: false };

        $scope.backCreateDepartment = function () {
            $scope.department = false;
            $scope.display = { createDep: false };
        };

        //$scope.createDepartmentSubmit = function () {
        //    $scope.display.createDep = false;
        //    $scope.$broadcast('newDep', {id :1 , name:'ram'});
        //}

        $scope.createDepartmentSubmit = function (isValid) {
            if (!isValid) {
                return;
            }
            //$scope.createDepartmentForm.$invalid = true;
            sLibrary.createDepartment($scope.formData.createDepartment).then(function (response) {
                if (response.success) {
                    $scope.display.createDep = false;
                    $scope.$broadcast('newDep', { id: response.payload.id, name: $scope.formData.createDepartment.name });
                }
            });
        };
       
        //$scope.createPrivateBox = function (isValid) {
        //    if (!isValid) {
        //        return;
        //    }
        //    sBox.createPrivate($scope.formData.privateBox).then(function () {

        //    });
        //};
        //$scope.createAcademiceBox = function (isValid) {
        //    if (!isValid) {
        //        return;
        //    }
        //    sBox.createAcademic($scope.formData.academicBox).then(function () {

        //    });
        //};

        $scope.completewizard = function() {
            //console.log($scope.box.id);
            $modalInstance.close($scope.box.url);
            //$rootScope.params.createBoxWizard = false;
        };
    }]
    );

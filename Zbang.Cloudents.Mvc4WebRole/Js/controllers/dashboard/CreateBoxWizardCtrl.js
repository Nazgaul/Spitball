var mWizardBoxCreate = mDashboard.controller('CreateBoxWizardCtrl',
     ['$scope',  'sLibrary', 'sBox',  '$location',
         '$filter', 'debounce', '$modalInstance',
    function ($scope,  sLibrary, sBox,  $location, $filter, debounce,
          $modalInstance) {

        $scope.box = {
           // id: 0
        }
        $scope.params = {
            changeDepartment: false,
            
        };
        $scope.isAcademicBox=true;

        $scope.changeDepartment = function () {
            $scope.params.changeDepartment = true;
            $scope.formData.departmentId = null;
            $scope.params.departmentSearch = null;
            $scope.selectedDepartment = null;
        };

        $scope.selectDepartment = function (deparment) {
            $scope.selectedDepartment = deparment;
            $scope.formData.departmentId = $scope.selectedDepartment.id;
            $scope.params.departmentSearch = $scope.selectedDepartment.name;
            $scope.departments = null;
            $scope.params.changeDepartment = false;
        };

        $scope.searchDepartment = debounce(function () {
            if (!$scope.params.departmentSearch) {
                $scope.departments = null;
                return;
            }

            sLibrary.items().then(function (response) {
                var data = response.success ? response.payload : {};
                var departments = data.nodes;
                $scope.departments = $filter('orderByFilter')(departments, { field: 'name', input: $scope.params.departmentSearch });
            });
        }, 200);

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

        $scope.completewizard = function () {
            //console.log($scope.box.id);
            $modalInstance.close($scope.box.url);
            //$rootScope.params.createBoxWizard = false;
        }
    }]
    );

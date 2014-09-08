mWizardBoxCreate.controller('createAcademicBoxCtrl',
        ['$scope',
         'sBox',
         'WizardHandler', 'debounce', 'sLibrary',
function ($scope, sBox, wizardHandler, debounce, sLibrary) {
    $scope.selectedDepartment = true;
    $scope.create = function (isValid) {

        //TODO: add disabled state
        if (!isValid) {
            return;
        }
        sBox.createAcademic($scope.formData.academicBox).then(function (response) {
            var data = response.success ? response.payload : [];
            $scope.box.url = data.url;
            wizardHandler.wizard().finish();
            //WizardHandler.wizard().next();
            //    //$modalInstance.close(box.payload || box.Payload);
        });
    };

    $scope.cancel = function () {
        // WizardHandler.wizard().finish();
        wizardHandler.wizard().finish();
    };
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

        sLibrary.searchDepartment({ term: $scope.params.departmentSearch }).then(function (response) {
            var data = response.success ? response.payload : {};
            var departments = data;
            $scope.departments = departments; //$filter('orderByFilter')(departments, { field: 'name', input: $scope.params.departmentSearch });
        });
    }, 200);
}
        ]);

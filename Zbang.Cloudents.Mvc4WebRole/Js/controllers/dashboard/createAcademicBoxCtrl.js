mWizardBoxCreate.controller('createAcademicBoxCtrl',
        ['$scope',
         'sBox','$filter',
         'WizardHandler', 'debounce', 'sLibrary','sUserDetails',
function ($scope, sBox,$filter, wizardHandler, debounce, sLibrary, sUserDetails) {
    $scope.selectedDepartment = true;
    var allDepartments;

    sLibrary.items().then(function (response) {
        var data = response.success ? response.payload : [];
        allDepartments = data.nodes;
        $scope.departments = allDepartments;
    });

    $scope.params.departmentSearch = sUserDetails.getDetails().department.name;

    $scope.create = function (isValid) {

        if (!isValid) {
            return;
        }
        $scope.formSubmit = true;
        sBox.createAcademic($scope.formData.academicBox).then(function (response) {
            $scope.formSubmit = false;
            var data = response.success ? response.payload : [];
            $scope.box.url = data.url;
            wizardHandler.wizard().finish();
            //WizardHandler.wizard().next();
        });
    };

    $scope.cancel = function () {
        // WizardHandler.wizard().finish();
        wizardHandler.wizard().finish();
    };

    $scope.$on('newDep',function(response,dep) {
        $scope.selectDepartment(dep);
    });

    $scope.changeDepartment = function () {
        $scope.params.changeDepartment = true;
        $scope.formData.departmentId = null;
        $scope.params.departmentSearch = null;
        $scope.selectedDepartment = null;
        $scope.departments = $filter('orderBy')(allDepartments, 'name');

    };

    $scope.selectDepartment = function (deparment) {
        $scope.selectedDepartment = deparment;
        $scope.formData.academicBox.departmentId = $scope.selectedDepartment.id;
        $scope.params.departmentSearch = $scope.selectedDepartment.name;
        $scope.departments = null;
        $scope.params.changeDepartment = false;
    };
    $scope.createDepartment = function() {
        $scope.display.createDep = true;
    };

    $scope.searchDepartment = debounce(function () {
        if (!$scope.params.departmentSearch) {
            $scope.departments = $filter('orderBy')(allDepartments, 'name');
            return;
        }

        if ($scope.selectedDepartment && $scope.params.departmentSearch !== $scope.selectedDepartment.name) {
            $scope.selectedDepartment = null;
        }

        if (allDepartments.length) {
            $scope.departments = $filter('orderByFilter')(allDepartments, { field: 'name', input: $scope.params.departmentSearch });
        }

    }, 200);
   
}
]);

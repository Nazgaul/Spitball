mWizardBoxCreate.controller('createAcademicBoxCtrl',
        ['$scope',
         'sBox','$filter',
        'debounce', 'sLibrary','sUserDetails',
function ($scope, sBox,$filter,debounce, sLibrary, sUserDetails) {
    $scope.selectedDepartment = true;



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
            $scope.box.id = data.id;
            $scope.next();
        });
    };

 
    var allDepartments;


    $scope.formData = {
        academicBox: {
            departmentId: sUserDetails.getDetails().department.id
        }
    };

    $scope.backCreateDepartment = function () {
        $scope.department = false;
        $scope.display = { createDep: false };
    };


    $scope.createDepartmentSubmit = function (isValid) {
        if (!isValid) {
            return;
        }

        sLibrary.createDepartment($scope.formData.createDepartment).then(function (response) {
            if (response.success) {
                $scope.display.createDep = false;
                $scope.selectDepartment({ id: response.payload.id, name: $scope.formData.createDepartment.name });                
            }
        });
    };

    $scope.selectDepartment = function (deparment) {
        $scope.selectedDepartment = deparment;
        $scope.formData.academicBox.departmentId = $scope.selectedDepartment.id;
        $scope.params.departmentSearch = $scope.selectedDepartment.name;
        $scope.departments = null;
        $scope.params.changeDepartment = false;

        sLibrary.chooseDeparment({ id: deparment.id }).then(function (response) {
            if (response.success) {
                sUserDetails.setDepartment($scope.selectedDepartment);           
            }
        });
    };

    $scope.createDepartment = function () {
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

    $scope.changeDepartment = function () {
        $scope.params.changeDepartment = true;
        $scope.formData.departmentId = null;
        $scope.params.departmentSearch = null;
        $scope.selectedDepartment = null;
        $scope.departments = $filter('orderBy')(allDepartments, 'name');

    };

   
}
]);

mDashboard.controller('CreateBoxWizardCtrl',
     ['$scope', 'sDashboard', 'sLibrary', 'sBox', 'sUser', '$location', '$filter', 'debounce',
    function ($scope, sDashboard, sLibrary, sBox, sUser, $location, $filter, debounce) {

        $scope.params = {
            changeDepartment: false
        };

        $scope.formData = {
            privacySettings: 'AnyoneWithUrl'
        };

        $scope.changeDepartment = function () {
            $scope.params.changeDepartment = true;
        };

        $scope.selectDepartment = function (department) {
            $scope.formData.department = department;
            $scope.params.changeDepartment = false;
        };

        $scope.searchDepartment = debounce(function () {
            if (!$scope.formData.department.name) {
                $scope.departments = null;
                return;
            }

            sLibrary.items().then(function (response) {
                var data = response.success ? response.payload : {};
                var departments = data.nodes;
                $scope.departments = $filter('orderByFilter')(departments, { field: 'name', input: $scope.formData.department.name });
            });
        }, 200);

        $scope.createPrivateBox = function (isValid) {
            if (!isValid) {
                return;
            }
            sBox.createPrivateBox($scope.formData.privateBox).then(function () {

            });
        };
        $scope.createAcademiceBox = function (isValid) {
            if (!isValid) {
                return;
            }
            sBox.createAcademic($scope.formData.academicBox).then(function () {

            });
        };
    }]
    );
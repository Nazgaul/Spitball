mDashboard.controller('CreateBoxWizardCtrl',
     ['$scope', 'sDashboard', 'sLibrary', 'sBox', 'sUser', '$location',
    function ($scope, sDashboard, sLibrary, sBox, sUser, $location) {

        $scope.params = {};

        $scope.formData = {
            privacySettings: 'AnyoneWithUrl'
        };

        $scope.changeDepartment = function () {
            $scope.params.changeDepartment = true;
        };

        $scope.selectDepartment = function () {

        };

        $scope.searchDepartment = function () {

        };

    }]
    );
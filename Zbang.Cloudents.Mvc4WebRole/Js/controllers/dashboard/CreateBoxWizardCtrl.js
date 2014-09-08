var mWizardBoxCreate = mDashboard.controller('CreateBoxWizardCtrl',
     ['$scope',  'sLibrary', 'sBox',  '$location',
         '$filter', 'debounce', '$modalInstance',
    function ($scope,  sLibrary, sBox,  $location, $filter, debounce,
          $modalInstance) {

        $scope.box = {
           // id: 0
        }
        //$scope.params = {
        //    changeDepartment: false,
            
        //};
        //$scope.isAcademicBox=true;

        //$scope.display = { createDep: true };

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

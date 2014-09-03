mDashboard.controller('CreateBoxCtrl',
        ['$scope',
         '$modalInstance',
         'sBox',

         function ($scope, $modalInstance, sBox) {
             $scope.formData = {
                 privacySettings: 'AnyoneWithUrl'
             };

             $scope.create = function (isValid) {
                 if (!isValid) {
                     return;
                 }

                 sBox.createPrivate($scope.formData).then(function (box) {
                     $modalInstance.close(box.payload || box.Payload);
                 });
             };

             $scope.cancel = function () {
                 $modalInstance.dismiss();
             };
         }
        ]);

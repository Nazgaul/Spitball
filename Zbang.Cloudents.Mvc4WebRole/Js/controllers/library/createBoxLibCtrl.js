/// <reference path="C:\Users\Guy\Source\Repos\Zbox\Zbang.Cloudents.Mvc4WebRole\Views/Library/_SelectUni.mobile.cshtml" />
mLibrary.controller('CreateBoxLibCtrl',
        ['$scope',
         '$modalInstance',
         'sBox',

         function ($scope, $modalInstance, Box) {
             $scope.formData = {
                 privacySettings: 'AnyoneWithUrl'
             };

             $scope.create = function (isValid) {
                 if (!isValid) {
                     return;
                 }

                 Box.create($scope.formData).then(function (box) {
                     $modalInstance.close(box.payload || box.Payload);
                 });
             };

             $scope.cancel = function () {
                 $modalInstance.dismiss();
             };
         }
        ]);

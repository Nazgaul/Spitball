define(['app'], function (app) {
    app.controller('CreateBoxCtrl',
        ['$scope',
         '$modalInstance',
         'Box',

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
});
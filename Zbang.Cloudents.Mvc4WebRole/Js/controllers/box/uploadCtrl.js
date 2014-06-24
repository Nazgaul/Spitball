define(['app'], function (app) {
    app.controller('UploadCtrl',
        ['$scope','Box',

         function ($scope,Box) {
             $scope.saveLink = function () {
             };

             $scope.saveDropbox = function () {
             };

             $scope.saveGoogleDrive = function () {
             };

             $scope.uploader = null;//upload;
         }
    ]);
});

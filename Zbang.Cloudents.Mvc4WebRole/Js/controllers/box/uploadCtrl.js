﻿define('uploadCtrl',['app','dropbox'], function (app,dropboxService) {
    app.controller('UploadCtrl',
        ['$scope', '$rootScope',          
         '$modalInstance', 'Dropbox',
          '$timeout',//temp
         //'googleDrive','dropbox',

        function ($scope, $rootScope, $modalInstance, Dropbox,$timeout) {
            $timeout(function () {                
                $rootScope.$broadcast('initUpload');
            });
            
            //function ($scope, Box, googleDrive, dropbox) {            
            $scope.saveLink = function () {
                $modalInstance.close({ url: true });
            };

            $scope.saveDropbox = function () {
                Dropbox.choose().then(function (files) {                   
                    $modalInstance.close({ dropbox: true, files: files });
                });
            };

            $scope.saveGoogleDrive = function () {
                $modalInstance.close();
            };

            $scope.cancel = function () {
                $modalInstance.dismiss();
            };

            //$rootScope.uploader.bind('afteraddingall', function (event, items) {
            //    $modalInstance.close();
            //});

            $scope.$on('BeforeUpload', function (event, data) {
                $modalInstance.dismiss();
            });

        }
    ]);
    app.controller('UploadLinkCtrl',
        ['$scope','$modalInstance',
         //'googleDrive','dropbox',

        function ($scope, $modalInstance) {
            $scope.formData = {};

            $scope.add = function (isValid) {
                $modalInstance.close($scope.formData.url);
            };

            $scope.cancel = function () {
                $modalInstance.dismiss();
            };
        }
    ]);
});

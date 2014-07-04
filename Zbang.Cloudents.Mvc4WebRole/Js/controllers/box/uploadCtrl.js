define('uploadCtrl', ['app', 'dropbox', 'googleDrive'], function (app, dropboxService) {
    app.controller('UploadCtrl',
        ['$scope', '$rootScope', '$modalInstance',
            'Dropbox', 'GoogleDrive', '$timeout',//temp
         //'googleDrive','dropbox',

        function ($scope, $rootScope, $modalInstance, Dropbox, GoogleDrive, $timeout) {
            $timeout(function () {
                $rootScope.$broadcast('initUpload');
            });

            $scope.sources = {
                dropboxLoaded: false,
                googleDriveLoaded: false
            }

            GoogleDrive.init().then(function () {
                $scope.sources.googleDriveLoaded = true;
            });

            Dropbox.init().then(function () {
                $scope.sources.dropboxLoaded = true;
            });

            $scope.saveLink = function () {
                $modalInstance.close({ url: true });
            };

            $scope.saveDropbox = function () {
                Dropbox.choose().then(function (files) {
                    $modalInstance.close({ dropbox: true, files: files });
                });
            };

            $scope.saveGoogleDrive = function () {
                GoogleDrive.picker().then(function (files) {
                    $modalInstance.close({ googleDrive: true, files: files });
                });
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
        ['$scope', '$modalInstance',
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

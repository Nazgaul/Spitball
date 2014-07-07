mBox.controller('UploadCtrl',
    ['$scope', '$rootScope', '$modalInstance',
        'sDropbox', 'sGoogle', '$timeout',//temp
     //'googleDrive','dropbox',

    function ($scope, $rootScope, $modalInstance, Dropbox, Google, $timeout) {
        $timeout(function () {
            cd.pubsub.publish('uploadAngular', function () {
                $rootScope.$broadcast('initUpload');
            });
        });

        $scope.sources = {
            dropboxLoaded: false,
            googleDriveLoaded: false
        }

        Google.initDrive().then(function () {
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
            Google.picker(true).then(function (files) { //isImmediate is true if it failes it will automatically try with false
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
mBox.controller('UploadLinkCtrl',
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

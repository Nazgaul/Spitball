(function () {
    angular.module('app.upload').controller('Upload', upload);
    upload.$inject = ['$scope', 'itemService', '$q', '$timeout', '$stateParams', '$rootScope', 'externalUploadProvider'];

    function upload($scope, itemService, $q, $timeout, $stateParams, $rootScope, externalUploadProvider) {
        var u = this;
        var boxid = $stateParams.boxId || $scope.$parent.cc.box.id;
        var cc = $scope.$parent.cc || {};

        var uploadChoose = {
            none: 0,
            computer: 1,
            drive: 2,
            dropbox: 3,
            link: 4
        };

        $scope.$on('open_upload', function () {
            externalUploadProvider.dropboxInit().then(function() {
                u.dropBoxLoaded = true;
            });
            externalUploadProvider.googleDriveInit().then(function() {
                u.googleDriveLoaded = true;
            });
           

        });
        $scope.$on('close_upload', function () {
            u.files = u.files.filter(function(file) {
                return !file.complete;
            });
            if (!u.files.length) {
                u.uploadStep = uploadChoose.none;
            }
        });
        //u.dropBoxLoaded = false;
        //u.googleDriveLoaded = false;

        u.google = function () {
            externalUploadProvider.google(boxid).then(function (response) {
                alert('done');
            });
        }

        u.dropBox = function () {
            externalUploadProvider.dropBox(boxid).then(function (response) {
                alert('done');
            });
        };

        u.uploadStep = uploadChoose.none;

        u.link = 'http://';
        u.alert = null;
        u.submitFormProcess = false;

        $scope.closeAlert = function () {
            u.alert = null;
        }
        //upload 
        u.uploadLink = function () {
            if (!u.link) {
                u.alert = 'not a valid url';
                return;
            }
            u.submitFormProcess = cc.submitFormProcess = true;
            itemService.addLink(u.link, boxid).then(function () {
                u.uploadStep = uploadChoose.none;
            }, function (response) {
                u.alert = response;
            }).finally(function () {
                u.submitFormProcess = cc.submitFormProcess = false;
            });

        }

        u.files = [];

        u.fileUpload = {
            url: '/upload/file/',
            options: {
                chunk_size: '3mb',
                drop_element: 'dropElement'
            },
            callbacks: {
                filesAdded: function (uploader, files) {

                    for (var i = 0; i < files.length; i++) {
                        files[i].sizeFormated = plupload.formatSize(files[i].size);
                        files[i].complete = false;
                        u.files.push(files[i]);
                    }
                    $timeout(function () {
                        uploader.start();
                    }, 1);
                },

                beforeUpload: function (up, file) {
                    up.settings.multipart_params = {
                        fileName: file.name,
                        fileSize: file.size,
                        boxId: boxid,
                        comment: false
                    };
                },
                fileUploaded: function (uploader, file, response) {
                    file.complete = true;
                    var obj = JSON.parse(response.response);
                    if (obj.success) {
                        $rootScope.$broadcast('item_upload', obj.payload);
                    }
                },
                error: function (uploader, error) {
                    u.alert = error.message;
                }
            }
        }
    }
})();
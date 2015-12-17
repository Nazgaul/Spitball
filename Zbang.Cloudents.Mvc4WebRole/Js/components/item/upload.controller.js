(function () {
    angular.module('app.upload').controller('Upload', upload);
    upload.$inject = ['$scope', 'itemService', '$q', '$timeout', '$stateParams', '$rootScope', 'externalUploadProvider', '$location', '$anchorScroll'];

    function upload($scope, itemService, $q, $timeout, $stateParams, $rootScope, externalUploadProvider, $location, $anchorScroll) {
        var u = this, tab = null, boxid = $stateParams.boxId;

        var uploadChoose = {
            none: 0,
            computer: 1,
            drive: 2,
            dropbox: 3,
            link: 4
        };

        $scope.$on('open_upload', function (e, args) {
            tab = args;
            boxid = $stateParams.boxId;
            $rootScope.$broadcast('close-collapse');
            $location.hash('upload');
            $anchorScroll();
            u.open = true;

            externalUploadProvider.dropboxInit().then(function () {
                u.dropBoxLoaded = true;
            });
            externalUploadProvider.googleDriveInit().then(function () {
                u.googleDriveLoaded = true;
            });


        });

        $scope.$on('close-collapse', function () {
            closeUpload();
        });

        u.closeUpload = closeUpload;
        //u.dropBoxLoaded = false;
        //u.googleDriveLoaded = false;

        u.google = google;
        u.dropBox = dropBox;
        u.uploadStep = uploadChoose.none;
        u.link = 'http://';
        u.submitFormProcess = false;
        u.uploadLink = uploadLink;

        function google() {
            externalUploadProvider.google(boxid).then(function (response) {
                $rootScope.$broadcast('item_upload', response);
            }, function (response) {
                $scope.app.showToaster(response, 'uploadSection');
            });
        }

        function dropBox() {
            externalUploadProvider.dropBox(boxid).then(function (response) {
                $rootScope.$broadcast('item_upload', response);
            }, function (response) {
                $scope.app.showToaster(response, 'uploadSection');
            });
        };

        function closeUpload() {
            u.open = false;
            u.files = u.files.filter(function (file) {
                return !file.complete;
            });
            if (!u.files.length) {
                u.uploadStep = uploadChoose.none;
            }
            $rootScope.$broadcast('close_upload');
        }



      
        //upload 

        function uploadLink(myform) {
            u.submitFormProcess = true;
            itemService.addLink(u.link, boxid).then(function (response) {
                $rootScope.$broadcast('item_upload', response);
                u.uploadStep = uploadChoose.none;
            }, function (response) {
                myform.link.$setValidity('server', false);
                u.error = response;
            }).finally(function () {
                u.submitFormProcess = false;
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
                        tabId: tab,
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
                    $scope.app.showToaster(error.message, 'upload');
                }
            }
        }
    }
})();
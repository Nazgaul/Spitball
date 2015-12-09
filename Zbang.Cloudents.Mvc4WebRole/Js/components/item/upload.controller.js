﻿(function () {
    angular.module('app.upload').controller('Upload', upload);
    upload.$inject = ['$scope', 'itemService', '$q', '$timeout', '$stateParams', '$rootScope', 'externalUploadProvider'];

    function upload($scope, itemService, $q, $timeout, $stateParams, $rootScope, externalUploadProvider) {
        var u = this, tab = null;

        var uploadChoose = {
            none: 0,
            computer: 1,
            drive: 2,
            dropbox: 3,
            link: 4
        };

        $scope.$on('open_upload', function (e, args) {
            tab = args;
            $rootScope.$broadcast('close-collapse');
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
        u.alert = null;
        u.submitFormProcess = false;
        u.uploadLink = uploadLink;

        function google() {
            externalUploadProvider.google($stateParams.boxId).then(function (response) {
                $rootScope.$broadcast('item_upload', response);
            }, function (response) {
                u.alert = response;
            });
        }

        function dropBox() {
            externalUploadProvider.dropBox($stateParams.boxId).then(function (response) {
                $rootScope.$broadcast('item_upload', response);
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



        $scope.closeAlert = function () {
            u.alert = null;
        }
        //upload 

        function uploadLink() {
            if (!u.link) {
                u.alert = 'not a valid url';
                return;
            }
            u.submitFormProcess = true;
            itemService.addLink(u.link, $stateParams.boxId).then(function (response) {
                $rootScope.$broadcast('item_upload', response);
                u.uploadStep = uploadChoose.none;
            }, function (response) {
                u.alert = response;
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
                        boxId: $stateParams.boxId,
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
                    u.alert = error.message;
                }
            }
        }
    }
})();
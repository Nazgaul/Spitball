//(function() {
//    angular.module("app", ["angular-plupload", "ang-drag-drop"]);
//})();
(function () {
    'use strict';
    angular.module('app.upload').controller('Upload', upload);
    upload.$inject = ['$scope', 'itemService', '$timeout', '$stateParams', '$rootScope',
        'externalUploadProvider', '$anchorScroll',
        'boxService',  'resManager'];

    function upload($scope, itemService, $timeout, $stateParams, $rootScope, externalUploadProvider,
        $anchorScroll, boxService, resManager) {
        var u = this, tab = $stateParams.tabId, boxid = $stateParams.boxId;

        var uploadChoose = {
            none: 0,
            computer: 1,
            drive: 2,
            dropbox: 3,
            link: 4
        };

        $scope.$on('open_upload', function () {
            tab = $stateParams.tabId;
            boxid = $stateParams.boxId;
            
            $anchorScroll.yOffset = 100;
            $timeout(function () {
                $anchorScroll('upload');
            });
           // u.open = true;

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

        u.google = google;
        u.dropBox = dropBox;
        u.uploadStep = uploadChoose.none;
        u.submitFormProcess = false;
        u.uploadLink = uploadLink;
        //u.uploadCollapsed = uploadCollapsed;
        //u.uploadOpen = uploadOpen;

        function google() {
            externalUploadProvider.google(boxid, tab).then(externalUploadCallback, function (response) {
                $scope.app.showToaster(response, 'uploadSection');
            });
        }

        function dropBox() {
            externalUploadProvider.dropBox(boxid, tab).then(externalUploadCallback, function (response) {
                $scope.app.showToaster(response, 'uploadSection');
            });
        };
        function externalUploadCallback(response) {
            $rootScope.$broadcast('item_upload', response);
            $scope.app.showToaster(resManager.get('toasterUploadComplete'));
            $timeout(closeUpload, 2000);
        }

        function closeUpload() {
            //u.open = false;
            $rootScope.$broadcast('close_upload');
        }


        $scope.$on("uploadCollapsed",
            function() {
                u.files = u.files.filter(function(file) {
                    return !file.complete;
                });
                if (!u.files.length) {
                    u.files = [];
                    u.filesCompleteCount = 0;
                    u.filesErrorCount = 0;
                    u.uploadStep = uploadChoose.none;
                }
            });
        


        //upload 

        function uploadLink(myform) {
            u.submitFormProcess = true;
            itemService.addLink(u.link, boxid, tab).then(function (response) {
                $rootScope.$broadcast('item_upload', response);
                $scope.app.showToaster(resManager.get('toasterUploadComplete'));
                u.uploadStep = uploadChoose.none;
                u.link = '';
                $timeout(closeUpload, 2000);
            }, function (response) {
                myform.link.$setValidity('server', false);
                u.error = response;
            }).finally(function () {
                u.submitFormProcess = false;
            });

        }

        function removeFile(file, uploader) {
            //var file = this;
            if (file.status === plupload.UPLOADING) {
                uploader.stop();
            }
            uploader.removeFile(file);

            if (uploader.total.queued > 0) {
                $timeout(function () {
                    uploader.start();
                });
            }
            if (file.systemId) {
                boxService.deleteItem(file.systemId);
                $rootScope.$broadcast('item_delete', file.systemId);

            }

            var index = u.files.indexOf(file);
            u.files.splice(index, 1);

            u.filesCompleteCount = u.files.filter(function (f) {
                return f.complete;
            }).length;
            u.filesErrorCount = u.files.filter(function (f) {
                return f.error;
            }).length;
        }

        u.files = [];
        u.filesCompleteCount = 0;
        u.filesErrorCount = 0;

        u.fileUpload = {
            url: '/upload/file/',
            options: {
                chunk_size: '3mb',
                drop_element: 'dropElement'
            },
            callbacks: {
                filesAdded: function (uploader, files) {

                    for (var i = 0; i < files.length; i++) {
                        var file = files[i];
                        (function (file) {
                            file.sizeFormated = plupload.formatSize(file.size);
                            file.boxId = boxid;
                            file.tabId = tab;
                            file.complete = false;
                            file.remove = function () { removeFile(file, uploader); }


                            var img = new mOxie.Image();

                            img.onload = function () {
                                this.crop(95, 105, false);
                                file.content = this.getAsDataURL("image/jpeg", 80);
                            };

                            img.onembedded = function () {
                                this.destroy();
                            };

                            img.onerror = function () {
                                this.destroy();
                            };

                            img.load(file.getSource());
                        })(file);
                        u.files.push(file);

                    }
                    $timeout(function () {
                        uploader.start();
                    }, 1);
                },

                beforeUpload: function (up, file) {
                    up.settings.multipart_params = {
                        fileName: file.name,
                        fileSize: file.size,
                        boxId: file.boxId,
                        tabId: file.tabId,
                        comment: false
                    };
                },
                fileUploaded: function (uploader, file, response) {
                    //cacheFactory.clearAll();
                    file.complete = true;
                    var obj = JSON.parse(response.response);
                    if (obj.success) {
                        u.filesCompleteCount++;
                        file.systemId = obj.payload.item.id;
                        $rootScope.$broadcast('item_upload', obj.payload);
                    }
                },
                uploadComplete: function () {
                    //toasterUploadComplete
                    $scope.app.showToaster(resManager.get('toasterUploadComplete'));
                    $timeout(closeUpload, 2000);
                },
                error: function (uploader, error) {
                    error.file.error = true;
                    u.filesErrorCount++;
                }
            }
        }
    }
})();


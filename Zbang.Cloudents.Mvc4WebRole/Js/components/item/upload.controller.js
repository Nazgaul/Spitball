(function () {
    angular.module('app.upload').controller('Upload', upload);
    upload.$inject = ['$scope', 'itemService', '$q', '$timeout', '$stateParams', '$rootScope',
        'externalUploadProvider', '$location', '$anchorScroll', 'boxService', 'CacheFactory'];

    function upload($scope, itemService, $q, $timeout, $stateParams, $rootScope, externalUploadProvider, $location, $anchorScroll, boxService, cacheFactory) {
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
                u.files = [];
                u.filesCompleteCount = 0;
                u.filesErrorCount = 0;
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
                boxService.deleteItem(file.systemId, boxid);
                $rootScope.$broadcast('item_delete', file.systemId);
                
            }
            
            var index = u.files.indexOf(file);
            u.files.splice(index, 1);

            u.filesCompleteCount = u.files.filter(function(f) {
                return f.complete;
            }).length;
            u.filesErrorCount = u.files.filter(function (f) {
                return f.error;
            }).length;
            // u.filesCompleteCount = 0;
            // u.filesErrorCount = 0;

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
                        file.sizeFormated = plupload.formatSize(file.size);
                        file.boxId = boxid;
                        file.tabId = tab;
                        file.complete = false;
                        file.remove = function () { removeFile(file, uploader); }
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
                    cacheFactory.clearAll();
                    file.complete = true;
                    var obj = JSON.parse(response.response);
                    if (obj.success) {
                        u.filesCompleteCount++;
                        file.systemId = obj.payload.item.id;
                        $rootScope.$broadcast('item_upload', obj.payload);
                    }
                },
                error: function (uploader, error) {
                    error.file.error = true;
                    u.filesErrorCount++;
                }
            }
        }
    }
})();
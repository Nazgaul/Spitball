app.controller('UploadListCtrl',
    ['$scope', '$rootScope', '$timeout', '$interval',

    function ($scope, $rootScope, $timeout, $interval) {

        init();

        var params = {
            progressMaxWidth: 75,
            title: {
                uploaded: 'Uploaded Files',
                uploadingFiles: 'Uploading Files',
                uploading: 'Uploading '
            }
        };

        //#region plupload 

        $scope.$on('FilesAdded', function (e, files) {

            if (!$scope.uploader.init) { //shows window if closed
                $scope.uploader.init = true;
            }

            _.forEach(files, function (file) {
                file.progressWidth = 0;
                file.plupload = true;
                $scope.uploader.fileList.push(file);
            });

            if (!$scope.uploader.collapsed) { //if view is collapsed we show uploading if not setting the currentFile will show the title 
                $scope.uploader.title = params.title.uploadingFiles;
            } else {
                $scope.uploader.title = params.title.uploading;
            }

            $scope.uploader.currentFile = _.find($scope.uploader.fileList, function (file) {
                return !(file.error || file.uploaded);
            });

            $scope.collapsed = false;
            $scope.uploader.uploading = true;


        });

        $scope.$on('UploadProgress', function (e, file) {
            var uFile = getFile(file.id);

            if (!uFile) {
                return;
            }

            uFile.progressWidth = params.progressMaxWidth * (uFile.percent / 100);
        });

        $scope.$on('UploadFileError', function (e, file) {
            var uFile = getFile(file.id);
            uFile.error = true;

        });

        $scope.$on('FileUploaded', function (e, file) {
            var uFile = getFile(file.id);

            if (!uFile) {
                return;
            }

            uFile.uploaded = true;

            if (finishedUploading()) {
                $scope.uploader.uploading = false;
                $scope.uploader.title = params.title.uploaded;
                $scope.uploader.currentFile = null;
            }
        });

        $scope.cancelUpload = function (file) {

            if (file.status === plupload.UPLOADING) {
                file.uploader.stop();
            }
            file.uploader.removeFile(file);

            if (file.uploader.total.queued > 0) {
                $timeout(function () {
                    file.uploader.start();
                });
            } else {
                file.uploader.trigger('UploadComplete');
            }



            var index = $scope.uploader.fileList.indexOf(file);
            $scope.uploader.fileList.splice(index, 1);

            if ($scope.uploader.fileList.length === 0) {
                $scope.uploader.title = params.title.uploaded;
                $scope.uploader.uploading = false;
            }
        };



        //#endregion

        //#region links
        $scope.$on('LinkAdded', function (e, link) {
            if (!$scope.uploader.init) { //shows window if closed
                $scope.uploader.init = true;
            }

            $scope.uploader.fileList.push(link);

            if (!$scope.uploader.collapsed) { //if view is collapsed we show uploading if not setting the currentFile will show the title 
                $scope.uploader.title = params.title.uploadingFiles;
            } else {
                $scope.uploader.title = params.title.uploading;
            }


            $scope.uploader.currentFile = _.find($scope.uploader.fileList, function (file) {
                return !(file.error || file.uploaded);
            });

            link.percent = 50;
            link.progressWidth = params.progressMaxWidth * (link.percent / 100);

            $scope.uploader.uploading = true;


        });

        $scope.$on('LinkUploaded', function (e, link) {
            var uLink = getFile(link.id);
            if (!uLink) {
                return;
            }

            uLink.uploaded = true;
            uLink.percent = 100;
            uLink.progressWidth = params.progressMaxWidth;
            $interval.cancel(uLink.interval);

            if (finishedUploading()) {
                $scope.uploader.uploading = false;
                $scope.uploader.title = params.title.uploaded;
                $scope.uploader.currentFile = null;
            }
        });

        $scope.$on('UploadLinkError', function (e, link) {
            var uLink = getFile(link.id);
            uLink.error = true;

        });




        //#endregion


        //#region dropbox

        $scope.$on('DropboxAdded', function (e, db) {
            if (!$scope.uploader.init) { //shows window if closed
                $scope.uploader.init = true;
            }


            $scope.uploader.fileList.push(db);


            if (!$scope.uploader.collapsed) { //if view is collapsed we show uploading if not setting the currentFile will show the title 
                $scope.uploader.title = params.title.uploadingFiles;
            } else {
                $scope.uploader.title = params.title.uploading;
            }


            $scope.uploader.currentFile = _.find($scope.uploader.fileList, function (file) {
                return !(file.error || file.uploaded);
            });


            db.percent = 50;
            db.progressWidth = params.progressMaxWidth * (db.percent / 100);

            $scope.uploader.uploading = true;
        });

        $scope.$on('DropboxUploaded', function (e, db) {
            var uDb = getFile(db.id);
            if (!uDb) {
                return;
            }

            uDb.uploaded = true;
            uDb.percent = 100;
            uDb.progressWidth = params.progressMaxWidth;
            $interval.cancel(uDb.interval);

            if (finishedUploading()) {
                $scope.uploader.uploading = false;
                $scope.uploader.title = params.title.uploaded;
                $scope.uploader.currentFile = null;
            }
        });

        $scope.$on('UploadDropboxError', function (e, db) {
            var uDb = getFile(db.id);
            uDb.error = true;

        });

        //#endregion


        $scope.toggleCollapse = function () {

            if (!$scope.uploader.fileList.length) {
                $scope.uploader.collapsed = false;
                return;
            }

            $scope.uploader.collapsed = !$scope.uploader.collapsed;

            if (!$scope.uploader.uploading) {
                return;
            }

            if ($scope.uploader.collapsed) {
                $scope.uploader.title = params.title.uploading;
                return;
            }

            $scope.uploader.title = params.title.uploadingFiles;

        };

        $scope.closeView = function () {
            init();
        };

        function finishedUploading() {
            var files = $scope.uploader.fileList.filter(function (file) {
                return file.error || file.uploaded;
            });

            return files.length === $scope.uploader.fileList.length;
        }

        function getFile(fileId) {
            var file = _.find($scope.uploader.fileList, function (file) {
                return file.id === fileId;
            });

            return file;
        }

        function init() {
            $scope.uploader = {
                collapsed: false,
                currentFile: null,
                fileList: [],
                uploading: false,
                title: null,
                init: false
            };
        }


        //@*data-finish="@DialogResources.UploadedFiles" data-maintitle="@DialogResources.UploadingFiles" data-minimizetile="@DialogResources.Uploading"*@
    }]
);
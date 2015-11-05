(function () {
    angular.module('app.upload').controller('Upload', upload);
    upload.$inject = ['$scope', 'itemService', 'dropboxService', '$q', 'googleService', '$timeout', '$stateParams', '$rootScope'];

    function upload($scope, itemService, dropboxService, $q, googleService, $timeout, $stateParams, $rootScope) {
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

        $scope.$on('uploadPhase', function () {
            dropboxService.init().then(function () {
                u.dropBoxLoaded = true;
            });
            $q.all([googleService.initDrive(), googleService.initGApi()]).then(function () {
                u.googleDriveLoaded = true;
            });

        });
        u.dropBoxLoaded = false;
        u.googleDriveLoaded = false;

        u.google = function () {
            googleService.picker().then(function (response) {
                var filesUpload = [];
                for (var i = 0; i < response.length; i++) {
                    filesUpload.push(itemService.addLink(response[i].link, boxid, null, null, response[i].name));
                }
                $q.all(filesUpload).then(function () {
                    alert('done');
                });
            });
        }

        u.dropBox = function () {

            dropboxService.choose().then(function (response) {
                var filesUpload = [];
                for (var i = 0; i < response.length; i++) {
                    filesUpload.push(itemService.addFromDropBox(boxid, response[i].link, response[i].name));
                }
                $q.all(filesUpload).then(function () {
                    alert('done');
                });
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
                        //isComment: false

                    };


                    //var data = {};
                    //if (file.newQuestion) {
                    //    data.newQuestion = true;
                    //}
                    //if (file.questionId) {
                    //    data.questionId = file.questionId;
                    //}

                    //$rootScope.$broadcast('BeforeUpload', data);

                },
                //uploadProgress: function (uploader, file) {
                //    console.log(file);
                //},
                fileUploaded: function (uploader, file, response) {
                    file.complete = true;
                    // $scope.loading = false;
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
(function () {
    angular.module('app.upload').controller('Upload', upload);
    upload.$inject = ['$scope', 'itemService', 'dropboxService', '$q', 'googleService', '$timeout'];

    function upload($scope, itemService, dropboxService, $q, googleService, $timeout) {
        var u = this;
        var cc = $scope.$parent.cc;

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
                    filesUpload.push(itemService.addLink(response[i].link, cc.box.id, null, null, response[i].name));
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
                    filesUpload.push(itemService.addFromDropBox(cc.box.id, response[i].link, response[i].name));
                }
                $q.all(filesUpload).then(function () {
                    alert('done');
                });
            });


        };

        u.uploadStep = uploadChoose.none;

        u.link = 'http://';
        //upload 
        u.uploadLink = function () {
            if (!u.link) {
                cc.alert = 'not a valid url';
                return;
            }
            cc.submitFormProcess = true;
            itemService.addLink(u.link, cc.box.id).then(function () {
                u.uploadStep = uploadChoose.none;
            }, function (response) {
                cc.alert = response;
            }).finally(function () {
                cc.submitFormProcess = false;
            });

        }

        u.fileUpload = {
            url: '/upload/file/',
            options: {
                chunk_size: '3mb',
                drop_element: 'dropElement'
            },
            callbacks: {
                filesAdded: function (uploader, files) {
                    $timeout(function () {
                        uploader.start();
                    }, 1);
                },

                beforeUpload: function (up, file) {
                    up.settings.multipart_params = {
                        fileName: file.name,
                        fileSize: file.size,
                        boxId: cc.box.id,
                        question: false,
                        isComment: false

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
                //    $scope.loading = file.percent / 100.0;
                //},
                fileUploaded: function (uploader, file, response) {
                    // $scope.loading = false;
                    var obj = JSON.parse(response.response);
                    if (obj.success) {
                        //self.data.image = obj.payload;
                        //userDetailsService.changeImage(obj.payload);
                    }
                },
                error: function (uploader, error) {
                    //$scope.loading = false;
                    alert(error.message);
                }
            }
        }
    }
})();
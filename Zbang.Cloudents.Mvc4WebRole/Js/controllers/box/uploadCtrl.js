mBox.controller('UploadCtrl',
    ['$scope', '$rootScope', '$q', '$modal', '$modalInstance', 'sFacebook', '$filter',
        'sDropbox', 'sGoogle', 'sUpload', 'data',

    function ($scope, $rootScope, $q, $modal, $modalInstance, sFacebook, $filter, Dropbox, Google, sUpload, data) {
        var jsResources = window.JsResources;
        $scope.boxId = data.boxId;
        $scope.tabId = data.tabId;
        $scope.questionId = data.questionId;
        $scope.newQuestion = data.newQuestion;

        $scope.sources = {
            dropboxLoaded: false,
            googleDriveLoaded: false
        }


        var drivePromise = Google.initDrive(),
            initGApiPromise = Google.initGApi();
        var all = $q.all([drivePromise, initGApiPromise]);
        all.then(function () {

            Google.checkAuth(true).then(function () {
                $scope.sources.googleDriveLoaded = true;

            }, function () {
                $scope.sources.googleDriveLoaded = true;

            });
        });



        Dropbox.init().then(function () {
            $scope.sources.dropboxLoaded = true;
        });

        $scope.saveLink = function () {
            $modalInstance.close();

            var modalInstance = $modal.open({
                windowClass: "uploadLink",
                templateUrl: '/Box/UploadLinkPartial/',
                controller: 'UploadLinkCtrl',
                backdrop: 'static'
            });

            modalInstance.result.then(function (url) {

                var data = {
                    id: guid(),
                    name: url,
                    fileUrl: url,
                    boxId: $scope.boxId,
                    tabId: $scope.tabId
                };

                cd.pubsub.publish('addPoints', { type: 'itemUpload', amount: 1 });

                sUpload.link(data).then(function (response) {
                    if (!response.success) {
                        $rootScope.$broadcast('UploadLinkError', data);
                        alert('error');
                        return;
                    }

                    $rootScope.$broadcast('LinkUploaded', data);


                    var sentObj = {
                        itemDto: response.payload,
                        boxId: data.boxId,
                        tabId: data.tabId,
                        questionId: $scope.questionId,
                        newQuestion : $scope.newQuestion
                    }


                    $rootScope.$broadcast('ItemUploaded', sentObj);
                }, function () {
                    $rootScope.$broadcast('UploadLinkError', data);
                });

                data.size = 1024;
                $rootScope.$broadcast('LinkAdded', data);

            });
        };

        $scope.saveDropbox = function () {

            Dropbox.choose().then(function (files) {

                _.forEach(files, function (file) {
                    (function (fileData) {

                        var data = {
                            id: guid(),
                            name: fileData.name,
                            fileUrl: fileData.link,
                            boxId: $scope.boxId,
                            tabId: $scope.tabId

                        };
                        sUpload.dropbox(data).then(function (response) {
                            if (!response.success) {
                                $rootScope.$broadcast('UploadDropboxError', data);
                                alert('error');

                                return;
                            }
                            $rootScope.$broadcast('DropboxUploaded', data);

                            var sentObj = {
                                itemDto: response.payload,
                                boxId: data.boxId,
                                tabId: data.tabId,
                                questionId: $scope.questionId,
                                newQuestion: $scope.newQuestion
                            }

                            $rootScope.$broadcast('ItemUploaded', sentObj);
                        }, function () {
                            $rootScope.$broadcast('UploadDropboxError', data);
                        });

                        data.size = fileData.bytes;
                        $rootScope.$broadcast('DropboxAdded', data);

                    })(file);
                });


                cd.pubsub.publish('addPoints', { type: 'itemUpload', amount: files.length });


                $modalInstance.close();

            });

        };

        $scope.saveGoogleDrive = function () {
            if (!Google.isAuthenticated()) {
                Google.checkAuth(false).then(function () {
                    loadPicker();
                });
                return;
            }
            loadPicker();

            function loadPicker() {
                Google.picker().then(function (files) { //isImmediate is true if it failes it will automatically try with false
                    _.forEach(files, function (file) {
                        (function (fileData) {

                            var data = {
                                id: guid(),
                                name: fileData.name,
                                fileUrl: fileData.link,
                                boxId: $scope.boxId,
                                tabId: $scope.tabId
                            };
                            sUpload.link(data).then(function (response) {
                                if (!response.success) {
                                    $rootScope.$broadcast('UploadLinkError', data);
                                    alert('error');
                                    return;
                                }


                                $rootScope.$broadcast('LinkUploaded', data);

                                var sentObj = {
                                    itemDto: response.payload,
                                    boxId: data.boxId,
                                    tabId: data.tabId,
                                    questionId: $scope.questionId,
                                    newQuestion: $scope.newQuestion
                                }
                                
                                $rootScope.$broadcast('ItemUploaded', sentObj);


                            }, function () {
                                $rootScope.$broadcast('UploadLinkError', data);
                            });

                            data.size = fileData.size;
                            $rootScope.$broadcast('LinkAdded', data);
                        })(file);
                    });

                    cd.pubsub.publish('addPoints', { type: 'itemUpload', amount: files.length });
                });
            }
            $modalInstance.close();
        };

        $scope.cancel = function () {
            $modalInstance.dismiss();
        };

        $scope.$on('BeforeUpload', function (event, data) {
            $modalInstance.dismiss();
        });



        function guid() {
            var guid = (G() + G() + "-" + G() + "-" + G() + "-" +
            G() + "-" + G() + G() + G()).toUpperCase();

            function G() {
                return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1)
            }


            return guid;
        }
    }
    ]);
mBox.controller('UploadLinkCtrl',
    ['$scope', '$modalInstance',

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
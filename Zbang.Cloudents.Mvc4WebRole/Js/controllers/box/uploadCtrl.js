app.controller('UploadCtrl',
    ['$scope', '$rootScope', '$q', 'sModal', 'sFacebook', '$filter',
        'sDropbox', 'sGoogle', 'sUpload', '$timeout','$analytics','sNotify','sGmfcnHandler',

    function ($scope, $rootScope, $q, sModal, sFacebook, $filter, sDropbox, sGoogle, sUpload, $timeout,
        $analytics, sNotify, sGmfnHandler) {
        "use strict";
        $scope.sources = {
            dropboxLoaded: false,
            googleDriveLoaded: false
        }


        var drivePromise = sGoogle.initDrive(),
            initGApiPromise = sGoogle.initGApi();
        var all = $q.all([drivePromise, initGApiPromise]);
        all.then(function () {

            sGoogle.checkAuth(true).then(function () {
                $scope.sources.googleDriveLoaded = true;

            }, function () {
                $scope.sources.googleDriveLoaded = true;

            });
        });



        sDropbox.init().then(function () {
            $scope.sources.dropboxLoaded = true;
        });

        $timeout(function () {
            $rootScope.$broadcast('PluploadRefresh');
        });

        $scope.saveLink = function () {

            $analytics.eventTrack( 'Save Link', {
                category: 'Upload'
            });

            if ($scope.close) {//fix for step 3
                $scope.close();
            }

            sModal.open('uploadLink', {
                callback: {
                    close: function (url) {
                        var data = {
                            id: guid(),
                            name: url,
                            fileUrl: url,
                            boxId: $scope.boxId || $scope.box.id, //fix for step 3
                            tabId: $scope.tabId,
                            question: $scope.newQuestion
                        };
                        sUpload.link(data).then(function (response) {
                            
                            $rootScope.$broadcast('LinkUploaded', data);


                            var sentObj = {
                                itemDto: response,
                                boxId: data.boxId,
                                tabId: data.tabId,
                                questionId: $scope.questionId,
                                newQuestion: $scope.newQuestion
                            }
                            
                            sGmfnHandler.addPoints({ type: 'itemUpload', amount: 1 });


                            $rootScope.$broadcast('ItemUploaded', sentObj);
                        }, function (response) {                           
                            $rootScope.$broadcast('UploadLinkError', data);
                            sNotify.alert(response);
                        });

                        data.size = 1024;
                        $rootScope.$broadcast('LinkAdded', data);
                        if ($scope.completeWizard) {//fix for step 3
                            $scope.completeWizard(true);
                        }
                    }
                }
            });          
        };

        $scope.saveDropbox = function () {
            $analytics.eventTrack('Save Dropbox', {
                category: 'Upload'
            });

            sDropbox.choose().then(function (files) {

                _.forEach(files, function (file) {
                    (function (fileData) {

                        var data = {
                            id: guid(),
                            name: fileData.name,
                            fileUrl: fileData.link,
                            boxId: $scope.boxId || $scope.box.id, //fix for step 3
                            tabId: $scope.tabId,
                            question: $scope.newQuestion

                        };
                        sUpload.dropbox(data).then(function (response) {
                            
                            $rootScope.$broadcast('DropboxUploaded', data);

                            var sentObj = {
                                itemDto: response,
                                boxId: data.boxId,
                                tabId: data.tabId,
                                questionId: $scope.questionId,
                                newQuestion: $scope.newQuestion
                            }

                            if (_.last(files) === fileData) {
                                sGmfnHandler.addPoints({ type: 'itemUpload', amount: files.length });
                                
                            }

                            $rootScope.$broadcast('ItemUploaded', sentObj);
                        }, function () {
                            $rootScope.$broadcast('UploadDropboxError', data);
                        });

                        data.size = fileData.bytes;
                        $rootScope.$broadcast('DropboxAdded', data);

                    })(file);
                });
                if ($scope.close) {//fix for step 3
                    $scope.close();
                } else {
                    $scope.completeWizard(true);
                }
            });

        };

        $scope.saveGoogleDrive = function () {
            if (!sGoogle.isAuthenticated()) {
                sGoogle.checkAuth(false).then(function () {
                    loadPicker();
                });
                return;
            }
            loadPicker();
            $analytics.eventTrack('Save Drive', {
                category: 'Upload'
            });
            function loadPicker() {
                sGoogle.picker().then(function (files) { //isImmediate is true if it failes it will automatically try with false
                    _.forEach(files, function (file) {
                        (function (fileData) {

                            var data = {
                                id: guid(),
                                name: fileData.name,
                                fileUrl: fileData.link,
                                boxId: $scope.boxId || $scope.box.id, //fix for step 3
                                tabId: $scope.tabId
                            };
                            sUpload.link(data).then(function (response) {
                                $rootScope.$broadcast('LinkUploaded', data);

                                var sentObj = {
                                    itemDto: response,
                                    boxId: data.boxId,
                                    tabId: data.tabId,
                                    questionId: $scope.questionId,
                                    newQuestion: $scope.newQuestion
                                }


                                if (_.last(files) === fileData) {
                                    sGmfnHandler.addPoints({ type: 'itemUpload', amount: files.length });
                                }

                                $rootScope.$broadcast('ItemUploaded', sentObj);


                            }, function () {
                                $rootScope.$broadcast('UploadLinkError', data);
                            });

                            data.size = fileData.size;
                            $rootScope.$broadcast('LinkAdded', data);




                        })(file);
                    });
                });
            }
            if ($scope.close) {//fix for step 3
                $scope.close();
            } else {
                $scope.completeWizard(true);
            }
        };

        $scope.cancel = function () {
            $scope.dismiss();
        };

        $scope.$on('BeforeUpload', function () {
            if ($scope.dismiss) { //fix for step 3
                $scope.dismiss();
            } else {
                $scope.completeWizard(true);
            }
        });



        function guid() {
            var guid2 = (G() + G() + "-" + G() + "-" + G() + "-" +
            G() + "-" + G() + G() + G()).toUpperCase();

            function G() {
                return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
            }


            return guid2;
        }
    }
    ]);
mBox.controller('UploadLinkCtrl',
    ['$scope', '$modalInstance', '$analytics',

    function ($scope, $modalInstance, $analytics) {
        "use strict";

        $scope.formData = {};

        $scope.add = function () {
            $modalInstance.close($scope.formData.url);

            $analytics.eventTrack('Link', {
                category: 'Saved Link'
            });
        };

        $scope.cancel = function () {
            $modalInstance.dismiss();
            $analytics.eventTrack('Link', {
                category: 'Cancel'
            });
        };
    }
    ]);
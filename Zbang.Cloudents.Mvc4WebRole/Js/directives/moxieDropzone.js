mDashboard.directive('plDropzoneUploader',
    ['$analytics',function ($analytics) {
        "use strict";
        return {
            restrict: 'A',
            controller: ['$scope', '$rootScope', 'sUserDetails', 'sNotify', '$angularCacheFactory', 'sGmfcnHandler', function ($scope, $rootScope, sUserDetails, sNotify, $angularCacheFactory, sGmfcnHandler) {
                var uploader,
                    options = {
                        runtimes: 'html5,flash',
                        multi_selection: true,
                        browse_button: 'dropzoneBtn',
                        chunk_size: '3mb',
                        url: '/Upload/File/',
                        flash_swf_url: '/Scripts/plupload2/Moxie.swf',
                        headers: {
                            'X-Requested-With': 'XMLHttpRequest'
                        }
                    };

                this.addFileBox = function (boxId, files) {
                    $scope.currentBoxId = boxId;
                    uploader.addFile(files);
                };
                this.addFileFeedQuestion = function (boxId, files) {
                    $scope.newQuestion = true;
                    this.addFileBox(boxId, files);
                };
                this.addFileFeedReply = function (boxId, questionId, files) {
                    $scope.questionId = questionId;
                    this.addFileBox(boxId, files);
                };

                this.init = function (container) {
                    options.container = container;
                    init();
                }

                function init() {
                    uploader = new plupload.Uploader(options);
                    uploader.init();
                    uploader.disableBrowse();

                    uploader.bind('Error', function (up, err) {
                        sNotify.alert("Cannot upload, error: " + err.message + (err.file ? ", File: " + err.file.name : "") + "");
                        if ($rootScope.$$phase) {
                            $rootScope.$broadcast('UploadFileError', err.file);
                        } else {
                            $rootScope.$apply(function () {
                                $rootScope.$broadcast('UploadFileError', err.file);
                            });
                        }
                    });

                    uploader.bind('FilesAdded', function (up, files) {

                        _.forEach(files, function (file) {
                            file.uploader = up;
                            file.newQuestion = $scope.newQuestion,
                            file.questionId = $scope.questionId
                        });

                        $scope.newQuestion = $scope.questionId = null;

                        if ($rootScope.$$phase) {
                            $rootScope.$broadcast('FilesAdded', files);
                        }
                        else {
                            $rootScope.$apply(function () {
                                $rootScope.$broadcast('FilesAdded', files);
                            });

                        }
                        uploader.start();
                    });


                    uploader.bind('BeforeUpload', function (up, file) {
                        up.settings.multipart_params = {
                            fileName: file.name,
                            fileSize: file.size,
                            boxId: $scope.currentBoxId
                        };

                        $rootScope.$broadcast('BeforeUpload');

                    });

                    uploader.bind('FileUploaded', function (up, file, res) {
                        var response = JSON.parse(res.response);

                        $angularCacheFactory.clearAll();

                        if ($rootScope.$$phase) {
                            post();
                            return;
                        }

                        $rootScope.$apply(function () {
                            post();
                        });
                        function post() {
                            $rootScope.$broadcast('FileUploaded', file);
                            response.payload.itemDto = response.payload.fileDto;
                            if (!response.success) {
                                sNotify.alert(response.payload);
                                return;
                            }



                            response.payload.questionId = file.questionId;
                            response.payload.newQuestion = file.newQuestion;
                            $rootScope.$broadcast('ItemUploaded', response.payload);
                        }

                    });

                    uploader.bind('UploadProgress', function (up, file) {

                        if ($rootScope.$$phase) {
                            $rootScope.$broadcast('UploadProgress', file);
                            return;
                        }

                        $rootScope.$apply(function () {
                            $rootScope.$broadcast('UploadProgress', file);
                        });

                    });

                    uploader.bind('UploadComplete', function (up, files) {

                        if (files && files.length > 0) {
                            sGmfcnHandler.addPoints({ type: 'itemUpload', amount: files.length });
                        }

                        up.files = [];
                        up.splice();

                    });
                }
            }],
            link: function (scope, elem, attrs, controller) {

                controller.init(attrs.plDropzoneUploader);
                var $main = angular.element('#main');
                if (!attrs.dropElement) {
                    $main.on('dragenter', toggle);
                    $main.on('dragleave', toggle);
                    $main.on('drop', toggleOff);
                    return;
                }
                $main.on('dragenter', '[dropzone-element]', toggle);
                $main.on('dragleave', '[dropzone-element]', toggle);
                $main.on('drop', '[dropzone-element]', toggleOff);


                function toggle() {
                    $(this).toggleClass('upload');
                    if ($(this).hasClass('upload')) {
                        $analytics.trackEvent('Drag Enter', {
                            category: attrs.plDropzoneUploader
                        });

                    }
                    return;

                    $analytics.trackEvent('Drag Leave', {
                        category: attrs.plDropzoneUploader
                    });
                }
                function toggleOff() {
                    $(this).removeClass('upload');

                    $analytics.trackEvent('Drag Drop', {
                        category: attrs.plDropzoneUploader
                    });
                }

                scope.$on('$destroy', function () {
                    $main.off('dragenter', toggle);
                    $main.off('dragleave', toggle);
                    $main.off('drop', toggleOff);
                });
            }
        };
    }]).
    directive('plDropzone',
    [
    function () {
        "use strict";
        return {
            restrict: "A",
            require: '^plDropzoneUploader',
            //scope:false,
            link: function (scope, elem, attrs, controller) {
                var dropzone = new mOxie.FileDrop({
                    drop_zone: elem[0]
                });


                dropzone.ondrop = function (event) {
                    if (attrs.newQuestion === 'true') { //feed
                        controller.addFileFeedQuestion(parseInt(attrs.boxId), dropzone.files);
                        return;
                    }
                    if (attrs.questionId && attrs.questionId.length) { //feed
                        controller.addFileFeedReply(parseInt(attrs.boxId), attrs.questionId, dropzone.files);
                        return;
                    }
                    controller.addFileBox(scope.box.id, dropzone.files); //dashboard
                };

                dropzone.init();
            }
        };
    }
    ]);

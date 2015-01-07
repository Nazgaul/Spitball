(function (angular) {
    "use strict";
    angular.module('plupload', []).
        directive('plUploader', function () {
            return {
                restrict: 'A',
                controller: ['$rootScope', '$stateParams', '$angularCacheFactory', function ($rootScope, $stateParams, $angularCacheFactory) {
                    var uploader,
                        plUpload = this,
                        boxId = $stateParams.boxId;

                    init();

                    plUpload.addFiles = function (files) {
                        angular.forEach(files, function (file) {
                            uploader.addFile(file);
                        });
                    };

                    function init() {
                        var options = {
                            runtimes: 'html5',
                            multi_selection: true,
                            browse_button: 'plFakeBtn',
                            chunk_size: '3mb',
                            url: '/Upload/File/',
                            headers: {
                                'X-Requested-With': 'XMLHttpRequest'
                            }
                        };

                        uploader = new plupload.Uploader(options);
                        uploader.init();
                        uploader.disableBrowse();

                        uploader.bind('Error', uploadError);
                        uploader.bind('FilesAdded', filesAdded);
                        uploader.bind('BeforeUpload', beforeUpload);
                        uploader.bind('UploadComplete', uploadComplete);
                        boxId = boxId;
                    };


                    function filesAdded(up, files) {
                        up.start();
                        $rootScope.$apply(function () {
                            $rootScope.$broadcast('uploadStart');
                        });
                    }

                    function beforeUpload(up, file) {
                        up.settings.multipart_params = {
                            fileName: file.name,
                            fileSize: file.size,
                            boxId: boxId
                        };
                    }

                    function uploadComplete(up, file, res) {
                        $angularCacheFactory.clearAll();
                        $rootScope.$apply(function () {
                            $rootScope.$broadcast('uploadComplete');
                        });

                    }

                    function uploadError(up, err) {
                        alert("Cannot upload, error: " + err.message + (err.file ? ", File: " + err.file.name : "") + "");
                    }

                }]
            };
        }).
    directive('plFileinput', function () {
        return {
            restrict: 'A',
            require: '^plUploader',
            link: function (scope, element, attrs, controller) {
                var fileinput = new mOxie.FileInput({
                    browse_button: element[0],
                    multiple: true
                });


                fileinput.onchange = function (event) {
                    controller.addFiles(fileinput.files); //dashboard
                };

                fileinput.init();
            }
        };
    });
})(angular)
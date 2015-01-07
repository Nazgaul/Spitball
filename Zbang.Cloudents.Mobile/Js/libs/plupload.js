(function (angular) {
    "use strict";
    angular.module('plupload', []).
        directive('plUploader', function () {
            return {
                restrict: 'A',
                controller: ['$scope', '$stateParams', function ($scope, $stateParams) {
                    var uploader,
                        plUpload = this,
                        boxId = $stateParams.boxId;

                    init();

                    plUpload.addFiles = function (files) {
                        uploader.addFile(files);
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
                        uploader.bind('UploadComplete', beforeUpload);
                        boxId = boxId;
                    };


                    function filesAdded(up, files) {
                        up.start();
                    }

                    function beforeUpload(up, files) {
                        up.settings.multipart_params = {
                            boxId: boxId
                        };
                    }

                    function uploadComplete(up, file, res) {
                        $scope.$emit('uploadComplete');
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
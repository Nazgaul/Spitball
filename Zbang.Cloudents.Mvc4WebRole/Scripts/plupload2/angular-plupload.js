'use strict';

angular.module('angular-plupload', [])
	.directive('plUpload', ['$rootScope', '$timeout', 'sUserDetails', '$angularCacheFactory', function ($rootScope, $timeout, sUserDetails, $angularCacheFactory) {
	    return {
	        restrict: 'A',
	        scope: {},
	        link: function (scope, iElement, iAttrs) {
	            var uploader;

	            scope.randomString = function (len, charSet) {
	                charSet = charSet || 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
	                var randomString = '';
	                for (var i = 0; i < len; i++) {
	                    var randomPoz = Math.floor(Math.random() * charSet.length);
	                    randomString += charSet.substring(randomPoz, randomPoz + 1);
	                }
	                return randomString;
	            }


	            var randomValue = scope.randomString(5),
                    dropElement = document.getElementById(iAttrs.dropArea);
	            iAttrs.$set('id', randomValue);

	            var options = {
	                runtimes: 'html5,flash',
	                browse_button: iElement[0],
	                drop_element: dropElement,
	                multi_selection: iAttrs.multiSelection !== 'false',
	                chunk_size: iAttrs.chunk || '3mb',
	                container: 'main',
	                url: iAttrs.url || '/Upload/File/',
	                flash_swf_url: '/Scripts/plupload2/Moxie.swf',
	                headers: {
	                    'X-Requested-With': 'XMLHttpRequest'
	                }
	            }

	            //if (iAttrs.dropArea) {
	            //    options.browse_button = null;
	            //    options.drop_element = document.getElementById(iAttrs.dropArea);
	            //}

	            uploader = new plupload.Uploader(options);
	            uploader.init();


	            uploader.bind('Error', function (up, err) {

	                alert("Cannot upload, error: " + err.message + (err.file ? ", File: " + err.file.name : "") + "");

	                $rootScope.$apply(function () {
	                    $rootScope.$broadcast('UploadFileError', err.file);
	                });


	                up.refresh(); // Reposition Flash/Silverlight
	            });

	            uploader.bind('FilesAdded', function (up, files) {

	                if (!sUserDetails.isAuthenticated()) {
	                    cd.pubsub.publish('register', { action: true });
	                    return;
	                }

	                _.forEach(files, function (file) {
	                    file.boxId = iAttrs.boxId;
	                    file.tabId = iAttrs.tabId || null;
	                    file.questionId = iAttrs.questionId || null;
	                    file.newQuestion = (iAttrs.newQuestion === 'true');
	                    file.fileName = file.name;
	                    file.uploader = up;
	                });

	                $rootScope.$apply(function () {
	                    $rootScope.$broadcast('FilesAdded', files);
	                });

	                uploader.start();
	            });

	            uploader.bind('BeforeUpload', function (up, file) {
	                up.settings.multipart_params = {
	                    fileName: file.name,
	                    fileSize: file.size,
	                    boxId: iAttrs.boxId,
	                    tabId: iAttrs.tabId || null
	                };

	                $rootScope.$broadcast('BeforeUpload');

	            });

	            uploader.bind('FileUploaded', function (up, file, res) {
	                var response = JSON.parse(res.response);

	                $angularCacheFactory.clearAll();

	                if (scope.$$phase) {
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
	                        alert(response.payload);
	                        return;
	                    }

	                    response.payload.tabId = file.tabId;
	                    response.payload.questionId = file.questionId;
	                    response.payload.newQuestion = file.newQuestion;
	                    $rootScope.$broadcast('ItemUploaded', response.payload);
	                }

	            });

	            uploader.bind('UploadProgress', function (up, file) {

	                if (scope.$$phase) {
	                    $rootScope.$broadcast('UploadProgress', file);
	                    return;
	                }

	                $rootScope.$apply(function () {
	                    $rootScope.$broadcast('UploadProgress', file);
	                });

	            });

	            uploader.bind('UploadComplete', function (up, files) {

	                if (files && files.length > 0) {
	                    cd.pubsub.publish('addPoints', { type: 'itemUpload', amount: files.length });
	                }


	                up.files = [];
	                up.splice();

	                if (iAttrs.destroy) {
	                    up.destroy();
	                }

	            });

	            scope.$on('$destroy', function () {
	                uploader.disableBrowse();
	            });


	            //scope.$on('$locationChangeStart', function (event) {
	            //    if (uploader.runtime !== 'flash') {
	            //        return;
	            //    }
	            //    var isOk = confirm('Leaving page will stop the file upload, are you sure you want to leave?');
	            //    if (!isOk) {
	            //        event.preventDefault();
	            //    }
	            //});

	            //uploader.bind('PostInit', function (event) {
	            //    if (uploader.runtime !== 'flash') {
	            //        return;
	            //    }

	            //    uploader.refresh();
	            //});

	        }
	    };
	}]).
    directive('plUploadStandalone', ['$timeout', function ($timeout) {
        return {
            restrict: 'A',
            scope: {
                onError: '&',
                onFilesAdded: '&',
                onUploaded: '&'

            },
            link: function (scope, element, attrs) {
                var options = {
                    runtimes: 'html5,flash',
                    browse_button: element[0],
                    multi_selection: false,
                    chunk_size: '3mb',
                    container: attrs.container,
                    url: attrs.url,
                    flash_swf_url: '/Scripts/plupload2/Moxie.swf',
                    filters: [
                        { title: "Image files", extensions: "jpg,gif,png" }
                    ],
                    headers: {
                        'X-Requested-With': 'XMLHttpRequest'
                    }
                }


                var uploader = new plupload.Uploader(options);
                uploader.init();


                uploader.bind('Error', function (up, err) {
                    if (scope.$$phase) {
                        scope.onError({ error: err });
                        return;
                    }
                    scope.$apply(function () {
                        scope.onError({ error: err });
                    });
                   
                    up.refresh(); // Reposition Flash/Silverlight
                });

                uploader.bind('FilesAdded', function (up, files) {

                    if (scope.$$phase) {
                        scope.onFilesAdded({ files: files });
                        return;
                    }

                    scope.$apply(function () {
                        scope.onFilesAdded({ files: files });
                    });
                    
                    uploader.start();
                    uploader.disableBrowse();
                });

                uploader.bind('FileUploaded', function (up, file, res) {
                    var response = JSON.parse(res.response);

                    $angularCacheFactory.clearAll();

                    if (scope.$$phase) {
                        scope.onUploaded({ response: response });
                        return;
                    }

                    scope.$apply(function () {
                        scope.onUploaded({ response: response });
                    });

                    uploader.disableBrowse(false);
                });

                scope.$on('$destroy', function () {
                    uploader.destroy();
                });


                scope.$on('$locationChangeStart', function (event) {
                    if (uploader.state === plupload.STOPPED) {
                        return;
                    }
                    var isOk = confirm('Leaving page will stop the file upload, are you sure you want to leave?');
                    if (!isOk) {
                        event.preventDefault();
                    }
                });


            }
        };

    }]);
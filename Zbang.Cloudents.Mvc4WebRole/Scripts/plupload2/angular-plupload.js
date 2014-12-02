angular.module('angular-plupload', [])
	.directive('plUpload', ['$rootScope', '$timeout', 'sUserDetails', '$angularCacheFactory', 'sNotify', 'sLogin', 'sGmfcnHandler', '$analytics', function ($rootScope, $timeout, sUserDetails, $angularCacheFactory, sNotify, sLogin, sGmfcnHandler, $analytics) {
	    "use strict";
	    return {
	        restrict: 'A',
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
	            dropElement.addEventListener('dragenter', function (e) {
	                    if (e.dataTransfer.types.indexOf('Files') === -1) {
	                        return;
	                    }
	                    $analytics.eventTrack('Drag Enter', {
	                        category: iAttrs.dropArea
	                    });	                
	            });


	            document.addEventListener('dragleave', function (e) {
	                $analytics.eventTrack('Drag Leave', {
	                    category: iAttrs.dropArea
	                });
	            });

	            document.addEventListener('drop', function (e) {
	                $analytics.eventTrack('Drop', {
	                    category: iAttrs.dropArea
	                });
	            });

	            uploader = new plupload.Uploader(options);
	            uploader.init();


	            uploader.bind('Error', function (up, err) {

	                sNotify.alert("Cannot upload, error: " + err.message + (err.file ? ", File: " + err.file.name : "") + "");


	                if ($rootScope.$$phase) {
	                    $rootScope.$broadcast('UploadFileError', err.file);
	                } else {
	                    $rootScope.$apply(function () {
	                        $rootScope.$broadcast('UploadFileError', err.file);
	                    });
	                }

	                up.refresh(); // Reposition Flash/Silverlight
	            });

	            uploader.bind('FilesAdded', function (up, files) {

	                if (!sUserDetails.isAuthenticated()) {
	                    sLogin.registerAction();
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
	                    boxId: iAttrs.boxId,
	                    tabId: iAttrs.tabId || null,
                        question: iAttrs.newQuestion === 'true'
	                };

	                $rootScope.$broadcast('BeforeUpload');

	            });

	            uploader.bind('FileUploaded', function (up, file, res) {
	                var response = JSON.parse(res.response);

	                $angularCacheFactory.clearAll();

	                if (uploader.total.uploaded > 0 && uploader.total.queued === 0) {
	                    sGmfcnHandler.addPoints({ type: 'itemUpload', amount: uploader.total.uploaded });

	                    up.files = [];
	                    up.splice();

	                    if (iAttrs.destroy) {
	                        up.destroy();
	                    }

	                }

	                if ($rootScope.$$phase) {
	                    if (!response.success) {
	                        uploader.trigger('Error', { file: file, message: response.payload });
	                        return;
	                    }
	                    post();
	                    return;
	                }

	                $rootScope.$apply(function () {
	                    if (!response.success) {
	                        uploader.trigger('Error', { file: file, message: response.payload });
	                        return;
	                    }
	                    post();
	                });
	                function post() {
	                    $rootScope.$broadcast('FileUploaded', file);
	                    response.payload.itemDto = response.payload.fileDto;
	                    if (!response.success) {
	                        sNotify.alert(response.payload);
	                        return;
	                    }

	                    response.payload.tabId = file.tabId;
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

	            //uploader.bind('UploadComplete', function (up, files) {

	            //});

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
    directive('plUploadStandalone', ['$timeout', '$rootScope', '$angularCacheFactory', 'sNotify', function ($timeout, $rootScope, $angularCacheFactor, sNotify) {
        "use strict";
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
                    if ($rootScope.$$phase) {
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

                    if ($rootScope.$$phase) {
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


                scope.$on('$routeChangeStart', function (event) {
                    if (uploader.state === plupload.STOPPED) {
                        return;
                    }
                    var isOk = sNotify.confirm('Leaving page will stop the file upload, are you sure you want to leave?');
                    if (!isOk) {
                        event.preventDefault();
                    }
                });


            }
        };

    }]);
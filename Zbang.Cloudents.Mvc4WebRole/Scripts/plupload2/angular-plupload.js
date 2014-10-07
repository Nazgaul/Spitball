'use strict';

angular.module('angular-plupload', [])
	.directive('plUpload', ['$rootScope', '$timeout', function ($rootScope, $timeout) {
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


	            var randomValue = scope.randomString(5);
	            iAttrs.$set('id', randomValue);
	            var options = {
	                runtimes: 'html5,flash',
	                browse_button: iElement[0],
	                drop_element: document.getElementById(iAttrs.dropArea),
	                multi_selection: true,
	                chunk_size: '3mb',
	                container: 'main',
	                url: '/Upload/File/',
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


	                $rootScope.$apply(function () {
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
	                });


	            });

	            uploader.bind('UploadProgress', function (up, file) {
	                $rootScope.$apply(function () {
	                    $rootScope.$broadcast('UploadProgress', file);
	                });
	            });

	            uploader.bind('UploadComplete', function (up, files) {

	                cd.pubsub.publish('addPoints', { type: 'itemUpload', amount: files.length });

	                up.files = [];
	                up.splice();


	                if (iAttrs.destroy) {
	                    up.destroy();
	                    alert('a');
	                }

	            });

	            scope.$on('$destroy', function () {
	                uploader.disableBrowse();
	                alert('a');
	            });

	            scope.$on('CancelFileUpload', function () {
	            });
	            scope.$on('$locationChangeStart', function (event) {
	                if (uploader.runtime !== 'flash') {
	                    return;
	                }
	                var isOk = confirm('Leaving page will stop the file upload, are you sure you want to leave?');
	                if (!isOk) {
	                    event.preventDefault();
	                }
	            });

	            //uploader.bind('PostInit', function (event) {
	            //    if (uploader.runtime !== 'flash') {
	            //        return;
	            //    }

	            //    uploader.refresh();
	            //});

	        }
	    };
	}])
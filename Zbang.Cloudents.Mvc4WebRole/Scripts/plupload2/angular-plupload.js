'use strict';

angular.module('angular-plupload', [])
	.directive('plUpload', ['$rootScope', function ($rootScope) {
	    return {
	        restrict: 'A',
	        scope: {},
	        link: function (scope, iElement, iAttrs) {

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
	                runtimes: 'html5,flash,html4',
	                browse_button: iElement[0],
	                multi_selection: true,
	                chunk_size: '3mb',
	                container : 'main',
	                url: '/Upload/File/',
	                flash_swf_url: '/plupload2/Moxie.swf',
	                headers: {
	                    'X-Requested-With': 'XMLHttpRequest'
	                }
	            }

	            var uploader = new plupload.Uploader(options);


	            uploader.init();

	            uploader.bind('Error', function (up, err) {

	                alert("Cannot upload, error: " + err.message + (err.file ? ", File: " + err.file.name : "") + "");
	                up.refresh(); // Reposition Flash/Silverlight
	            });

	            uploader.bind('FilesAdded', function (up, files) {
	                _.forEach(files, function (file) {
	                    file.boxId = iAttrs.boxId;
	                    file.tabId = iAttrs.tabId || null;
	                    file.fileName = file.name;
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

	            });

	            uploader.bind('FileUploaded', function (up, file, res) {
	                $rootScope.$broadcast('FileUploaded', res);
	            });

	            uploader.bind('UploadProgress', function (up, file) {
	                $rootScope.$broadcast('UploadProgress', file);
	            });

	            uploader.bind('UploadComplete', function () {
	                uploader.destroy();
	            });


	            scope.$on('$destroy', function () {
	                uploader.disableBrowse();
	            });

	            $rootScope.$on('$locationChangeStart', function (event) {
	                if (uploader.runtime !== 'flash') {
	                    return;
	                }
	                var isOk = confirm('Leaving page will stop the file upload, arey you sure you want to leave?');
	                if (!isOk) {
	                    event.preventDefault();
	                }
	            });
	        }
	    };
	}])
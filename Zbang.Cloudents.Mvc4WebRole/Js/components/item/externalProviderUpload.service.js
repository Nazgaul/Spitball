(function () {
    'use strict';
    angular.module('app.upload').service('externalUploadProvider', upload);

    upload.$inject = ['dropboxService', 'googleService', '$q', 'itemService'];

    function upload(dropboxService, googleService, $q, itemService) {
        var self = this;
        self.dropboxInit = function () {
            return dropboxService.init();
        }

        self.googleDriveInit = function () {
            return $q.all([googleService.initDrive(), googleService.initGApi()]);
        }
        function buildGoogleDeferes(link, boxId, tab, name, isQuestion) {
            var defer1 = $q.defer();
            itemService.addLink(link, boxId, tab, isQuestion, name).then(function (response2) {
                defer1.resolve(response2);
            },function() {
                defer1.resolve();
            });
            return defer1.promise;
        }
        self.google = function (boxId, tab, isQuestion) {
            var defer = $q.defer();
            googleService.picker().then(function (response) {
                var filesUpload = [];
                for (var i = 0; i < response.length; i++) {
                    
                    filesUpload.push(buildGoogleDeferes(response[i].link, boxId, tab, response[i].name, isQuestion));
                    //    );
                }
                $q.all(filesUpload).then(function (retVal) {
                    defer.resolve(retVal);
                });
            });
            return defer.promise;
        }
        function buildDropBoxDeferes(link, boxId, tab, name, isQuestion) {
            var defer1 = $q.defer();
            itemService.addFromDropBox(boxId, tab, link, name, isQuestion).then(function (response2) {
                defer1.resolve(response2);
            }, function () {
                defer1.resolve();
            });
            return defer1.promise;
        }
        self.dropBox = function (boxId, tab, isQuestion) {
            var defer = $q.defer();
            dropboxService.choose().then(function (response) {
                var filesUpload = [];
                for (var i = 0; i < response.length; i++) {
                    filesUpload.push(buildDropBoxDeferes(response[i].link, boxId, tab, response[i].name, isQuestion));
                }
                $q.all(filesUpload).then(function (retVal) {
                    defer.resolve(retVal);
                });
            });
            return defer.promise;


        };

    }
})()
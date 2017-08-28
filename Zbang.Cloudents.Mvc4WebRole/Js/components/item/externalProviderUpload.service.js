"use strict";
var app;
(function (app) {
    "use strict";
    var ExternalUploadProvider = (function () {
        function ExternalUploadProvider(dropboxService, googleService, $q, itemService) {
            this.dropboxService = dropboxService;
            this.googleService = googleService;
            this.$q = $q;
            this.itemService = itemService;
        }
        ExternalUploadProvider.prototype.dropboxInit = function () {
            return this.dropboxService.init();
        };
        ExternalUploadProvider.prototype.googleDriveInit = function () {
            return this.$q.all([this.googleService.initDrive(), this.googleService.initGApi()]);
        };
        ExternalUploadProvider.prototype.buildGoogleDeferes = function (link, boxId, tab, name, isQuestion) {
            var defer1 = this.$q.defer();
            this.itemService.addGoogle(link, boxId, tab, isQuestion, name).then(function (response2) {
                defer1.resolve(response2);
            }, function (response3) {
                defer1.reject(response3);
            });
            return defer1.promise;
        };
        ExternalUploadProvider.prototype.google = function (boxId, tab, isQuestion) {
            var _this = this;
            var defer = this.$q.defer();
            this.googleService.picker().then(function (response) {
                var filesUpload = [];
                for (var i = 0; i < response.length; i++) {
                    filesUpload.push(_this.buildGoogleDeferes(response[i].link, boxId, tab, response[i].name, isQuestion));
                    //    );
                }
                _this.$q.all(filesUpload).then(function (retVal) {
                    defer.resolve(retVal);
                }, function (retVal) {
                    defer.reject(retVal);
                });
            });
            return defer.promise;
        };
        ExternalUploadProvider.prototype.buildDropBoxDeferes = function (link, boxId, tab, name, isQuestion) {
            var defer1 = this.$q.defer();
            this.itemService.addFromDropBox(boxId, tab, link, name, isQuestion).then(function (response2) {
                defer1.resolve(response2);
            }, function () {
                defer1.resolve();
            });
            return defer1.promise;
        };
        ExternalUploadProvider.prototype.dropBox = function (boxId, tab, isQuestion) {
            var _this = this;
            var defer = this.$q.defer();
            this.dropboxService.choose().then(function (response) {
                var filesUpload = [];
                for (var i = 0; i < response.length; i++) {
                    filesUpload.push(_this.buildDropBoxDeferes(response[i].link, boxId, tab, response[i].name, isQuestion));
                }
                _this.$q.all(filesUpload).then(function (retVal) {
                    defer.resolve(retVal);
                });
            });
            return defer.promise;
        };
        ;
        ExternalUploadProvider.$inject = ['dropboxService', 'googleService', '$q', 'itemService'];
        return ExternalUploadProvider;
    }());
    angular.module("app.upload").service("externalUploadProvider", ExternalUploadProvider);
})(app || (app = {}));
// #    OLD externalProviderUpload.service.js
//(function () {
//    'use strict';
//    angular.module('app.upload').service('externalUploadProvider', upload);
//    upload.$inject = ['dropboxService', 'googleService', '$q', 'itemService'];
//    function upload(dropboxService, googleService, $q, itemService) {
//        var self = this;
//        self.dropboxInit = function () {
//            return dropboxService.init();
//        }
//        self.googleDriveInit = function () {
//            return $q.all([googleService.initDrive(), googleService.initGApi()]);
//        }
//        function buildGoogleDeferes(link, boxId, tab, name, isQuestion) {
//            var defer1 = $q.defer();
//            itemService.addLink(link, boxId, tab, isQuestion, name).then(function (response2) {
//                defer1.resolve(response2);
//            }, function () {
//                defer1.resolve();
//            });
//            return defer1.promise;
//        }
//        self.google = function (boxId, tab, isQuestion) {
//            var defer = $q.defer();
//            googleService.picker().then(function (response) {
//                var filesUpload = [];
//                for (var i = 0; i < response.length; i++) {
//                    filesUpload.push(buildGoogleDeferes(response[i].link, boxId, tab, response[i].name, isQuestion));
//                    //    );
//                }
//                $q.all(filesUpload).then(function (retVal) {
//                    defer.resolve(retVal);
//                });
//            });
//            return defer.promise;
//        }
//        function buildDropBoxDeferes(link, boxId, tab, name, isQuestion) {
//            var defer1 = $q.defer();
//            itemService.addFromDropBox(boxId, tab, link, name, isQuestion).then(function (response2) {
//                defer1.resolve(response2);
//            }, function () {
//                defer1.resolve();
//            });
//            return defer1.promise;
//        }
//        self.dropBox = function (boxId, tab, isQuestion) {
//            var defer = $q.defer();
//            dropboxService.choose().then(function (response) {
//                var filesUpload = [];
//                for (var i = 0; i < response.length; i++) {
//                    filesUpload.push(buildDropBoxDeferes(response[i].link, boxId, tab, response[i].name, isQuestion));
//                }
//                $q.all(filesUpload).then(function (retVal) {
//                    defer.resolve(retVal);
//                });
//            });
//            return defer.promise;
//        };
//    }
//})() 
//# sourceMappingURL=externalProviderUpload.service.js.map
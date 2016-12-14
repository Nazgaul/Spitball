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
            this.itemService.addLink(link, boxId, tab, isQuestion, name).then(function (response2) {
                defer1.resolve(response2);
            }, function () {
                defer1.resolve();
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
                }
                _this.$q.all(filesUpload).then(function (retVal) {
                    defer.resolve(retVal);
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

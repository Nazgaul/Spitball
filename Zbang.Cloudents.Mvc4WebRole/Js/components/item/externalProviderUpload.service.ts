
module app {
    "use strict";

    export interface IExternalUploadProvider {
        dropboxInit(): angular.IPromise<any>;
        googleDriveInit(): angular.IPromise<any>;
        buildGoogleDeferes(link: string, boxId: number, tab: Guid, name: string, isQuestion: Guid): angular.IPromise<any>;
        google(boxId: number, tab: Guid, isQuestion: Guid): angular.IPromise<any>;
        buildDropBoxDeferes(link: string, boxId: number, tab: Guid, name: string, isQuestion: Guid);
        dropBox(boxId: number, tab: Guid, isQuestion: Guid);
    }


    class ExternalUploadProvider implements IExternalUploadProvider {
        static $inject = ['dropboxService', 'googleService', '$q', 'itemService']
        constructor(private dropboxService: IDropboxService,
            private googleService: IGoogleService,
            private $q: angular.IQService,
            private itemService: IItemService) {
        }

        dropboxInit() {
            return this.dropboxService.init();
        }

        googleDriveInit() {
            return this.$q.all([this.googleService.initDrive(), this.googleService.initGApi()]);
        }
        buildGoogleDeferes(link: string, boxId: number, tab: Guid, name: string, isQuestion: Guid) {
            var defer1 = this.$q.defer();
            this.itemService.addGoogle(link, boxId, tab, isQuestion, name).then((response2) => {
                defer1.resolve(response2);
            }, (response3) => {
                defer1.reject(response3);
            });
            return defer1.promise;
        }
        google(boxId: number, tab: Guid, isQuestion: Guid) {
            var defer = this.$q.defer();
            this.googleService.picker().then((response) => {
                var filesUpload = [];
                for (var i = 0; i < response.length; i++) {

                    filesUpload.push(this.buildGoogleDeferes(response[i].link, boxId, tab, response[i].name, isQuestion));
                    //    );
                }
                this.$q.all(filesUpload).then((retVal) => {
                    defer.resolve(retVal);
                }, retVal => {
                    defer.reject(retVal);
                });
            });
            return defer.promise;
        }
        buildDropBoxDeferes(link: string, boxId: number, tab: Guid, name: string, isQuestion: Guid) {
            var defer1 = this.$q.defer();
            this.itemService.addFromDropBox(boxId, tab, link, name, isQuestion).then((response2) => {
                defer1.resolve(response2);
            }, () => {
                defer1.resolve();
            });
            return defer1.promise;
        }
        dropBox(boxId: number, tab: Guid, isQuestion: Guid) {
            var defer = this.$q.defer();
            this.dropboxService.choose().then((response) => {
                var filesUpload = [];
                for (var i = 0; i < response.length; i++) {
                    filesUpload.push(this.buildDropBoxDeferes(response[i].link, boxId, tab, response[i].name, isQuestion));
                }
                this.$q.all(filesUpload).then((retVal) => {
                    defer.resolve(retVal);
                });
            });
            return defer.promise;


        };

    }
    angular.module("app.upload").service("externalUploadProvider", ExternalUploadProvider);
}

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
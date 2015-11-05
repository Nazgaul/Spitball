(function () {
    angular.module('app.upload').service('externalUploadProvider', upload);

    upload.$inject = ['dropboxService', 'googleService', '$q', 'itemService'];

    function upload(dropboxService, googleService, $q, itemService) {
        var self = this;


        //self.dropBoxLoaded = false;
        //self.googleDriveLoaded = false;

        //self.init = function () {

            
        //    $q.all([googleService.initDrive(), googleService.initGApi()]).then(function () {
        //        self.googleDriveLoaded = true;
        //    });
        //}

        self.dropboxInit = function() {
            return dropboxService.init();
        }

        self.googleDriveInit = function() {
            return $q.all([googleService.initDrive(), googleService.initGApi()]);
        }

        self.google = function (boxId) {
            var defer = $q.defer();
            googleService.picker().then(function (response) {
                var filesUpload = [];
                for (var i = 0; i < response.length; i++) {
                    filesUpload.push(itemService.addLink(response[i].link, boxId, null, null, response[i].name));
                }
                $q.all(filesUpload).then(function (retVal) {
                    defer.resolve(retVal);
                });
            });
            return defer.promise;
        }

        self.dropBox = function (boxId) {
            var defer = $q.defer();
            dropboxService.choose().then(function (response) {
                var filesUpload = [];
                for (var i = 0; i < response.length; i++) {
                    filesUpload.push(itemService.addFromDropBox(boxId, response[i].link, response[i].name));
                }
                $q.all(filesUpload).then(function (retVal) {
                    defer.resolve(retVal);
                });
            });
            return defer.promise;


        };

    }
})()
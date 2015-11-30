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

        self.dropboxInit = function () {
            return dropboxService.init();
        }

        self.googleDriveInit = function () {
            return $q.all([googleService.initDrive(), googleService.initGApi()]);
        }

        self.google = function (boxId) {
            var defer = $q.defer();
            googleService.picker().then(function (response) {
                defer.resolve(itemService.addLink(response[0].link, boxId, null, response[0].name));
            });
            return defer.promise;
        }

        self.dropBox = function (boxId) {
            var defer = $q.defer();
            dropboxService.choose().then(function (response) {
                defer.resolve(itemService.addFromDropBox(boxId, response[0].link, response[0].name));
            });
            return defer.promise;


        };

    }
})()
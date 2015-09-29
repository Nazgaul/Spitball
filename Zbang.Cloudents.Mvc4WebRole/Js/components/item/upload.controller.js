(function () {
    angular.module('app.upload').controller('Upload', upload);
    upload.$inject = ['$scope', 'itemService', 'dropboxService', '$q'];

    function upload($scope, itemService, dropboxService, $q) {
        var u = this;
        var cc = $scope.$parent.cc;

        var uploadChoose = {
            none: 0,
            computer: 1,
            drive: 2,
            dropbox: 3,
            link: 4
        };

        $scope.$on('uploadPhase', function() {
            dropboxService.init().then(function () {
                u.dropBoxLoaded = true;
            });
        });
        u.dropBoxLoaded = false;

        

        u.dropBox = function () {
           
            dropboxService.choose().then(function (response) {
                var filesUpload = [];
                for (var i = 0; i < response.length; i++) {
                    filesUpload.push(itemService.addFromDropBox(cc.box.id, response[i].link, response[i].name));
                }
                $q.all(filesUpload).then(function() {
                    alert('done');
                });
            });

           
        };

        u.uploadStep = uploadChoose.none;

        u.link = 'http://';
        //upload 
        u.uploadLink = function () {
            if (!u.link) {
                cc.alert = 'not a valid url';
                return;
            }
            cc.submitFormProcess = true;
            itemService.addLink(u.link, cc.box.id).then(function (response) {
                u.uploadStep = uploadChoose.none;
            }, function (response) {
                cc.alert = response;
            }).finally(function () {
                cc.submitFormProcess = false;
            });

        }
    }
})();
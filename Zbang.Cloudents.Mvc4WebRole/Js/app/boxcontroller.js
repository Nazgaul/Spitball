angular.module('mBox').
    controller('BoxController', ['$route','$q','Box', 'NewUpdates', function ($route,$q,Box, NewUpdates) {

        var infoPromise = $q.all(Box.info({ boxId: cd.getParameterFromUrl(2) }));
        var itemsPromise = $q.all(Box.items({ boxId: cd.getParameterFromUrl(2) }));
        var qnaPromise = $q.all(QnA.);
        Box.items().then(function (data) {

        }
    }]).
    controller('QnAController', ['Box', function (Box) {

    }]).
    controller('UploadController', ['Box', function (Box) {

    }]).
    controller('Settings', ['Box', function (Box) {

    }]);
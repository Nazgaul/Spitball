angular.module('box')
    .service('boxService',
    ['$window', 'box', function ($window, box) {
        var service = this;

        service.getData = function (boxId) {
            return box.data({ id: boxId });
        };

        service.goBack = function () {
            $window.history.back();
        };
    }]
);
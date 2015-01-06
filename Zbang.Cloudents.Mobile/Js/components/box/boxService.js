angular.module('box')
    .service('boxService',
    ['$rootScope', '$window', 'box', function ($rootScope, $window, box) {
        var service = this;

        service.getData = function (boxId) {
            return box.data({ id: boxId });
        };

        service.goBack = function () {
            $window.history.back();
        };

        service.doneLoad = function () {
            $rootScope.$broadcast('$stateLoaded');
        };
    }]
);
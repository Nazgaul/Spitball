angular.module('box')
    .service('boxService',
    ['$rootScope', 'box', function ($rootScope, box) {
        var service = this;

        service.getData = function (boxId) {
            return box.data({ id: boxId });
        };

        service.doneLoad = function () {
            $rootScope.$broadcast('$stateLoaded');
        };
    }]
);
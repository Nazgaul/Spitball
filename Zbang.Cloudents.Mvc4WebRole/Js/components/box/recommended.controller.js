
(function () {
    angular.module('app.box').controller('RecommendedController', recommended);
    recommended.$inject = ['boxService', '$stateParams'];

    function recommended(boxService, $stateParams) {
        var r = this;
        r.boxes = [];
        boxService.getRecommended($stateParams.boxId).then(function (response) {
            r.boxes = response;
        });

    }
})();
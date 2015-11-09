
(function () {
    angular.module('app.box').controller('RecommendedController', recommended);
    recommended.$inject = ['boxService', '$stateParams'];

    function recommended(boxService, $stateParams) {
        var r = this;


        if ($stateParams.boxtype.toLowerCase() === 'box') {
            r.isPrivate = true;
        } else {
            r.boxes = [];
            boxService.getRecommended($stateParams.boxId).then(function(response) {
                r.boxes = response;
            });
        }

    }
})();
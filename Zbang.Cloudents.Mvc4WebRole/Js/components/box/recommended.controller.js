(function () {
    'use strict';
    angular.module('app.box').controller('RecommendedController', recommended);
    recommended.$inject = ['boxService', '$stateParams'];

    function recommended(boxService, $stateParams) {
        var r = this;

        if ($stateParams.boxtype.toLowerCase() === 'box') {
            r.isPrivate = true;
        } else {
            //r.boxes = [];
            boxService.getRecommended($stateParams.boxId).then(function (response) {
                iterateItemAndAssign(response);
            });
        }

        function iterateItemAndAssign(response) {
            for (var i = 0; i < response.length; i++) {
                response[i].recommended = true;
            }
            r.boxes = response;
        }

    }
})();
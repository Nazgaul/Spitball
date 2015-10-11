(function () {
    angular.module('app.box').controller('BoxController', box);
    box.$inject = ['boxService', '$stateParams', '$location', '$scope', '$timeout', '$state'];

    function box(boxService, $stateParams, $location, $scope, $timeout, $state) {

        console.log($state);
        if (!$location.hash()) {
            $state.go('box.feed');
        }

        var b = this;
        var boxData;
        boxService.getBox($stateParams.boxId).then(function (response) {
            boxData = response;
            b.name = response.name;
            b.professorName = response.professorName;
            b.courseId = response.courseId;
        });

        $scope.$on('$viewContentLoaded', function () {
            $timeout(function () {
                Metronic.initAjax();
            });
            //TODO: maybe this is no good.
            //Metronic.initAjax(); // init core components

        });
    }
})();



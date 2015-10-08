(function () {
    angular.module('app.box').controller('BoxController', box);
    box.$inject = ['boxService', '$stateParams', '$location','$scope' ,'$timeout'];

    function box(boxService, $stateParams, $location, $scope, $timeout) {

        if (!$location.hash()) {
            $location.hash('feed');
        }
        //if ($location.path().endsWith($stateParams.boxName + '/')) {
        //        $location.path($location.path() + 'feed/');
        //}

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



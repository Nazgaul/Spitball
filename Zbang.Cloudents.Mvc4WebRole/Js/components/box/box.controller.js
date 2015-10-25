(function () {
    angular.module('app.box').controller('BoxController', box);
    box.$inject = ['boxService', '$stateParams', '$location', '$scope', '$timeout', '$state'];

    function box(boxService, $stateParams, $location, $scope, $timeout, $state) {

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
       // b.inviteOpen = true;
        b.inviteToBox = function () {
            b.inviteOpen = true;
            $scope.$broadcast('open_invite');
        }

        $scope.$on("close_invite", function () {
            b.inviteOpen = false;
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



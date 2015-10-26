(function () {
    angular.module('app.box').controller('BoxController', box);
    box.$inject = ['boxService', '$stateParams', '$location', '$scope', '$timeout', '$state', '$rootScope'];

    function box(boxService, $stateParams, $location, $scope, $timeout, $state, $rootScope) {

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


        b.inviteToBox = function () {
            b.inviteOpen = true;
            $scope.$broadcast('open_invite');
        }

        $scope.$on("close_invite", function () {
            b.inviteOpen = false;
        });

        b.uploadOn = false;
        b.uploadShow = isItemState($state.current.name);
        b.toggleUpload = function () {

            b.uploadShow = !b.uploadShow;
            b.uploadOn = !b.uploadOn;
        }


        //$scope.$on('$viewContentLoaded', function () {
        //    $timeout(function () {
        //        Metronic.initAjax();
        //    });
        //    //TODO: maybe this is no good.
        //    //Metronic.initAjax(); // init core components

        //});

        function isItemState(stateName) {
            return stateName === 'box.items';
        }

        $rootScope.$on('$stateChangeSuccess',
            function (event, toState, toParams, fromState, fromParams) {
                
                if (isItemState(toState.name)) {
                    b.uploadShow = true;
                } else {
                    b.uploadShow = false;
                }

            });
    }
})();



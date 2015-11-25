(function () {
    angular.module('app.box').controller('BoxController', box);
    box.$inject = ['boxService', 'boxData', '$stateParams', '$location', '$scope',  '$state', '$rootScope'];

    function box(boxService, boxData, $stateParams, $location, $scope,  $state, $rootScope) {

        if (!$location.hash()) {
            $state.go('box.feed');
        }
        var b = this;
        b.data = boxData;

        b.needFollow = boxData.userType === 'invite' || boxData.userType === 'none';
        //b.name = boxData.name;
        //b.professorName = boxData.professorName;
        //b.courseId = boxData.courseId;
        b.follow = follow;

        b.inviteToBox = function () {
            b.inviteOpen = true;
            $scope.$broadcast('open_invite');
        }

        $scope.$on("close_invite", function () {
            b.inviteOpen = false;
        });

        b.uploadOn = false;
        b.uploadShow = isItemState($state.current.name);
        b.toggleUpload = function (open) {

            b.uploadShow = !open;
            b.uploadOn = open;

            $scope.$broadcast('open_upload');
        }



        function follow() {
            boxService.follow($stateParams.boxId);
            b.needFollow = false;
        }
        function isItemState(stateName) {
            return stateName === 'box.items';
        }

        $rootScope.$on('$stateChangeSuccess',
            function (event, toState) {

                if (isItemState(toState.name)) {
                    b.uploadShow = true;
                } else {
                    b.uploadShow = false;
                }

            });
    }
})();



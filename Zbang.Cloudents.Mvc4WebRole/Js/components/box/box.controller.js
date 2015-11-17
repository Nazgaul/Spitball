(function () {
    angular.module('app.box').controller('BoxController', box);
    box.$inject = ['boxService', 'boxData', '$stateParams', '$location', '$scope', '$timeout', '$state', '$rootScope'];

    function box(boxService, boxData, $stateParams, $location, $scope, $timeout, $state, $rootScope) {

        if (!$location.hash()) {
            $state.go('box.feed');
        }
        var b = this;
        b.data = boxData;
        
        //b.name = boxData.name;
        //b.professorName = boxData.professorName;
        //b.courseId = boxData.courseId;


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



(function () {
    angular.module('app.dashboard').controller('Dashboard', dashboard);
    dashboard.$inject = ['dashboardService', '$scope', '$mdDialog', 'boxService', '$rootScope', 'resManager'];

    function dashboard(dashboardService, $scope, $mdDialog, boxService, $rootScope, resManager) {
        var d = this;
        d.boxes = [];
        d.inviteOpen = false;
        d.inviteToSpitabll = function () {
            d.inviteOpen = true;
            $scope.$broadcast('open_invite');
        }


        d.boxes = [];
        dashboardService.getBoxes().then(function (response) {
            d.boxes = d.boxes.concat(response);
            dashboardService.recommended().then(function (response2) {
                for (var i = 0; i < response2.length; i++) {
                    response2[i].recommended = true;
                    response2[i].updates = 0;
                }
                d.boxes = d.boxes.concat(response2);
            });
        });

        d.deleteBox = deleteBox;

        function deleteBox(ev, box) {

            //boxType //userType
            var confirm = $mdDialog.confirm()
                  .title(resManager.get('unfollowClass'))
                  .targetEvent(ev)
                   .ok(resManager.get('dialogOk'))
                 .cancel(resManager.get('dialogCancel'));

            $mdDialog.show(confirm).then(function () {
                var index = d.boxes.indexOf(box);
                $rootScope.$broadcast('remove-box', box.id);
                d.boxes.splice(index, 1);
                boxService.unfollow(box.id);
            });
        }

        $scope.$on("close_invite", function () {
            d.inviteOpen = false;
        });

    }
})();







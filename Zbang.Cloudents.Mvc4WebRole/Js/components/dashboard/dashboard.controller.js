(function () {
    angular.module('app.dashboard').controller('Dashboard', dashboard);
    dashboard.$inject = ['dashboardService', 'boxes', '$scope', '$mdDialog', 'boxService',
        '$rootScope', 'resManager', 'ajaxService'];

    function dashboard(dashboardService, boxes, $scope, $mdDialog, boxService, $rootScope,
        resManager, ajaxService) {
        var d = this;
        d.boxes = [];
        d.inviteOpen = false;
        d.showLeaderboard = true;

        d.inviteExpand = inviteExpand;
        d.inviteToSpitabll = inviteToSpitabll; 


        d.boxes = boxes;
        dashboardService.recommended().then(function (response2) {
            d.suggested = response2;
            for (var i = 0; i < response2.length; i++) {
                var retVal = response2[i];
                retVal.recommended = true;
                retVal.updates = 0;
            }
        });

        d.deleteBox = deleteBox;
        d.openCreate = openCreate;
        $scope.math = Math;

        function inviteExpand() {
            if (d.html) {
                return;
            }
            return ajaxService.getHtml('/share/invitedialog/').then(function (response) {
                d.html = response;
            });
        }
        function inviteToSpitabll() {
            d.inviteOpen = true;
            $scope.$broadcast('open_invite');
        }

        function openCreate() {
            d.createBoxOn = true;
            d.createBoxFocus = true;
        }

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

        $scope.$on('hide-leader-board', function () {
           d.showLeaderboard = false;
        });

    }
})();







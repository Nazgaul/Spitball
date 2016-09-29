
(function () {
    'use strict';
    angular.module('app.dashboard').controller('Dashboard', dashboard);
    dashboard.$inject = ['dashboardService', 'boxes', '$scope', '$mdDialog', 'boxService',
        '$rootScope', 'resManager', 'ajaxService2'];

    function dashboard(dashboardService, boxes, $scope, $mdDialog,
        boxService, $rootScope,resManager, ajaxService) {
        var d = this;
        d.inviteOpen = false;
        d.showLeaderboard = true;

        d.inviteExpand = inviteExpand;
        d.inviteToSpitabll = inviteToSpitabll;
        d.createNewBox = createNewBox;

        d.boxes = boxes;
        dashboardService.recommended().then(function (response2) {
            for (var i = 0; i < response2.length; i++) {
                var retVal = response2[i];
                retVal.recommended = true;
                retVal.updates = 0;
            }
            d.suggested = response2;
        });
        //$uiViewScroll($('.dashboard-stat2:last'));

        d.deleteBox = deleteBox;
        //$scope.math = Math;

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
            d.createBoxOn = false;
        }

        function createNewBox() {
            if (!d.createBoxOn) {
                d.createBoxOn = true;
                d.inviteOpen = false;
            }
            else {
                d.createBoxOn = false;
            }
        }
        //function openCreate() {
        //    d.createBoxOn = true;
        //    //d.createBoxFocus = true;
        //}

        function deleteBox(ev, box) {

            //boxType //userType
            var confirm = $mdDialog.confirm()
                  .title(resManager.get('unfollowClass'))
                  .targetEvent(ev)
                   .ok(resManager.get('dialogOk'))
                 .cancel(resManager.get('dialogCancel'));

            $mdDialog.show(confirm).then(function () {
                var index = d.boxes.indexOf(box);
                
                d.boxes.splice(index, 1);
                boxService.unfollow(box.id).then(function() {
                    $rootScope.$broadcast('remove-box', box.id);
                });
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







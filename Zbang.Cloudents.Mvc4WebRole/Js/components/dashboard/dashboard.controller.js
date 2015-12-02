﻿(function () {
    angular.module('app.dashboard').controller('Dashboard', dashboard);
    dashboard.$inject = ['dashboardService', '$scope', '$mdDialog', 'boxService'];

    function dashboard(dashboardService, $scope, $mdDialog, boxService) {
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

            //console.log(box);
            //if (box.boxType === 'academic')
            //boxType //userType
            // Appending dialog to document.body to cover sidenav in docs app
            var confirm = $mdDialog.confirm()
                  .title('Would you like to unfollow this class?')
                  //.textContent('All of the banks have agreed to forgive you your debts.')
                  .targetEvent(ev)
                  .ok('Ok')
                  .cancel('Cancel');

            $mdDialog.show(confirm).then(function () {

                var index = d.boxes.indexOf(box);
                d.boxes.splice(index, 1);
                boxService.unfollow(box.id);
            });
        }

        $scope.$on("close_invite", function () {
            d.inviteOpen = false;
        });

    }
})();







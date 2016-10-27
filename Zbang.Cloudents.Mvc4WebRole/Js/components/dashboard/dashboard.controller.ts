module app {
    "use strict";

    class Dashboard {
        static $inject = ["dashboardService", "boxes", "$scope", "$mdDialog", "boxService",
            "$rootScope", "resManager", "ajaxService2"];

        inviteOpen = false;
        //boxes;
        //suggested;
        html;
        createBoxOn;
        showLeaderboard = true;
        constructor(
            private dashboardService: IDashboardService,
            // TODO
            private boxes: any,
            private $scope: angular.IScope,
            private $mdDialog: angular.material.IDialogService,
            private boxService: IBoxService,
            private $rootScope: angular.IRootScopeService,
            private resManager: IResManager,
            private ajaxService2: IAjaxService2
        ) {
            //this.boxes2 = boxes;
            //dashboardService.recommended().then(response2 => {
            //    for (let i = 0; i < response2.length; i++) {
            //        const retVal = response2[i];
            //        retVal.recommended = true;
            //        retVal.updates = 0;
            //    }
            //    this.suggested = response2;
            //});
            $scope.$on("close_invite", () => {
                this.inviteOpen = false;
            });
            $scope.$on('hide-leader-board', () => {
                this.showLeaderboard = false;
            });
        }


        inviteExpand() {
            if (this.html) {
                return;
            }
            return this.ajaxService2.getHtml('/share/invitedialog/').then(response => {
                this.html = response;
            });
        }
        inviteToSpitabll() {
            this.inviteOpen = true;
            this.$scope.$broadcast('open_invite');
            this.createBoxOn = false;
        }
        createNewBox() {
            if (!this.createBoxOn) {
                this.createBoxOn = true;
                this.inviteOpen = false;
            }
            else {
                this.createBoxOn = false;
            }
        }
        //d.showLeaderboard = true;
        //$uiViewScroll($('.dashboard-stat2:last'));

        //$scope.math = Math;





        //function openCreate() {
        //    d.createBoxOn = true;
        //    //d.createBoxFocus = true;
        //}

        deleteBox(ev, box) {
            //boxType //userType
            const confirm = this.$mdDialog.confirm()
                .title(this.resManager.get('unfollowClass'))
                .targetEvent(ev)
                .ok(this.resManager.get('dialogOk'))
                .cancel(this.resManager.get('dialogCancel'));

            this.$mdDialog.show(confirm).then(() => {
                var index = this.boxes.indexOf(box);

                this.boxes.splice(index, 1);
                this.boxService.unfollow(box.id).then(() => {
                    this.$rootScope.$broadcast('remove-box', box.id);
                });
            });
        }


    }
    angular.module("app.dashboard").controller("Dashboard", Dashboard);
}
//(function () {
//    'use strict';
//    angular.module('app.dashboard').controller('Dashboard', dashboard);
//    dashboard.$inject = ['dashboardService', 'boxes', '$scope', '$mdDialog', 'boxService',
//        '$rootScope', 'resManager', 'ajaxService2'];

//    function dashboard(dashboardService, boxes, $scope, $mdDialog,
//        boxService, $rootScope, resManager, ajaxService) {
//        var d = this;
//        d.inviteOpen = false;
//        //d.showLeaderboard = true;

//        d.inviteExpand = inviteExpand;
//        d.inviteToSpitabll = inviteToSpitabll;
//        d.createNewBox = createNewBox;

//        d.boxes = boxes;
//        dashboardService.recommended().then(function (response2) {
//            for (var i = 0; i < response2.length; i++) {
//                var retVal = response2[i];
//                retVal.recommended = true;
//                retVal.updates = 0;
//            }
//            d.suggested = response2;
//        });
//        //$uiViewScroll($('.dashboard-stat2:last'));

//        d.deleteBox = deleteBox;
//        //$scope.math = Math;

//        function inviteExpand() {
//            if (d.html) {
//                return;
//            }
//            return ajaxService.getHtml('/share/invitedialog/').then(function (response) {
//                d.html = response;
//            });
//        }
//        function inviteToSpitabll() {
//            d.inviteOpen = true;
//            $scope.$broadcast('open_invite');
//            d.createBoxOn = false;
//        }

//        function createNewBox() {
//            if (!d.createBoxOn) {
//                d.createBoxOn = true;
//                d.inviteOpen = false;
//            }
//            else {
//                d.createBoxOn = false;
//            }
//        }
//        //function openCreate() {
//        //    d.createBoxOn = true;
//        //    //d.createBoxFocus = true;
//        //}

//        function deleteBox(ev, box) {

//            //boxType //userType
//            var confirm = $mdDialog.confirm()
//                .title(resManager.get('unfollowClass'))
//                .targetEvent(ev)
//                .ok(resManager.get('dialogOk'))
//                .cancel(resManager.get('dialogCancel'));

//            $mdDialog.show(confirm).then(function () {
//                var index = d.boxes.indexOf(box);

//                d.boxes.splice(index, 1);
//                boxService.unfollow(box.id).then(function () {
//                    $rootScope.$broadcast('remove-box', box.id);
//                });
//            });
//        }

//        $scope.$on("close_invite", function () {
//            d.inviteOpen = false;
//        });

//        //$scope.$on('hide-leader-board', function () {
//        //    d.showLeaderboard = false;
//        //});

//    }
//})();







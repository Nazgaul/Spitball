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

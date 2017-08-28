"use strict";
var app;
(function (app) {
    "use strict";
    var Dashboard = (function () {
        //showLeaderboard = true;
        function Dashboard(dashboardService, 
            // TODO
            boxes, $scope, $mdDialog, boxService, $rootScope, resManager, ajaxService2) {
            var _this = this;
            this.dashboardService = dashboardService;
            this.boxes = boxes;
            this.$scope = $scope;
            this.$mdDialog = $mdDialog;
            this.boxService = boxService;
            this.$rootScope = $rootScope;
            this.resManager = resManager;
            this.ajaxService2 = ajaxService2;
            this.inviteOpen = false;
            //this.boxes2 = boxes;
            //dashboardService.recommended().then(response2 => {
            //    for (let i = 0; i < response2.length; i++) {
            //        const retVal = response2[i];
            //        retVal.recommended = true;
            //        retVal.updates = 0;
            //    }
            //    this.suggested = response2;
            //});
            $scope.$on("close_invite", function () {
                _this.inviteOpen = false;
            });
            //$scope.$on('hide-leader-board', () => {
            //    this.showLeaderboard = false;
            //});
        }
        Dashboard.prototype.inviteExpand = function () {
            var _this = this;
            if (this.html) {
                return;
            }
            return this.ajaxService2.getHtml('/share/invitedialog/').then(function (response) {
                _this.html = response;
            });
        };
        Dashboard.prototype.inviteToSpitabll = function () {
            this.inviteOpen = true;
            this.$scope.$broadcast('open_invite');
            this.createBoxOn = false;
        };
        Dashboard.prototype.createNewBox = function () {
            if (!this.createBoxOn) {
                this.createBoxOn = true;
                this.inviteOpen = false;
            }
            else {
                this.createBoxOn = false;
            }
        };
        //d.showLeaderboard = true;
        //$uiViewScroll($('.dashboard-stat2:last'));
        //$scope.math = Math;
        //function openCreate() {
        //    d.createBoxOn = true;
        //    //d.createBoxFocus = true;
        //}
        Dashboard.prototype.deleteBox = function (ev, box) {
            var _this = this;
            //boxType //userType
            var confirm = this.$mdDialog.confirm()
                .title(this.resManager.get('unfollowClass'))
                .targetEvent(ev)
                .ok(this.resManager.get('dialogOk'))
                .cancel(this.resManager.get('dialogCancel'));
            this.$mdDialog.show(confirm).then(function () {
                var index = _this.boxes.indexOf(box);
                _this.boxes.splice(index, 1);
                _this.boxService.unfollow(box.id).then(function () {
                    _this.$rootScope.$broadcast('remove-box', box.id);
                });
            });
        };
        Dashboard.$inject = ["dashboardService", "boxes", "$scope", "$mdDialog", "boxService",
            "$rootScope", "resManager", "ajaxService2"];
        return Dashboard;
    }());
    angular.module("app.dashboard").controller("Dashboard", Dashboard);
})(app || (app = {}));
//# sourceMappingURL=dashboard.controller.js.map
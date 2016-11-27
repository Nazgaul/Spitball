var app;
(function (app) {
    "use strict";
    var Dashboard = (function () {
        function Dashboard(dashboardService, boxes, $scope, $mdDialog, boxService, $rootScope, resManager, ajaxService2) {
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
            this.showLeaderboard = true;
            $scope.$on("close_invite", function () {
                _this.inviteOpen = false;
            });
            $scope.$on('hide-leader-board', function () {
                _this.showLeaderboard = false;
            });
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
        Dashboard.prototype.deleteBox = function (ev, box) {
            var _this = this;
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

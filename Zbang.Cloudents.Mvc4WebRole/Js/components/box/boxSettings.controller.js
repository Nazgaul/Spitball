var app;
(function (app) {
    "use strict";
    var boxId;
    var BoxSettingsController = (function () {
        function BoxSettingsController($state, $stateParams, $scope, $rootScope, resManager, boxService, ajaxService2, $timeout, showToasterService) {
            var _this = this;
            this.$state = $state;
            this.$stateParams = $stateParams;
            this.$scope = $scope;
            this.$rootScope = $rootScope;
            this.resManager = resManager;
            this.boxService = boxService;
            this.ajaxService2 = ajaxService2;
            this.$timeout = $timeout;
            this.showToasterService = showToasterService;
            boxId = $stateParams.boxId;
            $scope.$on("close-collapse", function () {
                var boxController = _this.$scope["b"];
                boxController["inviteOpen"] = false;
                boxController["settingsOpen"] = false;
            });
            this.$timeout(function () {
                var boxController = _this.$scope["b"];
                _this.$rootScope.$broadcast('close-collapse');
                boxController["settingsOpen"] = true;
                _this.settings = _this.settings || {};
                _this.settings.name = boxController["data"].name;
                _this.settings.needFollow = boxController["needFollow"];
                _this.settings.submitDisabled = false;
                if (boxController["isAcademic"]) {
                    _this.settings.courseId = boxController["data"].courseId;
                    _this.settings.professorName = boxController["data"].professorName;
                }
                else if (boxController["owner"]) {
                    _this.settings.privacy = boxController["data"].privacySetting;
                }
                if (!_this.settings.notificationSettings) {
                    _this.boxService.notification(boxId).then(function (response2) {
                        _this.settings.notificationSettings = response2;
                    });
                }
            });
        }
        BoxSettingsController.prototype.updateBox = function (updateBoxForm) {
            var _this = this;
            if (this.settings.needFollow) {
                this.boxService.unfollow(boxId).then(function () {
                    _this.$rootScope.$broadcast("remove-box", boxId);
                    _this.$state.go("dashboard");
                });
                return;
            }
            var needToSave = false;
            angular.forEach(updateBoxForm, function (value, key) {
                if (key[0] === "$")
                    return;
                if (!needToSave) {
                    needToSave = !value.$pristine;
                }
            });
            if (needToSave) {
                var boxController_1 = this.$scope["b"];
                boxController_1["data"].name = this.settings.name;
                boxController_1["data"].privacySetting = this.settings.privacy;
                if (boxController_1["isAcademic"]) {
                    boxController_1["data"].courseId = this.settings.courseId;
                    boxController_1["data"].professorName = this.settings.professorName;
                }
                this.settings.submitDisabled = true;
                this.boxService.updateBox(boxId, boxController_1["data"].name, this.settings.courseId, this.settings.professorName, this.settings.privacy, this.settings.notificationSettings).then(function (response) {
                    var appController = _this.$scope["app"];
                    boxController_1["settingsOpen"] = false;
                    _this.$stateParams["boxName"] = response.queryString;
                    _this.showToasterService.showToaster(_this.resManager.get("toasterBoxSettings"));
                    _this.$state.go('box.feed', _this.$stateParams, { location: "replace" });
                }).finally(function () {
                    _this.settings.submitDisabled = false;
                });
            }
        };
        return BoxSettingsController;
    }());
    BoxSettingsController.$inject = ["$state", "$stateParams", "$scope",
        "$rootScope", "resManager", "boxService", "ajaxService2", "$timeout", "showToasterService"];
    angular.module('app.box').controller('BoxSettingsController', BoxSettingsController);
})(app || (app = {}));
//# sourceMappingURL=boxSettings.controller.js.map
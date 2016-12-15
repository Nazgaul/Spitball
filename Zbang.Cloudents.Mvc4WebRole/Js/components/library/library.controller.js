var app;
(function (app) {
    "use strict";
    var Library = (function () {
        function Library($anchorScroll, $stateParams, nodeData, universityData, itemThumbnailService, $mdMedia, $mdDialog, resManager, libraryService, showToasterService, $location, ajaxService2, $timeout, user, $state) {
            this.$anchorScroll = $anchorScroll;
            this.$stateParams = $stateParams;
            this.nodeData = nodeData;
            this.universityData = universityData;
            this.itemThumbnailService = itemThumbnailService;
            this.$mdMedia = $mdMedia;
            this.$mdDialog = $mdDialog;
            this.resManager = resManager;
            this.libraryService = libraryService;
            this.showToasterService = showToasterService;
            this.$location = $location;
            this.ajaxService2 = ajaxService2;
            this.$timeout = $timeout;
            this.user = user;
            this.$state = $state;
            this.submitDisabled = false;
            $anchorScroll.yOffset = 70;
            this.departments = nodeData.nodes;
            this.boxes = nodeData.boxes || [];
            this.nodeDetail = nodeData.details;
            this.buildState();
            this.university = {
                name: universityData.name,
                image: universityData.cover,
                logo: universityData.logo || itemThumbnailService.logo,
                id: universityData.id
            };
            this.createClassShow = this.state.emptyNode || this.state.withBoxes && $mdMedia('gt-xs');
            ;
            this
                .createDepartmentShowButton = !$stateParams["id"] || this.nodeDetail.state === "closed";
        }
        Library.prototype.goToSubLib = function (dep, $event) {
            var _this = this;
            if (dep.state === 'closed' && (dep.userType === 'pending' || dep.userType === 'none')) {
                var confirm_1 = this.$mdDialog.confirm()
                    .title(this.resManager.get('privateDepPopupTitle'))
                    .textContent(this.resManager.get('privateDepPopupContent'))
                    .targetEvent($event)
                    .ok(this.resManager.get('privateDepPopupRequestButton'))
                    .cancel(this.resManager.get('dialogCancel'));
                this.$mdDialog.show(confirm_1)
                    .then(function () {
                    _this.libraryService.requestAccess(dep.id)
                        .then(function () {
                        _this.$mdDialog.show(_this.$mdDialog.alert()
                            .title(_this.resManager.get('privateDepPopupTitleOnSend'))
                            .textContent(_this.resManager.get('privateDepPopupContentOnSend'))
                            .ok(_this.resManager.get('dialogOk')));
                    });
                });
                $event.preventDefault();
            }
        };
        Library.prototype.buildState = function () {
            this.state = {
                topTree: this.$stateParams["id"] == null && this.departments.length,
                emptyTopTree: this.$stateParams["id"] == null && this.departments.length === 0,
                withBoxes: this.boxes.length,
                emptyNode: this.$stateParams["id"] != null && this.boxes.length === 0 && this.departments.length === 0
            };
        };
        Library.prototype.createFirstBox = function (myform) {
            var _this = this;
            if (!myform.$valid) {
                return;
            }
            this.submitDisabled = true;
            this.libraryService.createDepartment(this.departmentName).then(function (response) {
                _this.departments.push(response);
                _this.libraryService.createClass(_this.boxName, _this.code, _this.professor, response.id).then(function (response2) {
                    _this.showToasterService.showToaster(_this.resManager.get('toasterCreateCourse'));
                    _this.$location.url(response2.url);
                }, function (response2) {
                    myform["depName"].$setValidity('server', false);
                    _this.error = response2;
                }).finally(function () {
                    _this.submitDisabled = false;
                });
            }, function (response) {
                myform["depName"].$setValidity('server', false);
                _this.error = response;
            }).finally(function () {
                _this.submitDisabled = false;
            });
        };
        Library.prototype.renameNode = function (myform) {
            var _this = this;
            if (!myform.$valid) {
                return;
            }
            this.libraryService.updateSettings(this.settings.name, this.$stateParams["id"], this.settings.privacy).then(function () {
                _this.nodeDetail.name = _this.settings.name;
                _this.nodeDetail.state = _this.settings.privacy;
                _this.settingsOpen = false;
            }, function (response) {
                myform["name"].$setValidity('server', false);
                _this.error = response;
            }).finally(function () {
                _this.submitDisabled = false;
            });
        };
        Library.prototype.toggleSettings = function () {
            var _this = this;
            this.createBoxOn = false;
            if (this.settingsHtml) {
                if (this.settingsOpen) {
                    this.settingsOpen = false;
                }
                else {
                    this.settingsOpen = true;
                }
                return;
            }
            return this.ajaxService2.getHtml("/library/unisettings").then(function (response) {
                _this.settingsHtml = response;
                _this.$timeout(function () {
                    _this.settings = {
                        name: _this.nodeDetail.name
                    };
                    _this.settingsOpen = true;
                });
            });
        };
        Library.prototype.openCreateBox = function () {
            var _this = this;
            this.createOn = true;
            this.settingsOpen = false;
            this.createDepartmentOn = false;
            if (this.createBoxOn) {
                this.createBoxOn = false;
            }
            else {
                this.createBoxOn = true;
            }
            this.$timeout(function () {
                _this.$anchorScroll("createCourse");
            });
        };
        Library.prototype.openCreateDepartment = function () {
            var _this = this;
            this.createOn = true;
            if (this.createDepartmentOn) {
                this.createDepartmentOn = false;
            }
            else {
                this.createDepartmentOn = true;
            }
            this.createBoxOn = false;
            this.settingsOpen = false;
            this.focusCreateDepartment = true;
            this.$timeout(function () {
                _this.$anchorScroll("createDepartment");
            });
        };
        Library.prototype.createBox = function (myform) {
            var _this = this;
            if (!this.secondStep) {
                this.secondStep = true;
                return;
            }
            this.submitDisabled = true;
            this.libraryService.createClass(this.boxName, this.code, this.professor, this.$stateParams["id"]).then(function (response) {
                _this.boxCancel();
                _this.createClassShow = _this.secondStep = false;
                _this.showToasterService.showToaster(_this.resManager.get('toasterCreateCourse'));
                _this.$location.url(response.url);
            }, function (response) {
                myform["professor"].$setValidity('server', false);
                _this.error = response;
            }).finally(function () {
                _this.submitDisabled = false;
            });
        };
        ;
        Library.prototype.createDepartment = function (myform) {
            var _this = this;
            this.submitDisabled = true;
            this.libraryService.createDepartment(this.departmentName, this.$stateParams["id"]).then(function (response) {
                _this.showToasterService.showToaster(_this.resManager.get('toasterCreateDepartment'));
                _this.$state.go("departmentWithNode", {
                    universityId: _this.$stateParams["universityId"],
                    universityName: _this.$stateParams["universityName"],
                    nodeName: response.name,
                    id: response.id
                });
            }, function (response) {
                myform["name"].$setValidity('server', false);
                _this.error = response;
            }).finally(function () {
                _this.submitDisabled = false;
            });
        };
        ;
        Library.prototype.canDelete = function (dep) {
            return this.user.isAdmin && dep.noDepartment === 0 && dep.noBoxes === 0;
        };
        Library.prototype.boxNext = function () {
            this.secondStep = true;
        };
        Library.prototype.boxCancel = function () {
            if (this.secondStep) {
                this.secondStep = false;
                return;
            }
            this.createBoxOn = false;
            this.createOn = false;
            this.resetFiled();
        };
        Library.prototype.resetFiled = function (myform) {
            this.departmentName = this.boxName = this.code = this.professor = '';
            if (myform) {
                myform.$setPristine();
                myform.$setUntouched();
            }
        };
        Library.prototype.departmentCancel = function (myform) {
            this.resetFiled(myform);
            this.createDepartmentOn = false;
            this.createOn = false;
        };
        Library.prototype.deleteDepartment = function (ev, department) {
            var _this = this;
            var confirm = this.$mdDialog.confirm()
                .title(this.resManager.get('deleteDepartment'))
                .targetEvent(ev)
                .ok(this.resManager.get('dialogOk'))
                .cancel(this.resManager.get('dialogCancel'));
            this.$mdDialog.show(confirm).then(function () {
                var index = _this.departments.indexOf(department);
                _this.departments.splice(index, 1);
                _this.buildState();
                _this.libraryService.deleteDepartment(department.id);
                _this.createClassShow = _this.departments.length === 0 && _this.$stateParams["id"] != null;
            });
        };
        Library.$inject = ["$anchorScroll", "$stateParams", "nodeData",
            "universityData", "itemThumbnailService", "$mdMedia", "$mdDialog", "resManager",
            "libraryService", "showToasterService", "$location", "ajaxService2", "$timeout", "user", "$state"];
        return Library;
    }());
    angular.module("app.library").controller("Library", Library);
})(app || (app = {}));

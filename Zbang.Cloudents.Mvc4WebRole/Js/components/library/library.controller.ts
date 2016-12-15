module app {
    "use strict";
    
    type nodeType = "closed" | "open";

    interface INodeDetails {
        name: string;
        parentUrl: string;
        state: nodeType ;
        userType: string;
    }
    class Library {
        static $inject = ["$anchorScroll", "$stateParams", "nodeData",
            "universityData", "itemThumbnailService", "$mdMedia", "$mdDialog", "resManager",
            "libraryService", "showToasterService", "$location", "ajaxService2", "$timeout", "user", "$state"];
        departments;
        boxes;
        nodeDetail: INodeDetails;
        university;
        createClassShow;
        createDepartmentShowButton;
        submitDisabled = false;
        state;
        error;
        constructor(
            private $anchorScroll: angular.IAnchorScrollService,
            private $stateParams: angular.ui.IStateParamsService,
            private nodeData: any,
            private universityData: any,
            private itemThumbnailService: IItemThumbnailService,
            private $mdMedia: angular.material.IMedia,
            private $mdDialog: angular.material.IDialogService,
            private resManager: IResManager,
            private libraryService: ILibraryService,
            private showToasterService: IShowToasterService,
            private $location: angular.ILocationService,
            private ajaxService2: IAjaxService2,
            private $timeout: angular.ITimeoutService,
            private user: IUserData,
            private $state: angular.ui.IStateService) {
            $anchorScroll.yOffset = 70;

            this.departments = nodeData.nodes;
            this.boxes = nodeData.boxes || [];
            this.nodeDetail = nodeData.details;

            //if (this.nodeDetail) {
            //    this.nodeDetail.isPrivate = false;
            //}
            this.buildState();
            this.university = {
                name: universityData.name,
                image: universityData.cover,
                logo: universityData.logo || itemThumbnailService.logo,
                id: universityData.id
            };
            this.createClassShow = this.state.emptyNode || this.state.withBoxes && $mdMedia('gt-xs');;// || l.state.emptyNodeAdmin;// l.departments.length === 0 && !l.topTree;
            this
                .createDepartmentShowButton = !$stateParams["id"] || this.nodeDetail.state === "closed";
//this.state.emptyNode || !this.state.withBoxes && $mdMedia('gt-xs');
        }


        goToSubLib(dep, $event: MouseEvent) {
            if (dep.state === 'closed' && (dep.userType === 'pending' || dep.userType === 'none')) {

                const confirm = this.$mdDialog.confirm()
                    .title(this.resManager.get('privateDepPopupTitle'))
                    .textContent(this.resManager.get('privateDepPopupContent'))
                    .targetEvent($event)
                    .ok(this.resManager.get('privateDepPopupRequestButton'))
                    .cancel(this.resManager.get('dialogCancel'));

                this.$mdDialog.show(confirm)
                    .then(() => {
                        this.libraryService.requestAccess(dep.id)
                            .then(() => {
                                this.$mdDialog.show(this.$mdDialog.alert()
                                    .title(this.resManager.get('privateDepPopupTitleOnSend'))
                                    .textContent(this.resManager.get('privateDepPopupContentOnSend'))
                                    .ok(this.resManager.get('dialogOk')));
                            });
                    });
                $event.preventDefault();
            }
        }


        buildState() {
            this.state = {
                topTree: this.$stateParams["id"] == null && this.departments.length,
                emptyTopTree: this.$stateParams["id"] == null && this.departments.length === 0,
                withBoxes: this.boxes.length,
                emptyNode: this.$stateParams["id"] != null && this.boxes.length === 0 && this.departments.length === 0
            };
        }
        departmentName;
        boxName;
        code;
        professor;
        createFirstBox(myform: angular.IFormController) {
            if (!myform.$valid) {
                return;
            }

            this.submitDisabled = true;
            this.libraryService.createDepartment(this.departmentName).then(response => {
                this.departments.push(response);

                this.libraryService.createClass(this.boxName, this.code, this.professor, response.id).then(response2 => {
                    this.showToasterService.showToaster(this.resManager.get('toasterCreateCourse'));
                    this.$location.url(response2.url);
                }, response2 => {
                    myform["depName"].$setValidity('server', false);
                    this.error = response2;
                }).finally(() => {
                    this.submitDisabled = false;
                });
            }, response => {
                myform["depName"].$setValidity('server', false);
                this.error = response;
            }).finally(() => {
                this.submitDisabled = false;
            });

        }

        settings;
        settingsOpen;
        renameNode(myform: angular.IFormController) {
            if (!myform.$valid) {
                return;
            }
            this.libraryService.updateSettings(this.settings.name, this.$stateParams["id"], this.settings.privacy).then(() => {
                this.nodeDetail.name = this.settings.name;
                this.nodeDetail.state = this.settings.privacy;
                this.settingsOpen = false;
            }, response => {
                myform["name"].$setValidity('server', false);
                this.error = response;
            }).finally(() => {
                this.submitDisabled = false;
            });

        }


        createBoxOn;
        settingsHtml;
        toggleSettings() {
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
            return this.ajaxService2.getHtml("/library/unisettings").then(response => {
                this.settingsHtml = response;
                this.$timeout(() => {
                    this.settings = {
                        name: this.nodeDetail.name
                    };
                    this.settingsOpen = true;
                });
            });

        }

        createOn;
        createDepartmentOn;
        openCreateBox() {
            this.createOn = true;
            this.settingsOpen = false;
            this.createDepartmentOn = false;
            if (this.createBoxOn) {
                this.createBoxOn = false;
            }
            else {
                this.createBoxOn = true;
            }
            this.$timeout(() => {
                this.$anchorScroll("createCourse");
            });
        }

        focusCreateDepartment;
        openCreateDepartment() {
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
            this.$timeout(() => {
                this.$anchorScroll("createDepartment");
            });
        }

        secondStep;
        createBox(myform: angular.IFormController) {
            if (!this.secondStep) {
                this.secondStep = true;
                return;
            }
            this.submitDisabled = true;
            this.libraryService.createClass(this.boxName, this.code, this.professor, this.$stateParams["id"]).then(response => {
                this.boxCancel();
                this.createClassShow = this.secondStep = false;
                //resetFiled(myform);
                this.showToasterService.showToaster(this.resManager.get('toasterCreateCourse'));
                this.$location.url(response.url);
            }, response => {
                myform["professor"].$setValidity('server', false);
                this.error = response;
            }).finally(() => {
                this.submitDisabled = false;
            });
        };

        createDepartment(myform: angular.IFormController) {
            this.submitDisabled = true;
            this.libraryService.createDepartment(this.departmentName, this.$stateParams["id"]).then(response => {


                this.showToasterService.showToaster(this.resManager.get('toasterCreateDepartment'));
                this.$state.go("departmentWithNode",
                    {
                        universityId: this.$stateParams["universityId"],
                        universityName: this.$stateParams["universityName"],
                        nodeName: response.name,
                        id: response.id
                    });
            }, response => {
                myform["name"].$setValidity('server', false);
                this.error = response;
            }).finally(() => {
                this.submitDisabled = false;
            });
        };

        canDelete(dep) {
            return this.user.isAdmin && dep.noDepartment === 0 && dep.noBoxes === 0;
        }

        boxNext() {
            this.secondStep = true;
        }
        boxCancel() {
            if (this.secondStep) {
                this.secondStep = false;
                return;
            }
            this.createBoxOn = false;
            this.createOn = false;
            this.resetFiled();
        }
        resetFiled(myform?: angular.IFormController) {
            this.departmentName = this.boxName = this.code = this.professor = '';
            if (myform) {
                myform.$setPristine();
                myform.$setUntouched();
            }
        }
        departmentCancel(myform: angular.IFormController) {
            this.resetFiled(myform);
            this.createDepartmentOn = false;
            this.createOn = false;
        }

        deleteDepartment(ev, department) {
            const confirm = this.$mdDialog.confirm()
                .title(this.resManager.get('deleteDepartment'))
                .targetEvent(ev)
                .ok(this.resManager.get('dialogOk'))
                .cancel(this.resManager.get('dialogCancel'));

            this.$mdDialog.show(confirm).then(() => {
                var index = this.departments.indexOf(department);
                this.departments.splice(index, 1);
                this.buildState();
                this.libraryService.deleteDepartment(department.id);
                this.createClassShow = this.departments.length === 0 && this.$stateParams["id"] != null;
            });
        }
    }
    angular.module("app.library").controller("Library", Library);

}





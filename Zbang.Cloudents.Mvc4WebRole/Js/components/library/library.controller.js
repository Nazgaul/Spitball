(function () {
    'use strict';
    angular.module('app.library').controller('Library', library);
    library.$inject = ['libraryService', '$stateParams', 'userDetailsFactory', 'nodeData', '$mdDialog',
        '$location', '$scope', 'resManager', 'universityData', 'itemThumbnailService', 'ajaxService2', '$timeout'];

    function library(libraryService, $stateParams, userDetailsFactory, nodeData, $mdDialog,
        $location, $scope, resManager, universityData, itemThumbnailService, ajaxService, $timeout) {

        var l = this, nodeId = $stateParams.id;
        l.departments = nodeData.nodes;
        l.boxes = nodeData.boxes || [];
        l.nodeDetail = nodeData.details;

        if (l.nodeDetail) {
            l.nodeDetail.isPrivate = false;
        }

        buildState();
        l.university = {
            name: universityData.name,
            image: universityData.cover,
            logo: universityData.logo || itemThumbnailService.logo
        };
        //l.universityName = userDetailsFactory.get().university.name;

        //l.topTree = nodeId == null;
        l.createDepartmentShow = !l.state.withBoxes; //userDetailsFactory.get().isAdmin && (l.topTree || l.boxes.length === 0);
        l.createClassShow = l.state.withBoxes || l.state.emptyNode;// || l.state.emptyNodeAdmin;// l.departments.length === 0 && !l.topTree;

        l.createDepartment = createDepartment;
        l.canDelete = canDelete;
        l.deleteDepartment = deleteDepartment;
        l.boxNext = boxNext;
        l.boxCancel = boxCancel;
        l.departmentCancel = departmentCancel;
        l.createBox = createBox;
        l.toggleSettings = toggleSettings;
        l.renameNode = renameNode;
        l.openCreateBox = openCreateBox;
        l.openCreateDepartment = openCreateDepartment;
        //l.createShow = createShow;

        l.createFirstBox = createFirstBox;
        l.goToSubLib = goToSubLib;
        l.submitDisabled = false;
        l.createDepartmenFocused = false;
        l.createClassFocused = false;

        function goToSubLib(dep, $event) {
            if (dep.state === 'closed' && (dep.userType === 'pending' || dep.userType === 'none')) {
                var confirm = $mdDialog.confirm()
                  .title(resManager.get('privateDepPopupTitle'))
                  .content(resManager.get('privateDepPopupContent'))
                  .targetEvent($event)
                   .ok(resManager.get('privateDepPopupRequestButton'))
                 .cancel(resManager.get('dialogCancel'));

                $mdDialog.show(confirm).then(function () {
                    libraryService.requestAccess(dep.id).then(function () {
                        $mdDialog.show($mdDialog.alert()
                            .title(resManager.get('privateDepPopupTitleOnSend'))
                            .content(resManager.get('privateDepPopupContentOnSend'))
                            .ok(resManager.get('dialogOk')));
                    });
                });
                $event.preventDefault();
            }
        }

        function buildState() {
            l.state = {
                topTree: nodeId == null && l.departments.length,
                emptyTopTree: nodeId == null && l.departments.length === 0,
               // withDepartmentAdmin: userDetailsFactory.get().isAdmin && l.departments.length,
                withBoxes: l.boxes.length,
                emptyNode: nodeId != null && l.boxes.length === 0 && l.departments.length === 0,
                emptyNodeAdmin: nodeId != null && l.boxes.length === 0 && l.departments.length === 0 && userDetailsFactory.get().isAdmin
            };
        }

        function createFirstBox(myform) {
            if (!myform.$valid) {
                return;
            }

            l.submitDisabled = true;
            libraryService.createDepartment(l.departmentName).then(function (response) {
                l.departments.push(response);

                libraryService.createClass(l.boxName, l.code, l.professor, response.id).then(function (response2) {
                    $scope.app.showToaster(resManager.get('toasterCreateCourse'));
                    $location.url(response2.url);
                }, function (response2) {
                    myform.depName.$setValidity('server', false);
                    l.error = response2;
                }).finally(function () {
                    l.submitDisabled = false;
                });
            }, function (response) {
                myform.depName.$setValidity('server', false);
                l.error = response;
            }).finally(function () {
                l.submitDisabled = false;
            });

        }

        //function createShow() {
        //    l.createOn = true;
        //    if (l.createClassShow) {
        //        openCreateBox();
        //    }
        //    if (l.createDepartmentShow) {
        //        openCreateDepartment();
        //    }

        //}

        function renameNode(myform) {
            if (!myform.$valid) {
                return;
            }
            libraryService.updateSettings(l.settings.name, nodeId, l.settings.privacy).then(function () {
                l.nodeDetail.name = l.settings.name;
                l.nodeDetail.state = l.settings.privacy;
                l.settingsOpen = false;
            }, function (response) {
                myform.name.$setValidity('server', false);
                l.error = response;
            }).finally(function () {
                l.submitDisabled = false;
            });

        }


        function toggleSettings() {
            l.createBoxOn = false;
            if (l.settingsHtml) {
                if (l.settingsOpen) {
                    l.settingsOpen = false;
                }
                else {
                    l.settingsOpen = true;
                }
                return;
            }
            return ajaxService.getHtml('/library/unisettings').then(function (response) {
                l.settingsHtml = response;
                $timeout(function () {
                    l.settings = {
                        name: l.nodeDetail.name
                    };
                    l.settingsOpen = true;
                });
            });

        }

        function openCreateBox() {
            l.createOn = true;
            l.settingsOpen = false;
            l.createDepartmentOn = false;
            if (l.createBoxOn) {
                l.createBoxOn = false;
            }
            else {
                l.createBoxOn = true;
            }
            l.focusCreateClass = true;
        }
        function openCreateDepartment() {
            l.createOn = true;
            if (l.createDepartmentOn) {
                l.createDepartmentOn = false;
            }
            else {
                l.createDepartmentOn = true;
            }
            l.createBoxOn = false;
            l.settingsOpen = false;
            l.focusCreateDepartment = true;

        }

        function createBox(myform) {
            if (!l.secondStep) {
                l.secondStep = true;
                return;
            }
            l.submitDisabled = true;
            libraryService.createClass(l.boxName, l.code, l.professor, nodeId).then(function (response) {
                boxCancel();
                l.createClassShow = l.secondStep = false;
                //resetFiled(myform);
                $scope.app.showToaster(resManager.get('toasterCreateCourse'));
                $location.url(response.url);
            }, function (response) {
                myform.professor.$setValidity('server', false);
                l.error = response;
            }).finally(function () {
                l.submitDisabled = false;
            });
        };

        function createDepartment(myform) {
            l.submitDisabled = true;
            libraryService.createDepartment(l.departmentName, nodeId).then(function (response) {
                response.state = l.nodeDetail ? l.nodeDetail.state : 'open';
                response.userType = l.nodeDetail ? l.nodeDetail.userType : 'none';
                l.departments.push(response);
                l.createDepartmentOn = false;
                l.createClassShow = false;
                resetFiled(myform);
                buildState();
                $scope.app.showToaster(resManager.get('toasterCreateDepartment'));
            }, function (response) {
                myform.name.$setValidity('server', false);
                l.error = response;
            }).finally(function () {
                l.submitDisabled = false;
            });
        };

        function canDelete(dep) {
            return userDetailsFactory.get().isAdmin && dep.noDepartment === 0 && dep.noBoxes === 0;
        }

        function boxNext() {
            l.secondStep = true;
        }
        function boxCancel() {
            if (l.secondStep) {
                l.secondStep = false;
                return;
            }
            l.createBoxOn = false;
            l.createOn = false;
            resetFiled();
        }
        function resetFiled(myform) {
            l.departmentName = l.boxName = l.code = l.professor = '';
            $scope.app.resetForm(myform);
        }
        function departmentCancel(myform) {
            resetFiled(myform);
            l.createDepartmentOn = false;
            l.createOn = false;
        }

        function deleteDepartment(ev, department) {
            var confirm = $mdDialog.confirm()
                  .title(resManager.get('deleteDepartment'))
                  .targetEvent(ev)
                   .ok(resManager.get('dialogOk'))
                 .cancel(resManager.get('dialogCancel'));

            $mdDialog.show(confirm).then(function () {
                var index = l.departments.indexOf(department);
                l.departments.splice(index, 1);
                buildState();
                libraryService.deleteDepartment(department.id);
                l.createClassShow = l.departments.length === 0 && nodeId != null;
            });
        }
    }
})();





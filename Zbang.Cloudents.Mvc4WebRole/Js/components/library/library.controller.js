(function () {
    angular.module('app.library').controller('Library', library);
    library.$inject = ['libraryService', '$stateParams', 'userDetailsFactory', 'nodeData', '$mdDialog',
        '$location', '$scope', 'resManager'];

    function library(libraryService, $stateParams, userDetailsFactory, nodeData, $mdDialog,
        $location, $scope, resManager) {




        var l = this;
        var nodeId = $stateParams.nodeId;

        l.departments = nodeData.nodes;
        l.boxes = nodeData.boxes || [];
        l.nodeDetail = nodeData.details;
        buildState();


        l.universityName = userDetailsFactory.get().university.name;
        //l.topTree = nodeId == null;
        l.createDepartmentShow = l.state.withDepartmentAdmin || l.state.emptyNodeAdmin; //userDetailsFactory.get().isAdmin && (l.topTree || l.boxes.length === 0);
        l.createClassShow = l.state.withBoxes || l.state.emptyNode || l.state.emptyNodeAdmin;// l.departments.length === 0 && !l.topTree;

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
        l.createShow = createShow;

        l.createFirstBox = createFirstBox;
        l.goToSubLib = goToSubLib;
        l.submitDisabled = false;

        function goToSubLib(dep, $event) {
            if (dep.state === 'closed' && (dep.userType === 'pending' || dep.userType === 'none')) {
                var confirm = $mdDialog.confirm()
                  .title('you need to request access to this node')
                  .targetEvent($event)
                   .ok(resManager.get('dialogOk'))
                 .cancel(resManager.get('dialogCancel'));

                $mdDialog.show(confirm).then(function () {
                    libraryService.requestAccess(dep.id).then(function() {
                        $mdDialog.show($mdDialog.alert()
                            .title('your request has being send')
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
                withDepartmentAdmin: userDetailsFactory.get().isAdmin && l.departments.length,
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

        function createShow() {
            l.createOn = true;
            if (l.createClassShow) {
                openCreateBox();
            }
            if (l.createDepartmentShow) {
                openCreateDepartment();
            }

        }

        function renameNode(myform) {
            if (!myform.$valid) {
                return;
            }

            l.submitDisabled = true;
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
            l.settings = {
                name: l.nodeDetail.name,
                privacy: l.nodeDetail.state
            };
            l.settingsOpen = true;
        }

        function openCreateBox() {
            l.settingsOpen = false;
            l.createBoxOn = true;
            l.focusCreateClass = true;
        }
        function openCreateDepartment() {
            l.createDepartmentOn = true;
            l.settingsOpen = false;
            l.focusCreateDepartment = true;

        }

        function createBox(myform) {
            l.submitDisabled = true;
            libraryService.createClass(l.boxName, l.code, l.professor, nodeId).then(function (response) {
                l.createClassShow = l.secondStep = false;
                resetFiled(myform);
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
                response.state = l.nodeDetail.state;
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





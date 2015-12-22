(function () {
    angular.module('app.library').controller('Library', library);
    library.$inject = ['libraryService', '$stateParams', 'user', 'nodeData', '$mdDialog', '$location', '$scope', 'resManager'];

    function library(libraryService, $stateParams, user, nodeData, $mdDialog, $location, $scope, resManager) {
        var l = this;
        var nodeId = $stateParams.nodeId;
        l.departments = nodeData.nodes;
        l.boxes = nodeData.boxes;
        l.nodeDetail = nodeData.details;

        l.universityName = user.university.name;

        l.createDepartmentShow = user.isAdmin && (nodeId == null || l.boxes.length === 0);
        l.createClassShow = l.departments.length === 0;
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

        l.submitDisabled = false;

        function renameNode(myform) {
            l.submitDisabled = true;
            libraryService.renameNode(l.settings.name, nodeId).then(function () {
                l.nodeDetail.name = l.settings.name;
                l.settingsOpen = false;
            }, function(response) {
                myform.name.$setValidity('server', false);
                l.error = response;
            }).finally(function() {
                l.submitDisabled = false;
            });

        }
        function toggleSettings() {
            l.settings = {
                name: l.nodeDetail.name
            };
            l.settingsOpen = true;
        }

        function openCreateBox() {
            l.settingsOpen = false;
            l.createBoxOn = true;
        }
        function openCreateDepartment() {
            l.createDepartmentOn = true;
            l.settingsOpen = false;
        }

        function createBox(myform) {
            l.submitDisabled = true;
            libraryService.createClass(l.boxName, l.code, l.professor, nodeId).then(function (response) {
                l.createClassShow = l.secondStep = false;
                resetFiled(myform);
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
                l.departments.push(response);
                l.createDepartmentOn = false;
                resetFiled(myform);
            }, function (response) {
                myform.name.$setValidity('server', false);
                l.error = response;
            }).finally(function () {
                l.submitDisabled = false;
            });
        };

        function canDelete(dep) {
            return user.isAdmin && dep.noDepartment === 0 && dep.noBoxes === 0;
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
            resetFiled();
        }
        function resetFiled(myform) {
            l.departmentName = l.boxName = l.code = l.professor = '';
            $scope.app.resetForm(myform);
        }
        function departmentCancel(myform) {
            resetFiled(myform);
            l.createDepartmentOn = false;
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
                libraryService.deleteDepartment(department.id);
            });
        }
    }
})();





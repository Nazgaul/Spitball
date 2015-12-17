(function () {
    angular.module('app.library').controller('Library', library);
    library.$inject = ['libraryService', '$stateParams', 'user', 'nodeData', '$mdDialog', '$location', '$scope'];

    function library(libraryService, $stateParams, user, nodeData, $mdDialog, $location, $scope) {
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

        function renameNode() {
            libraryService.renameNode(l.settings.name, nodeId).then(function () {
                l.nodeDetail.name = l.settings.name;
                l.settingsOpen = false;
            });

        }
        function toggleSettings() {
            l.settings = {
                name: l.nodeDetail.name
            };
            l.settingsOpen = true;
        }
        function createBox(myform) {

            libraryService.createClass(l.boxName, l.code, l.professor, nodeId).then(function (response) {
                //l.departments.push(response);
                l.createClassShow = l.secondStep = false;
                resetFiled(myform);
                $location.url(response.url);
            }, function (response) {
                myform.professor.$setValidity('server', false);
                l.error = response;
            });
        };

        function createDepartment(myform) {

            libraryService.createDepartment(l.departmentName, nodeId).then(function (response) {
                l.departments.push(response);
                l.createDepartmentOn = false;
                resetFiled(myform);
            }, function (response) {
                myform.name.$setValidity('server', false);
                l.error = response;
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
                  .title('Would you like to delete this department?')
                  //.textContent('All of the banks have agreed to forgive you your debts.')
                  .targetEvent(ev)
                  .ok('Ok')
                  .cancel('Cancel');

            $mdDialog.show(confirm).then(function () {
                var index = l.departments.indexOf(department);
                l.departments.splice(index, 1);
                libraryService.deleteDepartment(department.id);
            });
        }
    }
})();





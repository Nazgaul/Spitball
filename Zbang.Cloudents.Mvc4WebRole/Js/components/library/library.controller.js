(function () {
    angular.module('app.library').controller('Library', library);
    library.$inject = ['libraryService', '$stateParams', 'user', 'nodeData', '$mdDialog', '$location'];

    function library(libraryService, $stateParams, user, nodeData, $mdDialog, $location) {
        var l = this;
        var nodeId = $stateParams.nodeId;
        //libraryService.getDepartments($stateParams.nodeId).then(function (response) {
        l.departments = nodeData.nodes;
        l.boxes = nodeData.boxes;
        /*name: "10612"
parentUrl: "/library/"*/
        l.nodeDetail = nodeData.details;
        //});

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
        function createBox() {

            libraryService.createClass(l.boxName, l.code, l.professor, nodeId).then(function (response) {
                //l.departments.push(response);
                l.createClassShow =  l.secondStep = false;
                resetFiled();
                $location.url(response.url);
            }, function (response) {
                l.error = response;
            });
        };

        function createDepartment() {

            libraryService.createDepartment(l.departmentName, nodeId).then(function (response) {
                l.departments.push(response);
                l.createDepartmentOn = false;
                resetFiled();
            }, function (response) {
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
        function resetFiled() {
            l.departmentName = l.boxName = l.code = l.professor = '';
        }
        function departmentCancel() {
            resetFiled();
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





(function () {
    angular.module('app.library').controller('Library', library);
    library.$inject = ['libraryService', '$stateParams', 'userDetailsFactory', 'nodeData', '$mdDialog',
        '$location', '$scope', 'resManager'];

    function library(libraryService, $stateParams, userDetailsFactory, nodeData, $mdDialog,
        $location, $scope, resManager) {
        var l = this;
        var nodeId = $stateParams.nodeId;
        l.departments = nodeData.nodes;
        l.boxes = nodeData.boxes;
        l.nodeDetail = nodeData.details;

        l.universityName = userDetailsFactory.get().university.name;
        l.topTree = nodeId == null;
        l.createDepartmentShow = userDetailsFactory.get().isAdmin && (nodeId == null || l.boxes.length === 0);
        l.createClassShow = l.departments.length === 0 && nodeId != null;
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

        l.submitDisabled = false;

        function createShow() {
            l.createOn = true;
            //if (l.createClassShow && l.createDepartmentShow) {
            //    l.createOn = false;
            //    l.createBoxOn = true;
            //    l.createDepartmentOn = true;
            //    return;l.createBoxOn
            //}
            if (l.createClassShow) {
                openCreateBox();
            } 
            if (l.createDepartmentShow) {
                openCreateDepartment();
            }
            
        }
        //if (l.createDepartmentShow && l.createClassShow) {
        //    l.createOn = true;
        //    l.createDepartmentOn = true;
        //    l.createBoxOn = true;
        //}

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
                l.createClassShow = false;
                resetFiled(myform);
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
                libraryService.deleteDepartment(department.id);
                l.createClassShow = l.departments.length === 0 && nodeId != null;
            });
        }
    }
})();





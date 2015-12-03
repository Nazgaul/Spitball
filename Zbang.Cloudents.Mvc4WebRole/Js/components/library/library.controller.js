(function () {
    angular.module('app.library').controller('Library', library);
    library.$inject = ['libraryService', '$stateParams', 'user', 'nodeData'];

    function library(libraryService, $stateParams, user, nodeData) {
        var l = this;

        //libraryService.getDepartments($stateParams.nodeId).then(function (response) {
        l.departments = nodeData.nodes;
        l.boxes = nodeData.boxes;
        /*name: "10612"
parentUrl: "/library/"*/
        l.nodeDetail = nodeData.details;
        //});

        l.universityName = user.university.name;

        l.createDepartmentShow = user.isAdmin && ($stateParams.nodeId == null || l.boxes.length === 0);
        l.createClassShow = l.departments.length === 0;
        l.createDepartment = createDepartment;
        l.canDelete = canDelete;

        function createDepartment() {

            libraryService.createDepartment(l.departmentName, $stateParams.nodeId).then(function (response) {
                l.departments.push(response);
                l.createDepartmentOn = false;
                l.departmentName = '';
            }, function (response) {
                l.error = response;
            });
        };

        function canDelete(dep) {
            return user.isAdmin && dep.noDepartment === 0 && dep.noBoxes === 0;
        }
    }
})();





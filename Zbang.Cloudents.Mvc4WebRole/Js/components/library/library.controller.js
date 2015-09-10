(function () {
    angular.module('app.library').controller('Library', library);
    library.$inject = ['libraryService', 'userDetailsService', '$stateParams'];

    function library(libraryService, userDetailsService, $stateParams) {
        var l = this;

        libraryService.getDepartments($stateParams.nodeId).then(function (response) {
            l.departments = response.nodes;
            l.boxes = response.boxes;
            /*name: "10612"
parentUrl: "/library/"*/
            l.nodeDetail = response.details;
        });

        userDetailsService.getDetails().then(function (response) {
            l.universityName = response.universityName;
            
        });
    }
})();


(function () {
    angular.module('app.library').service('libraryService', library);
    library.$inject = ['ajaxService'];

    function library(ajaxservice) {
        var d = this;

        d.getDepartments = function (departmentId) {
            return ajaxservice.get('/library/nodes/', { section: departmentId }, 1800000);
        }
           
    }
})();
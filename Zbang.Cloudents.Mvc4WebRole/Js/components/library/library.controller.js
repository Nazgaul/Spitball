(function () {
    angular.module('app.library').controller('Library', library);
    library.$inject = ['libraryService','userDetailsService'];

    function library(libraryService, userDetailsService) {
        var l = this;

        libraryService.getDepartments().then(function (response) {
            l.departments = response.nodes;
        });

        userDetailsService.getDetails().then(function (response) {
            l.universityName = response.universityName;
            
        });
    }
})();


(function () {
    angular.module('app.library').service('libraryService', library);
    library.$inject = ['$q', 'ajaxService'];

    function library($q, ajaxservice) {
        var d = this;

        d.getDepartments = function (departmentId) {
            return ajaxservice.get('/library/Nodes/', { section: departmentId }, 1800000);
        }
           
    }
})();
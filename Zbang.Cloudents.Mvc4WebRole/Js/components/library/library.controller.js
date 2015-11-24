(function () {
    angular.module('app.library').controller('Library', library);
    library.$inject = ['libraryService', 'userDetails', '$stateParams'];

    function library(libraryService, userDetails, $stateParams) {
        var l = this;

        libraryService.getDepartments($stateParams.nodeId).then(function (response) {
            l.departments = response.nodes;
            l.boxes = response.boxes;
            /*name: "10612"
parentUrl: "/library/"*/
            l.nodeDetail = response.details;
        });

        userDetails.get().then(function (response) {
            l.universityName = response.university.name;
            
        });
    }
})();



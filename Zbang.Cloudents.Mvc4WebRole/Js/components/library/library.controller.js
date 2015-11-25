(function () {
    angular.module('app.library').controller('Library', library);
    library.$inject = ['libraryService', '$stateParams', 'user'];

    function library(libraryService, $stateParams, user) {
        var l = this;

        libraryService.getDepartments($stateParams.nodeId).then(function (response) {
            l.departments = response.nodes;
            l.boxes = response.boxes;
            /*name: "10612"
parentUrl: "/library/"*/
            l.nodeDetail = response.details;
        });

        l.universityName = user.university.name;

    }
})();





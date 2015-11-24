(function () {
    angular.module('app.library').service('libraryService', library);
    library.$inject = ['ajaxService'];

    function library(ajaxservice) {
        var d = this;

        d.getDepartments = function (departmentId) {
            return ajaxservice.get('/library/nodes/', { section: departmentId }, 1800000);
        }

        d.getUniversity = function(term) {
            return ajaxservice.get('/library/searchuniversity/', { term: term }, 1800000, true, true);
        }
        
        d.chooseUniversity = function (universityId, studentId) {
            return ajaxservice.post('/account/updateuniversity/', {
                universityId: universityId,
                studentId: studentId
            });
        }
        d.getUniversityByFriends = function(token) {
            return ajaxservice.get('/library/getuniversitybyfriends/', {
                token: token
            });

        }
    }
})();
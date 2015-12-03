(function () {
    angular.module('app.library').service('libraryService', library);
    library.$inject = ['ajaxService'];

    function library(ajaxservice) {
        var d = this;

        d.getDepartments = function (departmentId) {
            return ajaxservice.get('/library/nodes/', { section: departmentId });
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
        d.createUniversity = function(name,country) {
            return ajaxservice.post('/library/createuniversity/', {
                name: name,
                country:country
            });
        }

      
        d.createDepartment = function (name, nodeId) {
            return ajaxservice.post('/library/create/', {
                name: name,
                parentId: nodeId
            });
        }
        d.deleteDepartment = function(id) {
            return ajaxservice.post('/library/deletenode/', {
                id: id
            });
        }
       
        d.createClass = function(name, code, professor, nodeId) {
            return ajaxservice.post('/library/createbox/', {
                courseName: name,
                courseId: code,
                professor: professor,
                departmentId: nodeId
            });
        };
        /*[Required]
        public string Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string NewName { get; set; }*/
        d.renameNode = function(name,nodeId) {
            return ajaxservice.post('/library/renamenode/', {
                id: nodeId,
                newName: name
            });
        }
    }
})();
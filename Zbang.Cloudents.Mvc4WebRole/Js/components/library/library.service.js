'use strict';
(function () {
    angular.module('app.library').service('libraryService', library);
    library.$inject = ['ajaxService'];

    function library(ajaxservice) {
        var d = this;

        d.getDepartments = function (departmentId, universityId) {
            return ajaxservice.get('/university/nodes/', { section: departmentId, universityId: universityId });
        }

        d.getUniversity = function (term, page) {
            return ajaxservice.get('/university/searchuniversity/', { term: term, page: page });
        }

        d.chooseUniversity = function (universityId, studentId) {
            return ajaxservice.post('/account/updateuniversity/', {
                universityId: universityId,
                studentId: studentId
            });
        }
        d.createUniversity = function (name, country) {
            return ajaxservice.post('/university/createuniversity/', {
                name: name,
                country: country
            });
        }


        d.createDepartment = function (name, nodeId) {
            return ajaxservice.post('/university/create/', {
                name: name,
                parentId: nodeId
            });
        }
        d.deleteDepartment = function (id) {
            return ajaxservice.post('/university/deletenode/', {
                id: id
            });
        }

        d.createClass = function (name, code, professor, nodeId) {
            return ajaxservice.post('/university/createbox/', {
                courseName: name,
                courseId: code,
                professor: professor,
                departmentId: nodeId
            });
        };

        d.updateSettings = function (name, nodeId, settings) {
            return ajaxservice.post('/university/changesettings/', {
                id: nodeId,
                name: name,
                settings: settings
            });
        }
        d.requestAccess = function(nodeId) {
            return ajaxservice.post('/university/requestaccess/', {
                id: nodeId
            });
        }
    }
})();
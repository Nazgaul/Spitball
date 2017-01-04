var app;
(function (app) {
    'use strict';
    var Library = (function () {
        function Library(ajaxService) {
            this.ajaxService = ajaxService;
        }
        Library.prototype.getDepartments = function (departmentId, universityId) {
            return this.ajaxService.get("/university/nodes/", { section: departmentId, universityId: universityId });
        };
        Library.prototype.getAllDepartments = function () {
            return this.ajaxService.get("/university/allnodes/", null, "searchFirstBox");
        };
        Library.prototype.getUniversity = function (term, page) {
            return this.ajaxService.get("/university/search/", { term: term, page: page }, null, "uniSearch");
        };
        Library.prototype.chooseUniversity = function (universityId, studentId) {
            return this.ajaxService.post("/account/updateuniversity/", {
                universityId: universityId,
                studentId: studentId
            }, ["university", "searchFirstBox"]);
        };
        Library.prototype.createUniversity = function (name, country) {
            return this.ajaxService.post("/university/createuniversity/", {
                name: name,
                country: country
            }, "university");
        };
        Library.prototype.createDepartment = function (name, nodeId) {
            return this.ajaxService.post("/university/create/", {
                name: name,
                parentId: nodeId
            }, ["department", "searchFirstBox"]);
        };
        Library.prototype.deleteDepartment = function (id) {
            return this.ajaxService.post("/university/deletenode/", {
                id: id
            }, "department");
        };
        Library.prototype.createClass = function (name, code, professor, nodeId) {
            return this.ajaxService.post("/university/createbox/", {
                courseName: name,
                courseId: code,
                professor: professor,
                departmentId: nodeId
            }, ["department", "searchFirstBox"]);
        };
        ;
        Library.prototype.updateSettings = function (name, nodeId, settings) {
            return this.ajaxService.post("/university/changesettings/", {
                id: nodeId,
                name: name,
                settings: settings
            }, "department");
        };
        Library.prototype.requestAccess = function (nodeId) {
            return this.ajaxService.post("/university/requestaccess/", {
                id: nodeId
            });
        };
        return Library;
    }());
    Library.$inject = ["ajaxService2"];
    angular.module("app.library").service("libraryService", Library);
})(app || (app = {}));
//# sourceMappingURL=library.service.js.map
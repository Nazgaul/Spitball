var app;
(function (app) {
    'use strict';
    var Library = (function () {
        function Library(ajaxService) {
            this.ajaxService = ajaxService;
        }
        Library.prototype.getDepartments = function (departmentId, universityId, skipUrl) {
            return this.ajaxService.get("/university/nodes/", { section: departmentId, universityId: universityId, skipUrl: skipUrl });
        };
        Library.prototype.getUniversity = function (term, page) {
            return this.ajaxService.get("/university/search/", { term: term, page: page }, null, "uniSearch");
        };
        Library.prototype.chooseUniversity = function (universityId, studentId) {
            return this.ajaxService.post("/account/updateuniversity/", {
                universityId: universityId,
                studentId: studentId
            }, ["university"]);
        };
        Library.prototype.createUniversity = function (name, country) {
            return this.ajaxService.post("/university/createuniversity/", {
                name: name,
                country: country
            }, "university");
        };
        Library.prototype.createDepartment = function (name, nodeId, skipUrl) {
            return this.ajaxService.post("/university/create/", {
                name: name,
                parentId: nodeId,
                skipUrl: skipUrl
            }, "department");
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
            }, "department");
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
        Library.$inject = ["ajaxService2"];
        return Library;
    }());
    angular.module("app.library").service("libraryService", Library);
})(app || (app = {}));

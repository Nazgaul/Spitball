module app {
    'use strict';
    export interface ILibraryService {
        getDepartments(departmentId: Guid, universityId: number, skipUrl?: boolean): angular.IPromise<any>;
        getUniversity(term: string, page: number): angular.IPromise<any>;
        chooseUniversity(universityId: number, studentId: string): angular.IPromise<any>;
        createUniversity(name: string, country: string): angular.IPromise<any>;
        createDepartment(name: string, nodeId: Guid, skipUrl?: boolean): angular.IPromise<any>;
        deleteDepartment(id: Guid): angular.IPromise<any>;
        createClass(name: string, code: string, professor: string, nodeId: Guid): angular.IPromise<any>;
        updateSettings(name: string, nodeId: Guid, settings): angular.IPromise<any>;
        requestAccess(nodeId: Guid): angular.IPromise<any>;
    }

    class Library {
        static $inject = ["ajaxService2"];
        constructor(private ajaxService: IAjaxService2) {

        }
        getDepartments(departmentId: Guid, universityId: number, skipUrl?: boolean) {
            return this.ajaxService.get("/university/nodes/", { section: departmentId, universityId: universityId, skipUrl: skipUrl });
        }
        getUniversity(term, page) {
            return this.ajaxService.get("/university/search/", { term: term, page: page }, null, "uniSearch");
        }
        chooseUniversity(universityId, studentId) {
            return this.ajaxService.post("/account/updateuniversity/", {
                universityId: universityId,
                studentId: studentId
            }, ["university"]);
        }
        createUniversity(name, country) {
            return this.ajaxService.post("/university/createuniversity/", {
                name: name,
                country: country
            }, "university");
        }
        createDepartment(name, nodeId, skipUrl?: boolean) {
            return this.ajaxService.post("/university/create/", {
                name: name,
                parentId: nodeId,
                skipUrl: skipUrl
            }, "department");
        }
        deleteDepartment(id) {
            return this.ajaxService.post("/university/deletenode/", {
                id: id
            }, "department");
        }
        createClass(name, code, professor, nodeId) {
            return this.ajaxService.post("/university/createbox/", {
                courseName: name,
                courseId: code,
                professor: professor,
                departmentId: nodeId
            }, "department");
        };
        updateSettings(name, nodeId, settings) {
            return this.ajaxService.post("/university/changesettings/", {
                id: nodeId,
                name: name,
                settings: settings
            }, "department");
        }
        requestAccess(nodeId) {
            return this.ajaxService.post("/university/requestaccess/", {
                id: nodeId
            });
        }
    }
    angular.module("app.library").service("libraryService", Library);
}

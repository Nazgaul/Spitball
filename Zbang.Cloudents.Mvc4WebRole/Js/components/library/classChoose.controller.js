var app;
(function (app) {
    "use strict";
    var selectedCourses = [];
    var ClassChoose = (function () {
        function ClassChoose(searchService, $scope, libraryService, user) {
            this.searchService = searchService;
            this.$scope = $scope;
            this.libraryService = libraryService;
            this.user = user;
            this.showCreateClass = false;
            this.noresult = false;
            this.submitDisabled = false;
            this.create = {};
            this.departmentWithBoxes = {};
            this.departments = [];
            this.classSearch();
        }
        ClassChoose.prototype.classSearch = function () {
            var _this = this;
            this.noresult = false;
            this.noresult = false;
            var selectedCourseId = selectedCourses.map(function (f) { return f.id; });
            this.searchService.searchBoxSelect(this.term, 0)
                .then(function (response) {
                _this.departmentWithBoxes = {};
                for (var i = 0; i < response.length; i++) {
                    var box = response[i], department = box.department;
                    _this.departmentWithBoxes[department] = _this.departmentWithBoxes[department] || [];
                    if (selectedCourseId.indexOf(box.id) !== -1) {
                        box.selected = true;
                    }
                    _this.departmentWithBoxes[department].push(box);
                }
                if (!response.length) {
                    _this.noresult = true;
                }
            });
        };
        ClassChoose.prototype.choose = function (course) {
            selectedCourses.push(course);
            course.selected = true;
        };
        ClassChoose.prototype.goCreateClass = function () {
            var _this = this;
            this.showCreateClass = true;
            this.libraryService.getAllDepartments()
                .then(function (response) {
                _this.departments = response;
            });
        };
        ClassChoose.prototype.queryDepartments = function (text) {
            return this.departments;
        };
        ClassChoose.prototype.createClass = function (createBox) {
            var _this = this;
            if (createBox.$invalid) {
                return;
            }
            this.submitDisabled = true;
            var createObj = this.create;
            this.libraryService.createClass(createObj.name, createObj.number, createObj.professor, this.selectedDepartment.id)
                .then(function (response) {
                _this.departmentWithBoxes[_this.selectedDepartment.name] = _this.departmentWithBoxes[_this.selectedDepartment.name] || [];
                _this.departmentWithBoxes[_this.selectedDepartment.name].push({
                    id: response.id,
                    name: createObj.name,
                    courseCode: createObj.number,
                    professor: createObj.professor,
                    selected: true
                });
                angular.forEach(createObj, function (value, key) {
                    _this.create[key] = '';
                });
                _this.showCreateClass = false;
            }).catch(function (response) {
                createBox["name"].$setValidity('server', false);
                _this.create["error"] = response;
            }).finally(function () {
                _this.submitDisabled = false;
            });
        };
        ClassChoose.$inject = ["searchService", "$scope", "libraryService", "user"];
        return ClassChoose;
    }());
    angular.module("app.library").controller("ClassChoose", ClassChoose);
})(app || (app = {}));

var app;
(function (app) {
    "use strict";
    var ClassChoose = (function () {
        function ClassChoose(searchService, libraryService, $mdToast, $state, $mdDialog) {
            this.searchService = searchService;
            this.libraryService = libraryService;
            this.$mdToast = $mdToast;
            this.$state = $state;
            this.$mdDialog = $mdDialog;
            this.showCreateClass = false;
            this.selectedCourses = [];
            this.noresult = false;
            this.submitDisabled = false;
            this.create = {};
            this.departmentWithBoxes = {};
            this.departments = [];
            this.classSearch();
        }
        ClassChoose.prototype.classSearch = function () {
            this.noresult = false;
            this.noresult = false;
            var selectedCourseId = this.selectedCourses.map(function (f) { return f.id; });
            this.libraryService.getAllDepartments()
                .then(function (response) {
                console.log(response);
            });
        };
        ClassChoose.prototype.status = function (ev, course) {
            this.$mdDialog.show({
                templateUrl: "dialog.tmpl.html",
                parent: angular.element(document.body),
                targetEvent: ev,
                clickOutsideToClose: true,
                controller: this.chosenCourseController,
                locals: { courseData: course },
                fullscreen: true
            });
        };
        ClassChoose.prototype.chosenCourseController = function ($scope, $mdDialog, courseData) {
            $scope.courseData = courseData;
            $scope.close = function () {
                $mdDialog.hide();
            };
        };
        ClassChoose.prototype.choose = function (course) {
            var _this = this;
            this.selectedCourses.push(course);
            course.selected = true;
            var toasterContent = this.$mdToast.simple()
                .textContent("You have selected 3 classes")
                .hideDelay(0)
                .action("click here")
                .position("top center");
            toasterContent.toastClass("angular-animate");
            this.$mdToast.show(toasterContent).then(function () {
                _this.$state.go("dashboard");
            });
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
        ClassChoose.$inject = ["searchService", "libraryService", "$mdToast", "$state", "$mdDialog"];
        return ClassChoose;
    }());
    angular.module("app.library").controller("ClassChoose", ClassChoose);
})(app || (app = {}));

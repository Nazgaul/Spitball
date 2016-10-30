var app;
(function (app) {
    "use strict";
    var ClassChoose = (function () {
        function ClassChoose(searchService, libraryService, $mdToast, $state, $mdDialog, $filter, nodeData) {
            this.searchService = searchService;
            this.libraryService = libraryService;
            this.$mdToast = $mdToast;
            this.$state = $state;
            this.$mdDialog = $mdDialog;
            this.$filter = $filter;
            this.nodeData = nodeData;
            this.showCreateClass = false;
            this.selectedCourses = [];
            this.submitDisabled = false;
            this.create = {};
            this.data = [];
            this.classSearch();
        }
        ClassChoose.prototype.classSearch = function () {
            var _this = this;
            if (!this.term) {
                this.data = this.nodeData;
                return;
            }
            var filterDepartment = this.$filter("filter")(this.nodeData, this.term);
            this.data = this.$filter("filter")(filterDepartment, function (value) {
                value.boxes = _this.$filter("filter")(value.boxes, _this.term);
                return value;
            });
            if (!this.data.length) {
            }
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
            this.data = this.nodeData;
            this.term = '';
            this.showCreateClass = true;
            this.selectedDepartment = null;
            this.departmentName = '';
        };
        ClassChoose.prototype.queryDepartments = function (text) {
            var result = this.$filter("filter")(this.nodeData, text);
            return result;
        };
        ClassChoose.prototype.createClass = function (createBox) {
            var _this = this;
            this.submitDisabled = true;
            if (!this.selectedDepartment) {
                this.libraryService.createDepartment(this.departmentName, null, true)
                    .then(function (response) {
                    _this.nodeData.push(response);
                    _this.selectedDepartment = response;
                    _this.createClassCall(createBox);
                });
                return;
            }
            this.createClassCall(createBox);
        };
        ClassChoose.prototype.createClassCall = function (createBox) {
            var _this = this;
            var createObj = this.create;
            this.libraryService.createClass(createObj.name, createObj.number, createObj.professor, this.selectedDepartment.id)
                .then(function (response) {
                var department = _this.nodeData.find(function (f) { return f.id === _this.selectedDepartment.id; });
                var box = {
                    id: response.id,
                    name: createObj.name,
                    courseCode: createObj.number,
                    professor: createObj.professor
                };
                department.boxes = department.boxes || [];
                department.boxes.push(box);
                _this.selectedCourses.push(box);
                _this.selectedDepartment = null;
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
        ClassChoose.$inject = ["searchService", "libraryService", "$mdToast", "$state", "$mdDialog", "$filter", "nodeData"];
        return ClassChoose;
    }());
    angular.module("app.library").controller("ClassChoose", ClassChoose);
})(app || (app = {}));

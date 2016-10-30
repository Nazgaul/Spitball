var app;
(function (app) {
    "use strict";
    var ClassChoose = (function () {
        function ClassChoose(searchService, libraryService, $state, $mdDialog, $filter, nodeData, boxService, boxes, resManager) {
            var _this = this;
            this.searchService = searchService;
            this.libraryService = libraryService;
            this.$state = $state;
            this.$mdDialog = $mdDialog;
            this.$filter = $filter;
            this.nodeData = nodeData;
            this.boxService = boxService;
            this.boxes = boxes;
            this.resManager = resManager;
            this.showCreateClass = false;
            this.selectedCourses = [];
            this.submitDisabled = false;
            this.create = {};
            this.data = [];
            this.classSearch();
            var ids = [];
            angular.forEach(boxes, function (v) {
                ids.push(v.id);
                _this.selectedCourses.push({
                    id: v.id,
                    courseCode: v.courseCode,
                    name: v.name,
                    professor: v.professor
                });
            });
            angular.forEach(nodeData, function (v) {
                if (v.boxes) {
                    for (var i = v.boxes.length - 1; i >= 0; i--) {
                        var x = v.boxes[i];
                        if (ids.indexOf(x.id) !== -1) {
                            x["selected"] = true;
                        }
                    }
                }
            });
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
            this.boxService.follow(course.id);
            course["selected"] = true;
            this.selectedCourses.push(course);
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
        ClassChoose.prototype.requestAccess = function (department) {
            var _this = this;
            this.libraryService.requestAccess(department.id).then(function () {
                _this.$mdDialog.show(_this.$mdDialog.alert()
                    .title(_this.resManager.get('privateDepPopupTitleOnSend'))
                    .textContent(_this.resManager.get('privateDepPopupContentOnSend'))
                    .ok(_this.resManager.get('dialogOk')));
            });
        };
        ClassChoose.prototype.createClass = function (createBox) {
            var _this = this;
            if (createBox.$invalid) {
                return;
            }
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
        ClassChoose.$inject = ["searchService", "libraryService", "$state",
            "$mdDialog", "$filter", "nodeData", "boxService", "boxes", "resManager"];
        return ClassChoose;
    }());
    angular.module("app.library").controller("ClassChoose", ClassChoose);
})(app || (app = {}));

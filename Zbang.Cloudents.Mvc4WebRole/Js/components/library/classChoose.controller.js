var app;
(function (app) {
    "use strict";
    var selectedCourses = [];
    var ClassChoose = (function () {
        function ClassChoose(searchService, libraryService, $mdDialog, $filter, nodeData, boxService, boxes, resManager, $scope, $anchorScroll) {
            this.searchService = searchService;
            this.libraryService = libraryService;
            this.$mdDialog = $mdDialog;
            this.$filter = $filter;
            this.nodeData = nodeData;
            this.boxService = boxService;
            this.boxes = boxes;
            this.resManager = resManager;
            this.$scope = $scope;
            this.$anchorScroll = $anchorScroll;
            this.selectedCoursesView = [];
            this.data = [];
            this.classSearch();
            var ids = [];
            angular.forEach(boxes, function (v) {
                ids.push(v.id);
                selectedCourses.push({
                    id: v.id,
                    courseCode: v.courseCode,
                    name: v.name,
                    professor: v.professor,
                    department: null,
                    items: v.itemCount,
                    members: v.membersCount,
                    departmentId: v.departmentId
                });
            });
            this.selectedCoursesView = selectedCourses.slice();
            angular.forEach(nodeData, function (v) {
                if (v.boxes) {
                    var _loop_1 = function(i) {
                        var box = v.boxes[i];
                        box["twoLetter"] = box.name.substr(0, 2).toLowerCase();
                        if (ids.indexOf(box.id) !== -1) {
                            box["selected"] = true;
                            var course = selectedCourses.find(function (f) { return f.id === box.id; });
                            course.department = v.name;
                        }
                    };
                    for (var i = v.boxes.length - 1; i >= 0; i--) {
                        _loop_1(i);
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
            var boxes = this.$filter("filter")(angular.copy(this.nodeData), function (value) {
                value.boxes = _this.$filter("filter")(value.boxes, (function (v1) {
                    var lower = _this.term.toLowerCase();
                    if (v1.name.toLowerCase().indexOf(lower) !== -1) {
                        return v1;
                    }
                    if (v1.courseCode && v1.courseCode.toLowerCase().indexOf(lower) !== -1) {
                        return v1;
                    }
                    if (v1.professor && v1.professor.toLowerCase().indexOf(lower) !== -1) {
                        return v1;
                    }
                }));
                if (value.boxes && value.boxes.length) {
                    return value.boxes;
                }
                ;
            });
            var department = this.$filter("filter")(this.nodeData, { name: this.term });
            console.log(boxes, department);
            this.data = this.makeUnique(department.concat(boxes));
            this.$anchorScroll();
        };
        ClassChoose.prototype.makeUnique = function (array) {
            var flags = [];
            var output = [];
            var l = array.length;
            var i;
            for (i = 0; i < l; i++) {
                if (flags[array[i].id])
                    continue;
                flags[array[i].id] = true;
                output.push(array[i]);
            }
            return output;
        };
        ClassChoose.prototype.status = function (ev, course) {
            var _this = this;
            this.$mdDialog.show({
                templateUrl: "dialog.tmpl.html",
                targetEvent: ev,
                clickOutsideToClose: true,
                controller: 'classChooseUnfollowDialog',
                controllerAs: 'cd',
                locals: {
                    course: course,
                    courses: selectedCourses
                },
                fullscreen: false
            }).then(function (response) {
                _this.selectedCoursesView = selectedCourses.slice();
                var department = _this.nodeData.find(function (f) { return f.id === response.departmentId; });
                var box = department.boxes.find(function (f) { return f.id === response.id; });
                box["selected"] = false;
            });
        };
        ClassChoose.prototype.choose = function (course, department) {
            this.boxService.follow(course.id);
            this.$scope.$emit("refresh-boxes");
            course["selected"] = true;
            var pushOne = angular.extend({}, course, {
                department: department.name,
                departmentId: department.id
            });
            selectedCourses.push(pushOne);
        };
        ClassChoose.prototype.animationEnd = function (id) {
            var box = selectedCourses.find(function (f) { return f.id === id; });
            if (box) {
                this.selectedCoursesView.push(box);
            }
        };
        ClassChoose.prototype.goCreateClass = function (ev, department) {
            var _this = this;
            this.$mdDialog.show({
                templateUrl: "createClass.html",
                targetEvent: ev,
                clickOutsideToClose: true,
                controller: 'ClassChooseDialog',
                controllerAs: 'cd',
                locals: {
                    selectedDepartment: department,
                    nodeData: this.nodeData
                },
                fullscreen: false
            }).then(function (response) {
                _this.nodeData = response.nodeData;
                selectedCourses.push(response.box);
                _this.selectedCoursesView = selectedCourses.slice();
            });
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
        ClassChoose.$inject = ["searchService", "libraryService", "$mdDialog", "$filter",
            "nodeData", "boxService", "boxes", "resManager", "$scope", "$anchorScroll"];
        return ClassChoose;
    }());
    angular.module("app.library").controller("ClassChoose", ClassChoose);
})(app || (app = {}));

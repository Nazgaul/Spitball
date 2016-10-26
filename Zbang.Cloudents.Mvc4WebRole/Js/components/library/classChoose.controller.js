var app;
(function (app) {
    "use strict";
    var Steps;
    (function (Steps) {
        Steps[Steps["Start"] = 1] = "Start";
        Steps[Steps["SearchFirst"] = 2] = "SearchFirst";
        Steps[Steps["SearchFirstComplete"] = 3] = "SearchFirstComplete";
        Steps[Steps["SearchSecond"] = 4] = "SearchSecond";
        Steps[Steps["SearchSecondComplete"] = 5] = "SearchSecondComplete";
        Steps[Steps["SearchMore"] = 6] = "SearchMore";
        Steps[Steps["ChooseDepartment"] = 7] = "ChooseDepartment";
        Steps[Steps["CreateClass"] = 8] = "CreateClass";
    })(Steps || (Steps = {}));
    var currentNodeId;
    var ClassChoose = (function () {
        function ClassChoose(searchService, $scope, libraryService, user) {
            this.searchService = searchService;
            this.$scope = $scope;
            this.libraryService = libraryService;
            this.user = user;
            this.step = Steps.Start;
            this.selectedCourses = [];
            this.noresult = false;
            this.submitDisabled = false;
            this.create = {};
            searchService.searchBoxSelect('', 0)
                .then(function (response) {
                console.log(response);
            });
        }
        ClassChoose.prototype.classSearch = function () {
            var _this = this;
            var step = this.step;
            var formElement = this.$scope["classChooseFrom"];
            if (formElement.$invalid) {
                return;
            }
            this.noresult = false;
            if (this.term) {
                if (step === Steps.Start) {
                    this.step = Steps.SearchFirst;
                }
                var selectedCourseId = this.selectedCourses.map(function (f) { return f.id; });
                this.searchService.searchBox(this.term, 0)
                    .then(function (response) {
                    _this.searchResult = response.filter(function (f) { return selectedCourseId.indexOf(f.id) === -1; });
                    if (!response.length) {
                        _this.noresult = true;
                    }
                });
            }
            else {
                if (step === Steps.SearchFirst) {
                    this.step = Steps.Start;
                }
            }
        };
        ClassChoose.prototype.getRemainingElement = function () {
            return new Array(Math.max(0, 6 - this.selectedCourses.length));
        };
        ClassChoose.prototype.choose = function (course) {
            this.term = '';
            this.searchResult = [];
            this.selectedCourses.push(course);
            this.step = Math.min(this.step + 1, 5);
        };
        ClassChoose.prototype.selectDepartment = function (department) {
            var _this = this;
            this.term = '';
            department = department || {};
            currentNodeId = department.id;
            this.step = Steps.ChooseDepartment;
            this.libraryService.getDepartments(currentNodeId, this.user.university.id, true)
                .then(function (response) {
                if (response.nodes.length) {
                    _this.departmentlist = response.nodes;
                }
                else {
                    _this.selectedDepartment = department;
                    _this.step = Steps.CreateClass;
                }
            });
        };
        ClassChoose.prototype.backStep = function () {
            var numberOfSelectedCourses = this.selectedCourses.length;
            if (numberOfSelectedCourses > 2) {
                this.step = Steps.SearchMore;
                return;
            }
            if (numberOfSelectedCourses > 1) {
                this.step = Steps.SearchSecond;
                return;
            }
            this.step = Steps.SearchFirst;
        };
        ClassChoose.prototype.chooseMore = function () {
            if ([Steps.SearchFirstComplete, Steps.SearchSecondComplete].indexOf(this.step) !== -1) {
                this.step++;
            }
        };
        ClassChoose.prototype.createDepartment = function (newDepartment) {
            var _this = this;
            var name = this.create["department"];
            if (!name) {
                this.showCreateDepartment = false;
                return;
            }
            this.libraryService.createDepartment(name, currentNodeId, true)
                .then(function (response) {
                _this.selectedDepartment = {
                    id: response.id,
                    name: response.name
                };
                _this.step = Steps.CreateClass;
            }).catch(function (response) {
                newDepartment["name"].$setValidity('server', false);
                _this.create["error"] = response;
            });
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
                _this.selectedCourses.push({
                    id: response.id,
                    name: createObj.name,
                    courseCode: createObj.number,
                    professor: createObj.professor
                });
                angular.forEach(createObj, function (value, key) {
                    _this.create[key] = '';
                });
                if (_this.selectedCourses.length === 1) {
                    _this.step = Steps.SearchFirstComplete;
                }
                else {
                    _this.step = Steps.SearchSecondComplete;
                }
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

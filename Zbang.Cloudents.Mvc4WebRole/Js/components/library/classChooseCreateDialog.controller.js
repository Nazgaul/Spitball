var app;
(function (app) {
    "use strict";
    var ClassChooseDialog = (function () {
        function ClassChooseDialog($mdDialog, selectedDepartment, $filter, nodeData, libraryService) {
            this.$mdDialog = $mdDialog;
            this.selectedDepartment = selectedDepartment;
            this.$filter = $filter;
            this.nodeData = nodeData;
            this.libraryService = libraryService;
            this.submitDisabled = false;
            this.create = {};
            this.departmentName = selectedDepartment.name;
        }
        ClassChooseDialog.prototype.close = function () {
            this.$mdDialog.cancel();
        };
        ;
        ClassChooseDialog.prototype.createClass = function (createBox) {
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
                }).catch(function (response) {
                    createBox["name"].$setValidity('server', false);
                    _this.create["error"] = response;
                }).finally(function () {
                    _this.submitDisabled = false;
                });
                ;
                return;
            }
            this.createClassCall(createBox);
        };
        ClassChooseDialog.prototype.createClassCall = function (createBox) {
            var _this = this;
            var createObj = this.create;
            this.libraryService.createClass(createObj.name, createObj.number, createObj.professor, this.selectedDepartment.id)
                .then(function (response) {
                var department = _this.nodeData.find(function (f) { return f.id === _this.selectedDepartment.id; });
                var box = {
                    id: response.id,
                    name: createObj.name,
                    courseCode: createObj.number,
                    professor: createObj.professor,
                    items: 0,
                    members: 1,
                    selected: true
                };
                department.boxes = department.boxes || [];
                department.boxes.push(box);
                box["department"] = _this.selectedDepartment.name;
                angular.forEach(createObj, function (value, key) {
                    _this.create[key] = '';
                });
                _this.$mdDialog.hide({
                    nodeData: _this.nodeData,
                    box: box
                });
            }).catch(function (response) {
                createBox["name"].$setValidity('server', false);
                _this.create["error"] = response;
            }).finally(function () {
                _this.submitDisabled = false;
            });
        };
        ClassChooseDialog.prototype.queryDepartments = function (text) {
            var result = this.$filter("filter")(this.nodeData, text);
            return result;
        };
        ClassChooseDialog.$inject = ["$mdDialog", "selectedDepartment", "$filter", "nodeData", "libraryService"];
        return ClassChooseDialog;
    }());
    angular.module("app.library").controller("ClassChooseDialog", ClassChooseDialog);
})(app || (app = {}));

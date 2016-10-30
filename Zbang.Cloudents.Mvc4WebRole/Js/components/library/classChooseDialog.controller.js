var app;
(function (app) {
    var ClassChooseDialog = (function () {
        function ClassChooseDialog($mdDialog, $scope, courseData) {
            this.$mdDialog = $mdDialog;
            this.$scope = $scope;
            this.courseData = courseData;
        }
        ClassChooseDialog.prototype.close = function () {
            this.$mdDialog.hide();
        };
        ;
        ClassChooseDialog.prototype.remove = function () {
            var classChooseController = this.$scope["cc"];
            var index = classChooseController.selectedCourses.indexOf(this.courseData);
            classChooseController.selectedCourses.splice(index, 1);
        };
        ClassChooseDialog.$inject = ["$mdDialog", "$scope", "courseData"];
        return ClassChooseDialog;
    }());
    angular.module("app.library").controller("ClassChooseDialog", ClassChooseDialog);
})(app || (app = {}));

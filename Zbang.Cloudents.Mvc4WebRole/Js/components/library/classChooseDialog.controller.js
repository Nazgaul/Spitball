var app;
(function (app) {
    var ClassChooseDialog = (function () {
        function ClassChooseDialog($mdDialog, courseData, $scope) {
            this.$mdDialog = $mdDialog;
            this.courseData = courseData;
            this.$scope = $scope;
            console.log(courseData);
        }
        ClassChooseDialog.prototype.close = function () {
            this.$mdDialog.cancel();
        };
        ;
        ClassChooseDialog.prototype.remove = function () {
            var classChooseController = this.$scope["cc"];
            console.log(this.$scope["cc"]);
            var index = classChooseController.selectedCourses.indexOf(this.courseData);
            classChooseController.selectedCourses.splice(index, 1);
        };
        ClassChooseDialog.$inject = ["$mdDialog", "currentCourse", "$scope"];
        return ClassChooseDialog;
    }());
    angular.module("app.library").controller("ClassChooseDialog", ClassChooseDialog);
})(app || (app = {}));

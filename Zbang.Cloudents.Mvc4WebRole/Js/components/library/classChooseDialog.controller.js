var app;
(function (app) {
    var ClassChooseDialog = (function () {
        function ClassChooseDialog($mdDialog, course, courses, boxService) {
            this.$mdDialog = $mdDialog;
            this.course = course;
            this.courses = courses;
            this.boxService = boxService;
            this.confirmStep = false;
        }
        ClassChooseDialog.prototype.close = function () {
            this.confirmStep = false;
            this.$mdDialog.hide(this.courses);
        };
        ;
        ClassChooseDialog.prototype.remove = function () {
            this.boxService.unfollow(this.course.id);
            var index = this.courses.indexOf(this.course);
            this.courses.splice(index, 1);
            this.close();
        };
        ClassChooseDialog.$inject = ["$mdDialog", "course", "courses", "boxService"];
        return ClassChooseDialog;
    }());
    angular.module("app.library").controller("ClassChooseDialog", ClassChooseDialog);
})(app || (app = {}));

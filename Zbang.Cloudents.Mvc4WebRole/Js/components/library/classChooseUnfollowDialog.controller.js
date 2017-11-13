"use strict";
var app;
(function (app) {
    "use strict";
    var ClassChooseUnfollowDialog = (function () {
        function ClassChooseUnfollowDialog($mdDialog, course, courses, boxService) {
            this.$mdDialog = $mdDialog;
            this.course = course;
            this.courses = courses;
            this.boxService = boxService;
            this.confirmStep = false;
        }
        ClassChooseUnfollowDialog.prototype.close = function () {
            this.confirmStep = false;
            this.$mdDialog.cancel();
        };
        ;
        ClassChooseUnfollowDialog.prototype.remove = function () {
            this.boxService.unfollow(this.course.id);
            var index = this.courses.indexOf(this.course);
            this.courses.splice(index, 1);
            this.confirmStep = false;
            this.$mdDialog.hide(this.course);
        };
        return ClassChooseUnfollowDialog;
    }());
    ClassChooseUnfollowDialog.$inject = ["$mdDialog", "course", "courses", "boxService"];
    angular.module("app.library").controller("classChooseUnfollowDialog", ClassChooseUnfollowDialog);
})(app || (app = {}));
//# sourceMappingURL=classChooseUnfollowDialog.controller.js.map
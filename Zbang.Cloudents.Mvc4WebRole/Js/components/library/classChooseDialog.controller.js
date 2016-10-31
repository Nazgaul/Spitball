var app;
(function (app) {
    var index;
    var ClassChooseDialog = (function () {
        function ClassChooseDialog($mdDialog, course, courses, boxService) {
            this.$mdDialog = $mdDialog;
            this.course = course;
            this.courses = courses;
            this.boxService = boxService;
            index = this.courses.indexOf(course);
        }
        ClassChooseDialog.prototype.close = function () {
            this.$mdDialog.hide(this.courses);
        };
        ;
        ClassChooseDialog.prototype.remove = function () {
            this.courses.splice(index, 1);
            this.boxService.unfollow(this.course.id);
            this.course = this.courses[index];
            if (this.course) {
                this.prev();
            }
            if (!this.course) {
                this.close();
            }
        };
        ClassChooseDialog.prototype.next = function () {
            if (index < this.courses.length) {
                this.course = this.courses[++index];
            }
        };
        ClassChooseDialog.prototype.prev = function () {
            if (index > 0) {
                this.course = this.courses[--index];
            }
        };
        ClassChooseDialog.$inject = ["$mdDialog", "course", "courses", "boxService"];
        return ClassChooseDialog;
    }());
    angular.module("app.library").controller("ClassChooseDialog", ClassChooseDialog);
})(app || (app = {}));

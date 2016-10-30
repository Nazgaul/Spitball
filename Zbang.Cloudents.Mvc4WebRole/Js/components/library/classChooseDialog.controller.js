var app;
(function (app) {
    var index;
    var ClassChooseDialog = (function () {
        function ClassChooseDialog($mdDialog, currentCourseIndex, $scope) {
            this.$mdDialog = $mdDialog;
            this.currentCourseIndex = currentCourseIndex;
            this.$scope = $scope;
            index = currentCourseIndex;
            this.course = this.getSelectedCourses()[index];
        }
        ClassChooseDialog.prototype.close = function () {
            this.$mdDialog.cancel();
        };
        ;
        ClassChooseDialog.prototype.remove = function () {
            var classChooseController = this.$scope["cc"];
            classChooseController.selectedCourses.splice(index, 1);
        };
        ClassChooseDialog.prototype.next = function () {
            if (index < this.getSelectedCourses().length) {
                this.course = this.getSelectedCourses()[++index];
            }
        };
        ClassChooseDialog.prototype.prev = function () {
            if (index > 0) {
                this.course = this.getSelectedCourses()[--index];
            }
        };
        ClassChooseDialog.prototype.getSelectedCourses = function () {
            var classChooseController = this.$scope["cc"];
            return classChooseController.selectedCourses;
        };
        ClassChooseDialog.$inject = ["$mdDialog", "currentCourseIndex", "$scope"];
        return ClassChooseDialog;
    }());
    angular.module("app.library").controller("ClassChooseDialog", ClassChooseDialog);
})(app || (app = {}));

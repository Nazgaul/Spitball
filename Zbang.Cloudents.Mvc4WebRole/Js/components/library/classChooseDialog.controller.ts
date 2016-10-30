module app {
    class ClassChooseDialog {
        static $inject = ["$mdDialog", "$scope", "courseData"];
        constructor(private $mdDialog: angular.material.IDialogService,
            private $scope: angular.IScope,
            private courseData: any) {
        }

        close () {
            this.$mdDialog.hide();
        };

        remove() {
            const classChooseController = this.$scope["cc"];
            var index = classChooseController.selectedCourses.indexOf(this.courseData);
            classChooseController.selectedCourses.splice(index, 1);
        }

    }

    angular.module("app.library").controller("ClassChooseDialog", ClassChooseDialog);
}
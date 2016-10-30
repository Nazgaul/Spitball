module app {
    class ClassChooseDialog {
        static $inject = ["$mdDialog", "currentCourse", "$scope"];
        constructor(private $mdDialog: angular.material.IDialogService,

            private courseData: any,
            private $scope: angular.IScope) {
            console.log(courseData);
        }

        close() {
            this.$mdDialog.cancel();
        };

        remove() {
            //this.$mdDialog.hide(this.courseData);
            const classChooseController = this.$scope["cc"];
            console.log(this.$scope["cc"]);
            var index = classChooseController.selectedCourses.indexOf(this.courseData);
            classChooseController.selectedCourses.splice(index, 1);
        }

    }

    angular.module("app.library").controller("ClassChooseDialog", ClassChooseDialog);
}
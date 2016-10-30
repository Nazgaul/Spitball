module app {
    var index;
    class ClassChooseDialog {

        course;
        static $inject = ["$mdDialog", "currentCourseIndex", "$scope"];
        constructor(private $mdDialog: angular.material.IDialogService,

            private currentCourseIndex: number,
            private $scope: angular.IScope) {
            index = currentCourseIndex;
            this.course = this.getSelectedCourses()[index];

        }

        close() {
            this.$mdDialog.cancel();
        };

        remove() {
            //this.$mdDialog.hide(this.courseData);
            const classChooseController = this.$scope["cc"];
            classChooseController.selectedCourses.splice(index, 1);
        }
        next() {
            if (index < this.getSelectedCourses().length) {
                this.course = this.getSelectedCourses()[++index];
            }
        }
        prev() {
            if (index > 0) {
                this.course = this.getSelectedCourses()[--index];
            }
        }

        private getSelectedCourses() {
            const classChooseController = this.$scope["cc"];
            return classChooseController.selectedCourses;
        }

    }

    angular.module("app.library").controller("ClassChooseDialog", ClassChooseDialog);
}
module app {
    var index;
    class ClassChooseDialog {
        static $inject = ["$mdDialog", "course", "courses","boxService"];
        constructor(private $mdDialog: angular.material.IDialogService,
            private course: ISmallBoxClassChoose,
            private courses: Array<ISmallBoxClassChoose>,
            private boxService:IBoxService) {
            index = this.courses.indexOf(course);
        }

        close() {
            this.$mdDialog.hide(this.courses);
        };

        remove() {
            this.courses.splice(index, 1);
            this.boxService.unfollow(this.course.id);
            this.course = this.courses[index];
            if (this.course) {
                this.prev();
            }
            if (!this.course) {
                this.close();
            }
        }
        next() {
            if (index < this.courses.length) {
                this.course = this.courses[++index];
            }
        }
        prev() {
            if (index > 0) {
                this.course = this.courses[--index];
            }
        }

        //private getSelectedCourses() {
        //    const classChooseController = this.$scope["cc"];
        //    return classChooseController.selectedCourses;
        //}

    }

    angular.module("app.library").controller("ClassChooseDialog", ClassChooseDialog);
}
module app {
    class ClassChooseDialog {
        static $inject = ["$mdDialog", "course", "courses", "boxService"];
        constructor(private $mdDialog: angular.material.IDialogService,
            private course: ISmallBoxClassChoose,
            private courses: Array<ISmallBoxClassChoose>,
            private boxService: IBoxService) {
        }

        confirmStep = false;
        close() {
            this.confirmStep = false;
            this.$mdDialog.hide(this.courses);
        };
        
        remove() {
            this.boxService.unfollow(this.course.id);
            var index = this.courses.indexOf(this.course);
            this.courses.splice(index, 1); 
            this.close();
        }

    }

    angular.module("app.library").controller("ClassChooseDialog", ClassChooseDialog);
}
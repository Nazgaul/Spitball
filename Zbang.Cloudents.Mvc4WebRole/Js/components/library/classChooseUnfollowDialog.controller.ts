module app {
    "use strict";
    class ClassChooseUnfollowDialog {
        static $inject = ["$mdDialog", "course", "courses", "boxService"];
        constructor(private $mdDialog: angular.material.IDialogService,
            private course: ISelectedBoxClassChoose,
            private courses: Array<ISelectedBoxClassChoose>,
            private boxService: IBoxService) {
        }

        confirmStep = false;
        close() {
            this.confirmStep = false;
            this.$mdDialog.cancel();
        };
        
        remove() {
            this.boxService.unfollow(this.course.id);
            const index = this.courses.indexOf(this.course);
            this.courses.splice(index, 1); 
            this.confirmStep = false;
            this.$mdDialog.hide(this.course);
            
        }

    }

    angular.module("app.library").controller("classChooseUnfollowDialog", ClassChooseUnfollowDialog);
}
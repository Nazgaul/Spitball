
module app {
    "use strict";
    class AppController {
        static $inject = ["$scope","searchService"];
        tab: string;
        noResults = false;
        result = [];
        constructor(private $scope: angular.IScope, private searchService: IHelpService) {
        }
        formData: Object;
        class: string = "this is Class";
        yifat: number = 5;
        search() {
            if (this.formData) {
                Object.keys(this.formData).forEach(k => (!this.formData[k] && this.formData[k] !== undefined) && delete this.formData[k]);
                if (!Object.keys(this.formData).length) console.log("empty");
                console.log(this.formData);
                console.log(this.searchService.searchItems(this.formData));
            }
        }
    }

    angular.module("app").controller("AppController", AppController);
}
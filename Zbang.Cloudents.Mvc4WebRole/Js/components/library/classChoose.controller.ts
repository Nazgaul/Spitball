module app {
    "use strict";

    class ClassChoose {
        static $inject = ["searchService", "$scope"];
        stepTitle = 1;
        term;
        constructor(private searchService: ISearchService, private $scope: angular.IScope) {

        }
        classSearch() {
            const formElement: angular.IFormController = this.$scope["classChooseFrom"];

            if (formElement.$valid) {
                console.log('ss');
            }
            console.log('her')
            if (this.term) {
                this.stepTitle = 2;
            } else {
                this.stepTitle = 1;
            }
        }
    }

    angular.module("app.library").controller("ClassChoose", ClassChoose);
}
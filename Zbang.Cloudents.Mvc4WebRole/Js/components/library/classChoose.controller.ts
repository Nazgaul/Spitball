module app {
    "use strict";
    enum Steps {
        Start = 1,
        SearchFirst,
        SearchFirstComplete,
        SearchSecond,
        SearchSecondComplete,
        SearchMore,
        EmptyState,
        ChooseDepartment,
        CreateClass
    }
    class ClassChoose {
        static $inject = ["searchService", "$scope"];
        step = Steps.Start;
        term;
        constructor(private searchService: ISearchService, private $scope: angular.IScope) {

        }
        classSearch() {
            const formElement: angular.IFormController = this.$scope["classChooseFrom"];
            if (formElement.$invalid) {
                return;
            }
            if (this.term) {
                this.step = Steps.SearchFirst;
                this.searchService.searchBox(this.term, 0)
                    .then(response => {
                        console.log(response);
                    });
            } else {
                this.step = Steps.Start;
            }
        }
    }

    angular.module("app.library").controller("ClassChoose", ClassChoose);
}
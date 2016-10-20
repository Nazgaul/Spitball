module app {
    "use strict";
    enum Steps {
        Start = 1,
        SearchFirst = 2,
        SearchFirstComplete = 3,
        SearchSecond = 4,
        SearchSecondComplete = 5,
        SearchMore = 6,
        EmptyState,
        ChooseDepartment,
        CreateClass
    }
    class ClassChoose {
        static $inject = ["searchService", "$scope"];
        step = Steps.Start;
        term;
        searchResult;
        selectedCourses = [];
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
                        this.searchResult = response;
                    });
            } else {
                this.step = Steps.Start;
            }
        }
        getRemainingElement() {
            return new Array(Math.max(0, 6 - this.selectedCourses.length));
        }
        chose(course) {
            //TODO : ajax call
            this.selectedCourses.push(course);
            this.step = Steps.SearchFirstComplete;
        }
    }

    angular.module("app.library").controller("ClassChoose", ClassChoose);
}
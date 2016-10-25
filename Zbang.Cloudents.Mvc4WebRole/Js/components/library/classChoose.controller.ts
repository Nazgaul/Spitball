module app {
    "use strict";
    enum Steps {
        Start = 1,
        SearchFirst = 2,
        SearchFirstComplete = 3,
        SearchSecond = 4,
        SearchSecondComplete = 5,
        SearchMore = 6,
        //EmptyState,
        ChooseDepartment = 7,
        CreateClass = 8
    }
    class ClassChoose {
        static $inject = ["searchService", "$scope", "libraryService", "user"];
        step = Steps.Start;
        term;
        searchResult;
        selectedCourses = [];
        noresult = false;
        departmentlist;
        selectedDepartment;
        constructor(private searchService: ISearchService,
            private $scope: angular.IScope,
            private libraryService: ILibraryService,
            private user: IUserData) {

        }
        classSearch() {
            const step = this.step;
            const formElement: angular.IFormController = this.$scope["classChooseFrom"];
            if (formElement.$invalid) {
                return;
            }
            this.noresult = false;
            if (this.term) {

                if (step === Steps.Start) {
                    this.step = Steps.SearchFirst;
                }
                var selectedCourseId: Array<number> = this.selectedCourses.map(f => { return f.id });
                this.searchService.searchBox(this.term, 0)
                    .then((response: Array<any>) => {
                        this.searchResult = response.filter(f => selectedCourseId.indexOf(f.id) === -1);
                        if (!response.length) {
                            this.noresult = true;
                        }

                    });
            } else {
                if (step === Steps.SearchFirst) {
                    this.step = Steps.Start;
                }
            }
        }
        getRemainingElement() {
            return new Array(Math.max(0, 6 - this.selectedCourses.length));
        }
        choose(course) {
            //TODO : ajax call
            this.term = '';
            this.searchResult = [];
            this.selectedCourses.push(course);
            this.step = Math.min(this.step + 1, 5);
        }
        selectDepartment(department) {
            department = department || {};
            this.step = Steps.ChooseDepartment;
            this.libraryService.getDepartments(department.id, this.user.university.id, true)
                .then(response => {
                    console.log(response);
                    if (response.nodes.length) {
                        this.departmentlist = response.nodes;
                    } else {
                        this.selectedDepartment = department;
                        this.step = Steps.CreateClass;
                    }
                });
        }


        chooseMore() {
            if ([Steps.SearchFirstComplete, Steps.SearchSecondComplete].indexOf(this.step) !== -1) {
                this.step++;
            }

        }
    }

    angular.module("app.library").controller("ClassChoose", ClassChoose);
}
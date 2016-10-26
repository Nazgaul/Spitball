﻿module app {
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

    interface IBox {
        id: number;
        name: string;
        courseCode: string;
        professor: string;

    }
    interface ISelectedDepartment {
        name: string;
        id: Guid;
    }

    var currentNodeId;
    class ClassChoose {
        static $inject = ["searchService", "$scope", "libraryService", "user"];
        step = Steps.Start;
        term;
        searchResult;
        selectedCourses: Array<IBox> = [];
        noresult = false;
        departmentlist;
        selectedDepartment: ISelectedDepartment;
        submitDisabled = false;
        create = {};
        showCreateDepartment: boolean;
        constructor(private searchService: ISearchService,
            private $scope: angular.IScope,
            private libraryService: ILibraryService,
            private user: IUserData) {

            searchService.searchBoxSelect('', 0)
                .then(response => {
                    console.log(response);
                });

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
                var selectedCourseId = this.selectedCourses.map(f => { return f.id });
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
            this.term = '';
            department = department || {};
            currentNodeId = department.id;
            this.step = Steps.ChooseDepartment;
            this.libraryService.getDepartments(currentNodeId, this.user.university.id, true)
                .then(response => {
                    if (response.nodes.length) {
                        this.departmentlist = response.nodes;
                    } else {
                        this.selectedDepartment = department;
                        this.step = Steps.CreateClass;
                    }
                });
        }

        backStep() {
            const numberOfSelectedCourses = this.selectedCourses.length;
            if (numberOfSelectedCourses > 2) {
                this.step = Steps.SearchMore;
                return;
            }
            if (numberOfSelectedCourses > 1) {
                this.step = Steps.SearchSecond;
                return;
            }
            this.step = Steps.SearchFirst;
        }
        chooseMore() {
            if ([Steps.SearchFirstComplete, Steps.SearchSecondComplete].indexOf(this.step) !== -1) {
                this.step++;
            }

        }
        createDepartment(newDepartment: angular.IFormController) {
            const name = this.create["department"];
            if (!name) {
                this.showCreateDepartment = false;
                return;
            }
            this.libraryService.createDepartment(name, currentNodeId, true)
                .then(response => {
                    this.selectedDepartment = {
                        id: response.id,
                        name: response.name
                    }
                    this.step = Steps.CreateClass;
                }).catch(response => {
                    newDepartment["name"].$setValidity('server', false);
                    this.create["error"] = response;
                });
        }
        createClass(createBox: angular.IFormController) {
            if (createBox.$invalid) {
                return;
            }
            this.submitDisabled = true;
            const createObj: any = this.create;
            this.libraryService.createClass(createObj.name,
                createObj.number,
                createObj.professor,
                this.selectedDepartment.id)
                .then(response => {
                    this.selectedCourses.push({
                        id: response.id,
                        name: createObj.name,
                        courseCode: createObj.number,
                        professor: createObj.professor
                    });
                    angular.forEach(createObj,
                        (value, key) => {
                            this.create[key] = '';
                        });
                    if (this.selectedCourses.length === 1) {
                        this.step = Steps.SearchFirstComplete;
                    } else {
                        this.step = Steps.SearchSecondComplete;
                    }
                }).catch(response => {
                    createBox["name"].$setValidity('server', false);
                    this.create["error"] = response;
                }).finally(() => {
                    this.submitDisabled = false;
                });

        }
    }

    angular.module("app.library").controller("ClassChoose", ClassChoose);
}
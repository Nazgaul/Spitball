module app {
    "use strict";
    //enum Steps {
    //    Start = 1,
    //    SearchFirst = 2,
    //    SearchFirstComplete = 3,
    //    SearchSecond = 4,
    //    SearchSecondComplete = 5,
    //    SearchMore = 6,
    //    //EmptyState,
    //    ChooseDepartment = 7,
    //    CreateClass = 8
    //}

    interface IBox {
        id: number;
        name: string;
        courseCode: string;
        professor: string;

    }
    //interface ISelectedDepartment {
    //    name: string;
    //    id: Guid;
    //}

    //var currentNodeId;

    var selectedCourses: Array<IBox> = [];
    class ClassChoose {
        static $inject = ["searchService", "libraryService", "$mdToast", "$state","$mdDialog"];
        //step = Steps.Start;
        showCreateClass = false;
        //searchResult;

        noresult = false;
        //departmentlist;
        selectedDepartment: ISmallDepartment;
        submitDisabled = false;
        create = {};
        //showCreateDepartment: boolean;


        term;
        departmentWithBoxes = {};
        departments: Array<ISmallDepartment> = [];

        constructor(private searchService: ISearchService,
            private libraryService: ILibraryService,
            private $mdToast: angular.material.IToastService,
            private $state: angular.ui.IStateService,
            private $mdDialog: angular.material.IDialogService) {

            this.classSearch();
        }
        classSearch() {
            //const step = this.step;
            //const formElement: angular.IFormController = this.$scope["classChooseFrom"];
            //if (formElement.$invalid) {
            //    return;
            //}

            this.noresult = false;
            //if (this.term) {

            //if (step === Steps.Start) {
            //    this.step = Steps.SearchFirst;
            //}
            this.noresult = false;
            var selectedCourseId = selectedCourses.map(f => { return f.id });
            this.searchService.searchBoxSelect(this.term, 0)
                .then(response => {
                    this.departmentWithBoxes = {};
                    for (let i = 0; i < response.length; i++) {
                        const box = response[i], department = box.department;
                        this.departmentWithBoxes[department] = this.departmentWithBoxes[department] || [];
                        if (selectedCourseId.indexOf(box.id) !== -1) {
                            box.selected = true;
                        }
                        this.departmentWithBoxes[department].push(box);
                    }
                    if (!response.length) {
                        this.noresult = true;
                    }
                });

            //this.searchService.searchBox(this.term, 0)
            //    .then((response: Array<any>) => {
            //        this.searchResult = response.filter(f => selectedCourseId.indexOf(f.id) === -1);


            //    });
            //} else {
            //if (step === Steps.SearchFirst) {
            //    this.step = Steps.Start;
            //}
            //}
        }
        //getRemainingElement() {
        //    return new Array(Math.max(0, 6 - this.selectedCourses.length));
        //}
        status(ev) {
            this.$mdDialog.show({
                //controller: DialogController,
                templateUrl: "dialog.tmpl.html",
                parent: angular.element(document.body),
                targetEvent: ev,
                clickOutsideToClose: true,
                fullscreen: true // Only for -xs, -sm breakpoints.
            });
            //.then(function (answer) {
            //    //$scope.status = 'You said the information was "' + answer + '".';
            //}, function () {
            //    //$scope.status = 'You cancelled the dialog.';
            //});
        }
        choose(course) {
            //TODO : ajax call
            //this.term = '';
            //this.searchResult = [];
            selectedCourses.push(course);
            course.selected = true;
            const toasterContent = this.$mdToast.simple()
                .textContent("You have selected 3 classes")
                .hideDelay(0)
                .action("click here")
                .position("top");
            (toasterContent as any).toastClass("angular-animate md-center");
            this.$mdToast.show(toasterContent).then(() => {
                this.$state.go("dashboard");
            });
            //this.step = Math.min(this.step + 1, 5);
        }
        goCreateClass() {
            this.showCreateClass = true;
            this.libraryService.getAllDepartments()
                .then((response: Array<ISmallDepartment>) => {
                    this.departments = response;
                });
        }
        queryDepartments(text: String) {
            return this.departments;
        }
        //selectDepartment(department) {
        //    this.term = '';
        //    department = department || {};
        //    currentNodeId = department.id;
        //    //this.step = Steps.ChooseDepartment;
        //    this.libraryService.getDepartments(currentNodeId, this.user.university.id, true)
        //        .then(response => {
        //            if (response.nodes.length) {
        //                this.departmentlist = response.nodes;
        //            } else {
        //                this.selectedDepartment = department;
        //                //  this.step = Steps.CreateClass;
        //            }
        //        });
        //}

        //backStep() {
        //    const numberOfSelectedCourses = this.selectedCourses.length;
        //    if (numberOfSelectedCourses > 2) {
        //        //this.step = Steps.SearchMore;
        //        return;
        //    }
        //    if (numberOfSelectedCourses > 1) {
        //        // this.step = Steps.SearchSecond;
        //        return;
        //    }
        //    //this.step = Steps.SearchFirst;
        //}
        //chooseMore() {
        //    //if ([Steps.SearchFirstComplete, Steps.SearchSecondComplete].indexOf(this.step) !== -1) {
        //    //    this.step++;
        //    //}

        //}
        //createDepartment(newDepartment: angular.IFormController) {
        //    const name = this.create["department"];
        //    if (!name) {
        //        this.showCreateDepartment = false;
        //        return;
        //    }
        //    this.libraryService.createDepartment(name, currentNodeId, true)
        //        .then(response => {
        //            this.selectedDepartment = {
        //                id: response.id,
        //                name: response.name
        //            }
        //            // this.step = Steps.CreateClass;
        //        }).catch(response => {
        //            newDepartment["name"].$setValidity('server', false);
        //            this.create["error"] = response;
        //        });
        //}
        createClass(createBox: angular.IFormController) {
            //console.log(this.selectedDepartment);
            //return;
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
                    this.departmentWithBoxes[this.selectedDepartment.name] = this.departmentWithBoxes[this.selectedDepartment.name] || [];
                    this.departmentWithBoxes[this.selectedDepartment.name].push({
                        id: response.id,
                        name: createObj.name,
                        courseCode: createObj.number,
                        professor: createObj.professor,
                        selected: true
                    });
                    angular.forEach(createObj,
                        (value, key) => {
                            this.create[key] = '';
                        });
                    this.showCreateClass = false;
                    //if (this.selectedCourses.length === 1) {
                    //    //  this.step = Steps.SearchFirstComplete;
                    //} else {
                    //    //  this.step = Steps.SearchSecondComplete;
                    //}
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
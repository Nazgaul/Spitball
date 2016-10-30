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

    
    //interface ISelectedDepartment {
    //    name: string;
    //    id: Guid;
    //}

    //var currentNodeId;
    var allList: Array<ISmallDepartment> = [];
    class ClassChoose {
        static $inject = ["searchService", "libraryService", "$mdToast", "$state", "$mdDialog","$filter"];
        //step = Steps.Start;
        showCreateClass = false;
        selectedCourses: Array<ISmallBox> = [];
        //searchResult;

        noresult = false;
        //departmentlist;
        selectedDepartment: ISmallDepartment;
        submitDisabled = false;
        create = {};
        //showCreateDepartment: boolean;


        term;
        departmentWithBoxes = [];
        departments: Array<ISmallDepartment> = [];

        constructor(private searchService: ISearchService,
            private libraryService: ILibraryService,
            private $mdToast: angular.material.IToastService,
            private $state: angular.ui.IStateService,
            private $mdDialog: angular.material.IDialogService,
            private $filter: angular.IFilterService) {

            this.libraryService.getAllDepartments()
                .then(response => {
                    allList = response;
                    this.classSearch();
                });
            
        }
        classSearch() {
            //const step = this.step;
            //const formElement: angular.IFormController = this.$scope["classChooseFrom"];
            //if (formElement.$invalid) {
            //    return;
            //}
           
            
            this.noresult = false;
            if (!this.term) {
                this.departmentWithBoxes = allList;
                return;
            }
            var x = this.$filter('filter')(allList, this.term);
            this.departmentWithBoxes = this.$filter("filter")(x, (value, index, array) => {

                var retVal = this.$filter('filter')(value.boxes, this.term);
                return retVal;// && value.name.indexOf(this.term) !== -1;
            });

            //  currentPost.replies = $filter('orderBy')(currentPost.replies, 'creationTime', false);
            


            //if (step === Steps.Start) {
            //    this.step = Steps.SearchFirst;
            //}
            //this.noresult = false;
            //var selectedCourseId = this.selectedCourses.map(f => { return f.id });

            //        allList = response;
            //        console.log(response);
            //        this.departmentWithBoxes = response;

            //    });

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
        status(ev, course) {
            this.$mdDialog.show({
                templateUrl: "dialog.tmpl.html",
                parent: angular.element(document.body),
                targetEvent: ev,
                clickOutsideToClose: true,
                controller: this.chosenCourseController,
                locals: { courseData: course },
                fullscreen: true // Only for -xs, -sm breakpoints.
            });
            //.then(function (answer) {
            //    //$scope.status = 'You said the information was "' + answer + '".';
            //}, function () {
            //    //$scope.status = 'You cancelled the dialog.';
            //});
        }

        chosenCourseController($scope, $mdDialog, courseData) {
            $scope.courseData = courseData;

            $scope.close = function () {
                $mdDialog.hide();
            };
        }

        choose(course) {
            //TODO : ajax call
            //this.term = '';
            //this.searchResult = [];
            this.selectedCourses.push(course);
            course.selected = true;
            const toasterContent = this.$mdToast.simple()
                .textContent("You have selected 3 classes")
                .hideDelay(0)
                .action("click here")
                .position("top center");
            (toasterContent as any).toastClass("angular-animate");
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
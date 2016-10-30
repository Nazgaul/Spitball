module app {
    "use strict";

    //var allList: Array<ISmallDepartment> = [];

    class ClassChoose {
        static $inject = ["searchService", "libraryService", "$mdToast", "$state", "$mdDialog", "$filter", "nodeData"];
        showCreateClass = false;
        selectedCourses: Array<ISmallBox> = [];

        selectedDepartment: ISmallDepartment;
        submitDisabled = false;
        create = {};
        term;
        data: Array<ISmallDepartment> = [];
        //departments: Array<ISmallDepartment> = [];

        constructor(private searchService: ISearchService,
            private libraryService: ILibraryService,
            private $mdToast: angular.material.IToastService,
            private $state: angular.ui.IStateService,
            private $mdDialog: angular.material.IDialogService,
            private $filter: angular.IFilterService,
            private nodeData: Array<ISmallDepartment>) {

            this.classSearch();

            //this.libraryService.getAllDepartments()
            //    .then(response => {
            //        allList = response;
            //        this.classSearch();
            //    });

        }
        classSearch() {
            if (!this.term) {
                this.data = this.nodeData;
                return;
            }
            const filterDepartment = this.$filter("filter")(this.nodeData, this.term);
            this.data = this.$filter("filter")(filterDepartment, (value) => {
                value.boxes = this.$filter("filter")(value.boxes, this.term);
                return value;// && value.name.indexOf(this.term) !== -1;
            });
            if (!this.data.length) {
            }
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
        }


        departmentName: string;
        goCreateClass() {
            this.data = this.nodeData;
            this.term = '';
            this.showCreateClass = true;
            this.selectedDepartment = null;
            this.departmentName = '';
        }
        queryDepartments(text: String) {
            const result = this.$filter("filter")(this.nodeData, text);//.map(m => { return { name: m.name, id: m.id } });
            return result;
        }
        //selectDepartment(department) {
        //    console.log(department);
        //}
        createClass(createBox: angular.IFormController) {
            //if (!this.selectDepartment) {
            //}
            //if (createBox.$invalid) {
            //    return;
            //}
            this.submitDisabled = true;
            if (!this.selectedDepartment) {
                //cc.departmentName
                this.libraryService.createDepartment(this.departmentName, null, true)
                    .then(response => {
                        this.nodeData.push(response);
                        this.selectedDepartment = response;
                        this.createClassCall(createBox);
                    });
                return;
            }
            this.createClassCall(createBox);

        }

        private createClassCall(createBox: angular.IFormController) {
            const createObj: any = this.create;
            this.libraryService.createClass(createObj.name,
                createObj.number,
                createObj.professor,
                this.selectedDepartment.id)
                .then(response => {
                    const department = this.nodeData.find(f => f.id === this.selectedDepartment.id);
                    const box = {
                        id: response.id,
                        name: createObj.name,
                        courseCode: createObj.number,
                        professor: createObj.professor
                    };
                    department.boxes = department.boxes || [];
                    department.boxes.push(box);
                    this.selectedCourses.push(box);
                    this.selectedDepartment = null;
                    angular.forEach(createObj,
                        (value, key) => {
                            this.create[key] = '';
                        });
                    this.showCreateClass = false;
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
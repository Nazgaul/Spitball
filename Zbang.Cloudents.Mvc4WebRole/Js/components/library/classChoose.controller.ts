module app {
    "use strict";



    //var allList: Array<ISmallDepartment> = [];

    class ClassChoose {
        //static $inject = ["searchService", "libraryService", "$mdToast", "$state", "$mdDialog", "$scope"];
        static $inject = ["searchService", "libraryService", "$mdToast", "$state", "$mdDialog", "$filter", "nodeData", "boxService", "boxes", "$scope"];
        showCreateClass = false;
        selectedCourses: Array<ISmallBox> = [];

        selectedDepartment: ISmallDepartment;
        submitDisabled = false;
        create = {};
        term;
        data: Array<ISmallDepartment> = [];

        constructor(private searchService: ISearchService,
            private libraryService: ILibraryService,
            private $mdToast: angular.material.IToastService,
            private $state: angular.ui.IStateService,
            private $mdDialog: angular.material.IDialogService,
            private $filter: angular.IFilterService,
            private nodeData: Array<ISmallDepartment>,
            private boxService: IBoxService,
            private boxes: any,
            private $scope: angular.IScope) {
            this.classSearch();

            var ids = [];
            angular.forEach(boxes,

                (v) => {
                    ids.push(v.id);
                    this.selectedCourses.push({
                        id: v.id,
                        courseCode: v.courseCode,
                        name: v.name,
                        professor: v.professor
                    });
                });
            angular.forEach(nodeData,
                v => {
                    if (v.boxes) {
                        for (let i = v.boxes.length - 1; i >= 0; i--) {
                            const x = v.boxes[i];
                            if (ids.indexOf(x.id) !== -1) {
                                //v.boxes.splice(i, 1);
                                x["selected"] = true;
                            }
                        }
                    }
                });


        }
        classSearch() {
            if (!this.term) {
                this.data = this.nodeData;
                return;
            }
            const filterDepartment = this.$filter("filter")(this.nodeData, this.term);
            this.data = this.$filter("filter")(filterDepartment, (value) => {
                value.boxes = this.$filter("filter")(value.boxes, this.term);
                return value;
            });

        }

        status(ev, course) {
            this.$mdDialog.show({
                templateUrl: "dialog.tmpl.html",
                parent: angular.element(document.body),
                targetEvent: ev,
                clickOutsideToClose: true,
                controller: 'ClassChooseDialog',
                controllerAs: 'cd',
                locals: { courseData: course },
                scope: this.$scope,
                fullscreen: true // Only for -xs, -sm breakpoints.
            });

        }

        //chosenCourseController($scope, $mdDialog, courseData) {
        //    $scope.courseData = courseData;

        //    $scope.close = function () {
        //        $mdDialog.hide();
        //    };

        //    $scope.remove = function () {
        //        var index = this.selectedCourses.indexOf(courseData);
        //        this.selectedCourses.splice(index, 1);
        //    }
        //}

        choose(course) {
            this.boxService.follow(course.id);
            course["selected"] = true;
            this.selectedCourses.push(course);
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

        createClass(createBox: angular.IFormController) {
            if (createBox.$invalid) {
                return;
            }
            this.submitDisabled = true;
            if (!this.selectedDepartment) {
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
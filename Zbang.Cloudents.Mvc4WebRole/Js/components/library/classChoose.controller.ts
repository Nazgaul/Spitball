module app {
    "use strict";


    export interface ISmallBoxClassChoose extends ISmallBox {
        department: string;
        departmentId: Guid;
    }
    //var allList: Array<ISmallDepartment> = [];

    class ClassChoose {
        static $inject = ["searchService", "libraryService",    /*"$scope",*/
            "$mdDialog", "$filter", "nodeData", "boxService", "boxes", "resManager", "$scope", "$timeout"];
        //static $inject = ["searchService", "libraryService", "$mdToast", "$state", "$mdDialog", "$filter", "nodeData", "boxService", "boxes", "$scope"];
        showCreateClass = false;
        selectedCourses: Array<ISmallBoxClassChoose> = [];

        selectedDepartment: ISmallDepartment;
        submitDisabled = false;
        create = {};
        term;
        data: Array<ISmallDepartment> = [];

        constructor(private searchService: ISearchService,
            private libraryService: ILibraryService,
            private $mdDialog: angular.material.IDialogService,
            private $filter: angular.IFilterService,
            private nodeData: Array<ISmallDepartment>,
            private boxService: IBoxService,
            private boxes: any,
            private resManager: IResManager,
            private $scope,
            private $timeout: angular.ITimeoutService) {

            this.classSearch();

            var ids = [];
            angular.forEach(boxes,

                (v) => {
                    ids.push(v.id);
                    this.selectedCourses.push({
                        id: v.id,
                        courseCode: v.courseCode,
                        name: v.name,
                        professor: v.professor,
                        department: null,
                        items: v.itemCount,
                        members: v.membersCount,
                        departmentId: v.departmentId
                    });
                });
            angular.forEach(nodeData,
                v => {
                    if (v.boxes) {
                        for (let i = v.boxes.length - 1; i >= 0; i--) {
                            const x = v.boxes[i];
                            if (ids.indexOf(x.id) !== -1) {
                                x["selected"] = true;
                                const course = this.selectedCourses.find(f => f.id === x.id);
                                course.department = v.name;
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
            const data = angular.copy(this.nodeData);
            const filterDepartment = this.$filter("filter")(data, this.term);
            const departments = this.$filter("filter")(filterDepartment, (value) => {
                value.boxes = this.$filter("filter")(value.boxes, this.term);
                return value;
            });
            this.data = departments.filter(f => f.boxes.length > 0);


        }

        status(ev, course) {
            this.$mdDialog.show({
                templateUrl: "dialog.tmpl.html",
                targetEvent: ev,
                clickOutsideToClose: true,
                controller: 'ClassChooseDialog',
                controllerAs: 'cd',
                locals: {
                    course: course,
                    courses: this.selectedCourses
                },
                //scope: this.$scope,
                fullscreen: true // Only for -xs, -sm breakpoints.
            });
            //.then((response) => {
            //    console.log(response);
            //});

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

        choose(course, department) {
            this.$timeout(() => {
                this.boxService.follow(course.id);
                this.$scope.$emit("refresh-boxes");
                course["selected"] = true;

                const pushOne = angular.extend({},
                    course,
                    {
                        department: department.name,
                        departmentId: department.id
                    });
                this.selectedCourses.push(
                    pushOne);
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
        requestAccess(department) {
            this.libraryService.requestAccess(department.id).then(() => {
                this.$mdDialog.show(this.$mdDialog.alert()
                    .title(this.resManager.get('privateDepPopupTitleOnSend'))
                    .textContent(this.resManager.get('privateDepPopupContentOnSend'))
                    .ok(this.resManager.get('dialogOk')));
            });
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
                        professor: createObj.professor,
                        items: 0,
                        members: 1
                    };
                    department.boxes = department.boxes || [];
                    department.boxes.push(box);
                    box["department"] = this.selectedDepartment.name;
                    this.selectedCourses.push((box as ISmallBoxClassChoose));
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
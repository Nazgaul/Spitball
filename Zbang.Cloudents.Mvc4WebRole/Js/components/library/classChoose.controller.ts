module app {
    "use strict";


    export interface ISmallBoxClassChoose extends ISmallBox {
        department: string;
        departmentId: Guid;
    }
    //var allList: Array<ISmallDepartment> = [];

    class ClassChoose {
        static $inject = ["searchService", "libraryService", "$mdDialog", "$filter",
            "nodeData", "boxService", "boxes", "resManager", "$scope", "$anchorScroll"];
        //showCreateClass = false;
        selectedCourses: Array<ISmallBoxClassChoose> = [];

        //selectedDepartment: ISmallDepartment;
        //submitDisabled = false;
        //create = {};
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
            private $scope: angular.IScope,
            private $anchorScroll: angular.IAnchorScrollService
        ) {

            this.classSearch();
            //console.log(this.nodeData.map(m => m.boxes))
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
            //const data = angular.copy(this.nodeData);
            //console.log(data.length);

            // we manipulate the boxes inorder to remove them
            const boxes = this.$filter("filter")(angular.copy(this.nodeData), (value) => {
                //value.boxes = this.$filter("filter")(value.boxes, this.term);
                value.boxes = this.$filter("filter")(value.boxes, {
                    name: this.term,
                    courseCode: this.term,
                    professor: this.term
                });
                if (value.boxes && value.boxes.length) {
                    return value.boxes;
                }

            });
            const department = this.$filter("filter")(this.nodeData, { name: this.term });
            this.data = this.makeUnique(department.concat(boxes));
            this.$anchorScroll(); //scroll to the top

        }
        private makeUnique(array) {
            const flags = [];
            const output = [];
            const l = array.length;
            let i: number;
            for (i = 0; i < l; i++) {
                if (flags[array[i].id]) continue;
                flags[array[i].id] = true;
                output.push(array[i]);
            }
            return output;
        }

        status(ev, course) {
            this.$mdDialog.show({
                templateUrl: "dialog.tmpl.html",
                targetEvent: ev,
                clickOutsideToClose: true,
                controller: 'classChooseUnfollowDialog',
                controllerAs: 'cd',
                locals: {
                    course: course,
                    courses: this.selectedCourses
                },
                //scope: this.$scope,
                fullscreen: false // Only for -xs, -sm breakpoints.
            }).then((response: ISmallBoxClassChoose) => {
                var department = this.nodeData.find(f => f.id === response.departmentId);
                var box = department.boxes.find(f => f.id === response.id);
                box["selected"] = false;
            });

        }



        choose(course, department) {
            //this.$timeout(() => {
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
            //});

        }
        //departmentName: string;
        goCreateClass(ev, department) {
            this.$mdDialog.show({
                templateUrl: "createClass.html",
                targetEvent: ev,
                clickOutsideToClose: true,
                controller: 'ClassChooseDialog',
                controllerAs: 'cd',
                locals: {
                    selectedDepartment: department,
                    nodeData: this.nodeData
                    //    courses: this.selectedCourses
                },
                //scope: this.$scope,
                fullscreen: true // Only for -xs, -sm breakpoints.
            }).then((response) => {
                this.nodeData = response.nodeData;
                this.selectedCourses.push((response.box as ISmallBoxClassChoose));
            });

            //this.data = this.nodeData;
            //this.term = '';
            ////this.showCreateClass = true;
            //this.selectedDepartment = null;
            //this.departmentName = '';
        }

        requestAccess(department) {
            this.libraryService.requestAccess(department.id).then(() => {
                this.$mdDialog.show(this.$mdDialog.alert()
                    .title(this.resManager.get('privateDepPopupTitleOnSend'))
                    .textContent(this.resManager.get('privateDepPopupContentOnSend'))
                    .ok(this.resManager.get('dialogOk')));
            });
        }


    }

    angular.module("app.library").controller("ClassChoose", ClassChoose);
}
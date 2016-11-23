﻿module app {
    "use strict";


    export interface ISelectedBoxClassChoose extends ISmallBox {
        department: string;
        departmentId: Guid;
        // animate: boolean;
    }

    class ClassChoose {
        static $inject = ["searchService", "libraryService", "$mdDialog", "$filter",
            "nodeData", "boxService", "boxes", "resManager", "$scope", "$anchorScroll"];
        selectedCoursesView: Array<ISelectedBoxClassChoose> = [];
        //create = {};
        term;
        //limit = 0;
        data: Array<ISmallDepartment> = [];
        private selectedCourses: Array<ISelectedBoxClassChoose> = [];
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
            var ids = [];
            angular.forEach(boxes,

                (v) => {
                    ids.push(v.id);
                    this.selectedCourses.push({
                        id: v.id,
                        courseId: v.courseCode,
                        name: v.name,
                        professor: v.professor,
                        department: null,
                        items: v.itemCount,
                        members: v.membersCount,
                        departmentId: v.departmentId
                    });
                });
            this.selectedCoursesView = this.selectedCourses.slice();



            angular.forEach(nodeData,
                v => {
                    if (v.boxes) {
                        for (let i = v.boxes.length - 1; i >= 0; i--) {
                            const box = v.boxes[i];
                            if (ids.indexOf(box.id) !== -1) {
                                //box["selected"] = true;
                                v.boxes.splice(i, 1);
                                const course = this.selectedCourses.find(f => f.id === box.id);
                                course.department = v.name;
                            } else {
                                box["twoLetter"] = box.name.substr(0, 2).toLowerCase();
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
            // we manipulate the boxes inorder to remove them
            const boxes = this.$filter("filter")(angular.copy(this.nodeData), (value) => {
                //value.boxes = this.$filter("filter")(value.boxes, this.term);
                value.boxes = this.$filter("filter")(value.boxes,
                    ((v1: ISmallBox) => {
                        var lower = this.term.toLowerCase();
                        if (v1.name.toLowerCase().indexOf(lower) !== -1) {
                            return v1;
                        }
                        if (v1.courseId && v1.courseId.toLowerCase().indexOf(lower) !== -1) {
                            return v1;
                        }
                        if (v1.professor && v1.professor.toLowerCase().indexOf(lower) !== -1) {
                            return v1;
                        }

                    }));


                if (value.boxes && value.boxes.length) {
                    return value.boxes;
                };

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
                disableParentScroll:true,
                locals: {
                    course: course,
                    courses: this.selectedCourses //this.selectedCoursesView
                },
                fullscreen: false // Only for -xs, -sm breakpoints.
            }).then((response: ISelectedBoxClassChoose) => {
                this.selectedCoursesView = this.selectedCourses.slice();
                var department = this.nodeData.find(f => f.id === response.departmentId);
                if (!department) {
                    department = {
                        id: response.departmentId,
                        name: response.name,
                        type: 'open',
                        boxes: []
                    };
                    this.nodeData.push(department);
                }
                department.boxes.push({
                    courseId: response.courseId,
                    id: response.id,
                    items: response.items,
                    members: response.members,
                    name: response.name,
                    professor: response.professor
                });
                //var box = department.boxes.find(f => f.id === response.id);
                //box["selected"] = false;
            });

        }



        choose(course: ISmallBox, department: ISmallDepartment) {
            this.boxService.follow(course.id);
            this.$scope.$emit("refresh-boxes");


            department.boxes.splice(department.boxes.indexOf(course), 1);
            if (!department.boxes.length) {
                this.nodeData.splice(this.nodeData.indexOf(department), 1);
            }
            //course["selected"] = true;

            const pushOne = angular.extend({},
                course,
                {
                    department: department.name,
                    departmentId: department.id
                });
            this.selectedCourses.push(
                pushOne);

        }
        animationEnd(id) {
            const box = this.selectedCourses.find(f => f.id === id);
            if (box) {
                this.selectedCoursesView.push(box);
            }
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
                disableParentScroll:true
                //scope: this.$scope,
                //fullscreen: true // Only for -xs, -sm breakpoints.
            }).then((response) => {
                this.nodeData = response.nodeData;
                this.selectedCourses.push((response.box as ISelectedBoxClassChoose));
                this.selectedCoursesView = this.selectedCourses.slice();
                this.$scope.$emit("refresh-boxes");
            });
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
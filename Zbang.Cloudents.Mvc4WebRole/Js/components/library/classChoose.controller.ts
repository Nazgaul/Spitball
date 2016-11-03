module app {
    "use strict";


    export interface ISelectedBoxClassChoose extends ISmallBox {
        department: string;
        departmentId: Guid;
        // animate: boolean;
    }
    var selectedCourses: Array<ISelectedBoxClassChoose> = [];
    class ClassChoose {
        static $inject = ["searchService", "libraryService", "$mdDialog", "$filter",
            "nodeData", "boxService", "boxes", "resManager", "$scope", "$anchorScroll"];
        selectedCoursesView: Array<ISelectedBoxClassChoose> = [];
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
            var ids = [];
            angular.forEach(boxes,

                (v) => {
                    ids.push(v.id);
                    selectedCourses.push({
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
            this.selectedCoursesView = selectedCourses.slice();

            angular.forEach(nodeData,
                v => {
                    if (v.boxes) {
                        for (let i = v.boxes.length - 1; i >= 0; i--) {
                            const box = v.boxes[i];

                            box["twoLetter"] = box.name.substr(0, 2).toLowerCase();
                            if (ids.indexOf(box.id) !== -1) {
                                box["selected"] = true;
                                const course = selectedCourses.find(f => f.id === box.id);
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
            // we manipulate the boxes inorder to remove them
            const boxes = this.$filter("filter")(angular.copy(this.nodeData), (value) => {
                //value.boxes = this.$filter("filter")(value.boxes, this.term);
                value.boxes = this.$filter("filter")(value.boxes,
                    ((v1: ISmallBox) => {
                        var lower = this.term.toLowerCase();
                        if (v1.name.toLowerCase().indexOf(lower) !== -1) {
                            return v1;
                        }
                        if (v1.courseCode && v1.courseCode.toLowerCase().indexOf(lower) !== -1) {
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
            console.log(boxes, department);

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
                    courses: selectedCourses //this.selectedCoursesView
                },
                //scope: this.$scope,
                fullscreen: false // Only for -xs, -sm breakpoints.
            }).then((response: ISelectedBoxClassChoose) => {
                this.selectedCoursesView = selectedCourses.slice();
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
            selectedCourses.push(
                pushOne);

        }
        animationEnd(id) {
            //var strId = args.replace("box_", "");
            //var id = parseInt(strId, 10);
            const box = selectedCourses.find(f => f.id === id);
            if (box) {
                this.selectedCoursesView.push(box);
                //box.animate = true;
                //$scope.$apply();
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
                //scope: this.$scope,
                fullscreen: false // Only for -xs, -sm breakpoints.
            }).then((response) => {
                this.nodeData = response.nodeData;
                selectedCourses.push((response.box as ISelectedBoxClassChoose));
                this.selectedCoursesView = selectedCourses.slice();
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
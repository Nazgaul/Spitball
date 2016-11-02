module app {
    "use strict";
    class ClassChooseDialog {
        static $inject = ["$mdDialog", "selectedDepartment", "$filter", "nodeData", "libraryService"];
        constructor(private $mdDialog: angular.material.IDialogService,
            private selectedDepartment: ISmallDepartment,
            private $filter: angular.IFilterService,
            private nodeData: Array<ISmallDepartment>,
            private libraryService: ILibraryService
        ) {
            this.departmentName = selectedDepartment.name;
            //console.log(department)
        }
        departmentName;
        submitDisabled = false;
        create = {};
        //confirmStep = false;
        close() {
            this.$mdDialog.cancel();
        };



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
                    }).catch(response => {
                        createBox["name"].$setValidity('server', false);
                        this.create["error"] = response;
                    }).finally(() => {
                        this.submitDisabled = false;
                    });;
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
                        members: 1,
                        selected: true
                    };
                    department.boxes = department.boxes || [];
                    department.boxes.push(box);
                    box["department"] = this.selectedDepartment.name;
                    //this.selectedCourses.push((box as ISmallBoxClassChoose));
                    //this.selectedDepartment = null;
                    angular.forEach(createObj,
                        (value, key) => {
                            this.create[key] = '';
                        });
                    this.$mdDialog.hide({
                        nodeData: this.nodeData,
                        box: box
                    });
                    //this.showCreateClass = false;
                }).catch(response => {
                    createBox["name"].$setValidity('server', false);
                    this.create["error"] = response;
                }).finally(() => {
                    this.submitDisabled = false;
                });
        }

        queryDepartments(text: String) {
            const result = this.$filter("filter")(this.nodeData, text);//.map(m => { return { name: m.name, id: m.id } });
            return result;
        }

    }

    angular.module("app.library").controller("ClassChooseDialog", ClassChooseDialog);
}
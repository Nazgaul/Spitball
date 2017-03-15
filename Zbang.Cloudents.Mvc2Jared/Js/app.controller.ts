
module app {
    "use strict";
    class Document1 {
        itemName: string = "my document name";
        boxName: string = "my document box_name";
        department: string = "my document department";
        docType: string;

    }

    export class AppController {
        static $inject = ["$scope","searchService"];
        tab: string;
        resNum: number = 0;
        counter: number = 0;
        noResults = false;
        result = [];

        constructor(private $scope: angular.IScope, private searchService: IHelpService) {

        }
        formData: Object;
        doc: any;
        class: string = "this is Class";
        yifat: number = 5;
        search()
        {
            if (this.formData) {
                Object.keys(this.formData).forEach(k => (!this.formData[k] && this.formData[k] !== undefined) && delete this.formData[k]);
                if (!Object.keys(this.formData).length) console.log("empty");
                console.log(this.formData);
                var promise = this.searchService.searchItems(this.formData);
                promise.then(response => {
                this.result = response;
                this.doc = this.result[0];
                this.doc["tags"] = ["aaa", "bb"];
                this.resNum = this.result.length;
                });
            }
        }
        nextPage() {
            this.counter++;
            //change doc
            this.doc = this.result[this.counter];
        }
        prevPage() {
            this.counter--;
            this.doc = this.result[this.counter];
        }


    }

    angular.module("app").controller("AppController", AppController);
}
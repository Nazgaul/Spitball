
module app {
    "use strict";
    class Document1 {
        ItemName: string = "my document name";
        BoxName: string = "my document box_name";
        Department: string = "my document department";
        DocType: string;
        ItemId: number;
        Tags: any;
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
        documents = [];
        search()
        {
            if (this.formData) {
                Object.keys(this.formData).forEach(k => (!this.formData[k] && this.formData[k] !== undefined) && delete this.formData[k]);
                if (Object.keys(this.formData).length) {
                    console.log(this.formData);
                    var promise = this.searchService.searchItems(this.formData);
                    promise.then(response => {
                        this.result = response;
                        this.counter = 0;
                        this.doc = this.result[0];
                        this.getPreview();
                        this.resNum = this.result.length;
                    });
                }
            }
        }
        getPreview() {
            const amount = 15;
            var self = this; 
            return self.searchService.getPreview(self.doc.Blob, amount,self.doc.ItemId).then(data => {
                data = data || {};
                if (data.Content) {
                    self.documents = data.Content;
                    console.log(data.Content);
                    return;
                }
            });
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
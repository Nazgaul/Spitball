
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
        ChangedName: string = "";
        originalName: string;
        removedTags=[];
        newTags = [];
        newTag: string = "";
        originalType: string;
        ChangedType: string = "";
        formData: Object;
        doc: any;
        optionalTabs = [];
        documents = [];
        constructor(private $scope: angular.IScope, private searchService: IHelpService) {

        }
        search() {
            console.log("search");
            if (this.formData) {
                Object.keys(this.formData).forEach(k => (!this.formData[k] && this.formData[k] !== undefined) && delete this.formData[k]);
                if (Object.keys(this.formData).length) {
                    console.log(this.formData);
                    var promise = this.searchService.searchItems(this.formData);
                    promise.then(response => {
                        this.result = response;
                        this.counter = 0;
                        this.doc = this.result[0];
                        if (this.doc.DocType) {
                            this.optionalTabs[0].id = this.doc.TypeId;
                            this.optionalTabs[0].name = this.doc.DocType;
                        }
                        this.removedTags = [];
                        this.newTags = [];
                        this.getPreview();
                        this.getTabs();
                        this.resNum = this.result.length;
                        this.originalName = this.doc.ItemName;
                        this.originalType = this.doc.TypeId;
                    });
                }
            }
        }
        getTabs() {
            console.log(this.doc.BoxId);
            this.searchService.getTabs(this.doc.BoxId).then(response => {
                this.optionalTabs = response;
                console.log(this.optionalTabs);
            });
        }
        getPreview() {
            var self = this; 
            return self.searchService.getPreview(self.doc.Blob, self.doc.ItemId).then(data => {
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
            this.originalName = this.doc.ItemName;
            this.originalType = this.doc.DocType;
            this.getPreview();
        }
        prevPage() {
            this.counter--;
            this.doc = this.result[this.counter];
            this.originalName = this.doc.ItemName;
            this.originalType = this.doc.DocType;
            this.getPreview();
        }
        Save() {

            
                if(this.doc.ItemName != this.originalName)
            {
                    this.ChangedName = this.doc.ItemName;
            }

                if (this.doc.TypeId != this.originalType) {
                    this.ChangedType = this.doc.TypeId;
                }

                this.searchService.saveItem(this.doc.ItemId, this.doc.BoxId, this.ChangedName, this.ChangedType,
                    this.newTags, this.removedTags)
        }
        deleteTag(i) {
            //this.doc.ItemName = "deleted" + i;
            this.removedTags.push(this.doc.Tags[i]);
           // this.doc.Tags[i].hide();
            this.doc.Tags.splice(i,1);


        }
        AddNewTag() {
            this.newTags.push(this.newTag);
            this.doc.Tags.push(this.newTag);
            this.newTag = "";
            }


        

    }

    angular.module("app").controller("AppController", AppController);
}
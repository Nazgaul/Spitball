
module app {
    "use strict";

    export class AppController {
        static $inject = ["$scope", "searchService", "$interval"];
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
        doc: any = null;
        optionalTabs = [];
        documents = [];
        changesSaved = false
        constructor(private $scope: angular.IScope, private searchService: IHelpService, private $interval: angular.IIntervalService) {

        }
        search() {
            if (this.formData) {
                Object.keys(this.formData).forEach(k => (!this.formData[k] && this.formData[k] !== undefined) && delete this.formData[k]);
                if (Object.keys(this.formData).length) {
                    console.log(this.formData);
                    var promise = this.searchService.searchItems(this.formData);
                    promise.then(response => {
                        this.result = response;
                        this.counter = 0;
                        if (this.result.length > 0) {
                            this.doc = this.result[0];
                            this.optionalTabs = this.doc.Tabs;
                            this.originalName = this.doc.ItemName;
                            this.originalType = this.doc.TypeId;
                            this.getPreview();
                            this.noResults = false;
                        }
                        else {
                            this.noResults = true;
                        }
                     
                        this.removedTags = [];
                        this.newTags = [];
                        this.resNum = this.result.length;
                        this.$scope.$apply()
                    });
                }
            }
        }
        getPreview() {
            var self = this; 
            return self.searchService.getPreview(self.doc.Blob, self.doc.ItemId).then(data => {
                data = data || {};
                if (data.Content) {
                    self.documents = data.Content;
                    console.log(data.Content);
                }
                else {
                    self.documents = ["/images1/1.jpg"]
                }
                this.$scope.$apply()
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

                var promise = this.searchService.saveItem(this.doc.ItemId, this.doc.BoxId, this.ChangedName, this.ChangedType,
                    this.newTags, this.removedTags)
                promise.then(response =>{
                    this.changesSaved = true
                    this.$interval(() => {
                        this.changesSaved = false
                    }, 1500)
                    this.$scope.$apply()
                })
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
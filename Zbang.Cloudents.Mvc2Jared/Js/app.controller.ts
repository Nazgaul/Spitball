
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
        removedTags = [];
        newTags = [];
        newTag: string = "";
        originalType: string;
        ChangedType: number = -1;
        formData: any = {
            department: "",
            isSearchType: true
        };
        doc: any = null;
        documents = [];
        changesSaved = false
        showLoader = false
        statusText = ""
        departmentsAutoComplete = []
        universityAutoComplete = []
        tagAutoComplete = []
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
                            this.noResults = false
                            this.doc = this.result[0];
                            this.doc.TypeId = null;
                            if (!this.formData.isSearchType) {
                                this.doc.TypeId = this.doc.DocType.toString();
                            }
                            this.originalName = this.doc.ItemName;
                            this.originalType = this.doc.DocType;
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
            this.doc.TypeId = null;
            if (!this.formData.isSearchType) {
                this.doc.TypeId = this.doc.DocType.toString();
            }
            this.originalName = this.doc.ItemName;
            this.originalType = this.doc.DocType;
            this.getPreview();
        }
        prevPage() {
            this.counter--;
            this.doc = this.result[this.counter];
            this.doc.TypeId = null;
            if (!this.formData.isSearchType) {
                this.doc.TypeId = this.doc.DocType.toString();
            }
            this.originalName = this.doc.ItemName;
            this.originalType = this.doc.DocType;
            this.getPreview();
        }
        Save() {
            this.showLoader = true
            this.ChangedName = "";
            this.ChangedType = -1;
            if (this.doc.ItemName != this.originalName) {
                this.ChangedName = this.doc.ItemName;
            }
            if (this.doc.TypeId != null) {
                this.doc.DocType = parseInt(this.doc.TypeId, 10);
                if (this.doc.DocType != this.originalType) {
                    this.ChangedType = this.doc.DocType;
                }
            }

            var promise = this.searchService.saveItem(this.doc.ItemId, this.doc.BoxId, this.ChangedName, this.ChangedType,
                this.newTags, this.removedTags)
            promise.then(response => {
                this.showLoader = false
                this.showStatus("Your changes were saved")
                this.$scope.$apply()
            })
        }
        deleteTag(i) {
            //this.doc.ItemName = "deleted" + i;
            this.removedTags.push(this.doc.Tags[i]);
            // this.doc.Tags[i].hide();
            this.doc.Tags.splice(i, 1);


        }
        AddNewTag() {
            this.newTags.push(this.newTag);
            this.doc.Tags.push(this.newTag);
            this.newTag = "";
        }
        deleteDoc() {
            this.showLoader = true
            this.searchService.deleteDoc(this.doc.ItemId).then(() => {
                this.showLoader = false
                var docIndex = this.counter
                var docName = this.doc.ItemName
                this.result.splice(docIndex, 1)
                this.resNum = this.result.length
                if (this.counter == this.result.length) {
                    if (this.result.length == 0) {
                        this.doc = null
                        this.noResults = false;
                    }
                    else {
                        this.counter--
                        this.refreshDocData()
                    }
                }
                else {
                    this.refreshDocData()
                }
                this.showStatus(this.statusText = docName + " was deleted")
                this.$scope.$apply()
            })
        }
        refreshDocData() {
            this.doc = this.result[this.counter]
            this.getPreview();
        }
        showStatus(message) {
            this.statusText = message
            this.changesSaved = true
            this.$interval(() => {
                this.changesSaved = false
            }, 1500)
        }

        autocomplete(field) {
            var self = this;
            switch (field) {
                case "department":
                    this.searchService.autoDepartment(this.formData["department"]).then(data => {
                        self.departmentsAutoComplete = data
                        this.$scope.$apply()
                    })
                    break;
                case "university":
                    this.searchService.autoUni(this.formData["university"]).then(data => {
                        self.universityAutoComplete = data
                        this.$scope.$apply()
                    })
                    break;
                case "tag":
                    this.searchService.autoTag(this.newTag).then(data => {
                        self.tagAutoComplete = data
                        this.$scope.$apply()
                    })
                    break;
            }
        }
    }

    angular.module("app").controller("AppController", AppController);
}
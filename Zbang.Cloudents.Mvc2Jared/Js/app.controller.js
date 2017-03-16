var app;
(function (app) {
    "use strict";
    var Document1 = (function () {
        function Document1() {
            this.ItemName = "my document name";
            this.BoxName = "my document box_name";
            this.Department = "my document department";
        }
        return Document1;
    }());
    var AppController = (function () {
        function AppController($scope, searchService) {
            this.$scope = $scope;
            this.searchService = searchService;
            this.resNum = 0;
            this.counter = 0;
            this.noResults = false;
            this.result = [];
            this.ChangedName = "";
            this.removedTags = [];
            this.newTags = [];
            this.newTag = "";
            this.ChangedType = "";
            this.documents = [];
        }
        AppController.prototype.search = function () {
            var _this = this;
            console.log("search");
            if (this.formData) {
                Object.keys(this.formData).forEach(function (k) { return (!_this.formData[k] && _this.formData[k] !== undefined) && delete _this.formData[k]; });
                if (Object.keys(this.formData).length) {
                    console.log(this.formData);
                    var promise = this.searchService.searchItems(this.formData);
                    promise.then(function (response) {
                        _this.result = response;
                        _this.counter = 0;
                        _this.doc = _this.result[0];
                        if (_this.doc.DocType) {
                            _this.optionalTabs[0].id = _this.doc.TypeId;
                            _this.optionalTabs[0].name = _this.doc.DocType;
                        }
                        _this.removedTags = [];
                        _this.newTags = [];
                        _this.getPreview();
                        _this.getTabs();
                        _this.resNum = _this.result.length;
                        _this.originalName = _this.doc.ItemName;
                        _this.originalType = _this.doc.DocType;
                    });
                }
            }
        };
        AppController.prototype.getTabs = function () {
            var _this = this;
            console.log(this.doc.BoxId);
            this.searchService.getTabs(this.doc.BoxId).then(function (response) {
                _this.optionalTabs = response;
                console.log(_this.optionalTabs);
            });
        };
        AppController.prototype.getPreview = function () {
            var self = this;
            return self.searchService.getPreview(self.doc.Blob, self.doc.ItemId).then(function (data) {
                data = data || {};
                if (data.Content) {
                    self.documents = data.Content;
                    console.log(data.Content);
                    return;
                }
            });
        };
        AppController.prototype.nextPage = function () {
            this.counter++;
            //change doc
            this.doc = this.result[this.counter];
            this.getPreview();
        };
        AppController.prototype.prevPage = function () {
            this.counter--;
            this.doc = this.result[this.counter];
            this.getPreview();
        };
        AppController.prototype.Save = function () {
            if (this.doc.ItemName != this.originalName) {
                this.ChangedName = this.doc.ItemName;
            }
            if (this.doc.TypeId != this.originalType) {
                this.ChangedType = this.doc.TypeId;
            }
            this.searchService.saveItem(this.doc.ItemId, this.doc.BoxId, this.ChangedName, this.ChangedType, this.newTags, this.removedTags);
        };
        AppController.prototype.deleteTag = function (i) {
            //this.doc.ItemName = "deleted" + i;
            this.removedTags.push(this.doc.Tags[i]);
            // this.doc.Tags[i].hide();
            this.doc.Tags.splice(i, 1);
        };
        AppController.prototype.AddNewTag = function () {
            this.newTags.push(this.newTag);
            this.doc.Tags.push(this.newTag);
            this.newTag = "";
        };
        return AppController;
    }());
    AppController.$inject = ["$scope", "searchService"];
    app.AppController = AppController;
    angular.module("app").controller("AppController", AppController);
})(app || (app = {}));

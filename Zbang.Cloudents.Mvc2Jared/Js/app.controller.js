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
            this.removedTags = "";
            this.documents = [];
        }
        AppController.prototype.search = function () {
            var _this = this;
            if (this.formData) {
                Object.keys(this.formData).forEach(function (k) { return (!_this.formData[k] && _this.formData[k] !== undefined) && delete _this.formData[k]; });
                if (Object.keys(this.formData).length) {
                    console.log(this.formData);
                    var promise = this.searchService.searchItems(this.formData);
                    promise.then(function (response) {
                        _this.result = response;
                        _this.counter = 1;
                        _this.doc = _this.result[_this.counter];
                        _this.getPreview();
                        _this.resNum = _this.result.length;
                        _this.originalName = _this.doc.ItemName;
                    });
                }
            }
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
            //  saveItem(itemId, newName, newType, newTags, removedTags)
        };
        AppController.prototype.deleteTag = function (i) {
            //this.doc.ItemName = "deleted" + i;
            this.removedTags += this.doc.Tags[i];
            this.doc.Tags[i].hide();
        };
        AppController.prototype.AddNewTag = function () {
            this.doc.ItemName = "deleted";
        };
        return AppController;
    }());
    AppController.$inject = ["$scope", "searchService"];
    app.AppController = AppController;
    angular.module("app").controller("AppController", AppController);
})(app || (app = {}));

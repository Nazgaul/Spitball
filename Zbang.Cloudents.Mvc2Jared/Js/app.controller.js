var app;
(function (app) {
    "use strict";
    var AppController = (function () {
        function AppController($scope, searchService, $interval) {
            this.$scope = $scope;
            this.searchService = searchService;
            this.$interval = $interval;
            this.resNum = 0;
            this.counter = 0;
            this.noResults = false;
            this.result = [];
            this.ChangedName = "";
            this.removedTags = [];
            this.newTags = [];
            this.newTag = "";
            this.ChangedType = "";
            this.formData = {
                department: "",
                isSearchType: true
            };
            this.doc = null;
            this.optionalTabs = [];
            this.documents = [];
            this.changesSaved = false;
            this.showLoader = false;
            this.statusText = "";
            this.departmentsAutoComplete = [];
            this.universityAutoComplete = [];
            this.tagAutoComplete = [];
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
                        _this.counter = 0;
                        if (_this.result.length > 0) {
                            _this.noResults = false;
                            _this.doc = _this.result[0];
                            _this.optionalTabs = _this.doc.Tabs;
                            _this.originalName = _this.doc.ItemName;
                            _this.originalType = _this.doc.TypeId;
                            _this.getPreview();
                            _this.noResults = false;
                        }
                        else {
                            _this.noResults = true;
                        }
                        _this.removedTags = [];
                        _this.newTags = [];
                        _this.resNum = _this.result.length;
                        _this.$scope.$apply();
                    });
                }
            }
        };
        AppController.prototype.getPreview = function () {
            var _this = this;
            var self = this;
            return self.searchService.getPreview(self.doc.Blob, self.doc.ItemId).then(function (data) {
                data = data || {};
                if (data.Content) {
                    self.documents = data.Content;
                    console.log(data.Content);
                }
                else {
                    self.documents = ["/images1/1.jpg"];
                }
                _this.$scope.$apply();
            });
        };
        AppController.prototype.nextPage = function () {
            this.counter++;
            //change doc
            this.doc = this.result[this.counter];
            this.originalName = this.doc.ItemName;
            this.originalType = this.doc.DocType;
            this.optionalTabs = this.doc.Tabs;
            this.getPreview();
        };
        AppController.prototype.prevPage = function () {
            this.counter--;
            this.doc = this.result[this.counter];
            this.originalName = this.doc.ItemName;
            this.originalType = this.doc.DocType;
            this.optionalTabs = this.doc.Tabs;
            this.getPreview();
        };
        AppController.prototype.Save = function () {
            var _this = this;
            this.showLoader = true;
            if (this.doc.ItemName != this.originalName) {
                this.ChangedName = this.doc.ItemName;
            }
            if (this.doc.TypeId != this.originalType) {
                this.ChangedType = this.doc.TypeId;
            }
            var promise = this.searchService.saveItem(this.doc.ItemId, this.doc.BoxId, this.ChangedName, this.ChangedType, this.newTags, this.removedTags);
            promise.then(function (response) {
                _this.showLoader = false;
                _this.showStatus("Your changes were saved");
                _this.$scope.$apply();
            });
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
        AppController.prototype.deleteDoc = function () {
            var _this = this;
            this.showLoader = true;
            this.searchService.deleteDoc(this.doc.ItemId).then(function () {
                _this.showLoader = false;
                var docIndex = _this.counter;
                var docName = _this.doc.ItemName;
                _this.result.splice(docIndex, 1);
                _this.resNum = _this.result.length;
                if (_this.counter == _this.result.length) {
                    if (_this.result.length == 0) {
                        _this.doc = null;
                        _this.noResults = false;
                    }
                    else {
                        _this.counter--;
                        _this.refreshDocData();
                    }
                }
                else {
                    _this.refreshDocData();
                }
                _this.showStatus(_this.statusText = docName + " was deleted");
                _this.$scope.$apply();
            });
        };
        AppController.prototype.refreshDocData = function () {
            this.doc = this.result[this.counter];
            this.getPreview();
        };
        AppController.prototype.showStatus = function (message) {
            var _this = this;
            this.statusText = message;
            this.changesSaved = true;
            this.$interval(function () {
                _this.changesSaved = false;
            }, 1500);
        };
        AppController.prototype.autocomplete = function (field) {
            var _this = this;
            //var autoComleteModel;
            //var autoCompleteServiceFunc;
            //var autoCompleteList;
            var self = this;
            switch (field) {
                case "department":
                    this.searchService.autoDepartment(this.formData["department"]).then(function (data) {
                        self.departmentsAutoComplete = data;
                        _this.$scope.$apply();
                    });
                    break;
                case "university":
                    this.searchService.autoUni(this.formData["university"]).then(function (data) {
                        self.universityAutoComplete = data;
                        _this.$scope.$apply();
                    });
                    break;
                case "tag":
                    this.searchService.autoTag(this.newTag).then(function (data) {
                        self.tagAutoComplete = data;
                        _this.$scope.$apply();
                    });
                    break;
            }
        };
        return AppController;
    }());
    AppController.$inject = ["$scope", "searchService", "$interval"];
    app.AppController = AppController;
    angular.module("app").controller("AppController", AppController);
})(app || (app = {}));

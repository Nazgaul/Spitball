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
            this.class = "this is Class";
            this.yifat = 5;
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
                        _this.doc = _this.result[0];
                        _this.getPreview();
                        _this.resNum = _this.result.length;
                    });
                }
            }
        };
        AppController.prototype.getPreview = function () {
            var amount = 15;
            var self = this;
            return self.searchService.getPreview(self.doc.Blob, amount, self.doc.ItemId).then(function (data) {
                data = data || {};
                if (data.template) {
                    console.log(data.content);
                    //self.view = "preview-" + data.template.toLowerCase() + ".html";
                    //if (data.content) {
                    //    self.documents = self.documents.concat(data.content);
                    //}
                    return;
                }
            });
        };
        AppController.prototype.nextPage = function () {
            this.counter++;
            //change doc
            this.doc = this.result[this.counter];
        };
        AppController.prototype.prevPage = function () {
            this.counter--;
            this.doc = this.result[this.counter];
        };
        return AppController;
    }());
    AppController.$inject = ["$scope", "searchService"];
    app.AppController = AppController;
    angular.module("app").controller("AppController", AppController);
})(app || (app = {}));

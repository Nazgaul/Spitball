var app;
(function (app) {
    "use strict";
    var ShowToasterService = (function () {
        function ShowToasterService($document, $mdToast) {
            this.$document = $document;
            this.$mdToast = $mdToast;
        }
        ShowToasterService.prototype.showToaster = function (text, parentId) {
            var element = this.$document.find("header")[0];
            if (parentId) {
                element = this.$document[0].querySelector("#" + parentId);
            }
            var toaster = this.$mdToast.simple()
                .textContent(text)
                .position("top right")
                .parent(element)
                .hideDelay(2000);
            toaster.toastClass("angular-animate");
            this.$mdToast.show(toaster);
        };
        ;
        return ShowToasterService;
    }());
    ShowToasterService.$inject = ['$document', '$mdToast'];
    angular.module("app").service("showToasterService", ShowToasterService);
})(app || (app = {}));
//# sourceMappingURL=showToaster.service.js.map
var app;
(function (app) {
    "use strict";
    var PreviewController = (function () {
        function PreviewController($mdDialog, doc, blob, $sce) {
            this.$mdDialog = $mdDialog;
            this.downloadLink = "/chat/download/?blobName=" + blob;
            if (!doc || !doc.viewName) {
                this.view = 'preview-faild.html';
            }
            else {
                this.items = doc.content;
                if (doc.viewName === 'Text') {
                    this.items[0] = $sce.trustAsResourceUrl(this.items[0]);
                }
                this.view = "chat-" + doc.viewName + ".html";
            }
        }
        PreviewController.prototype.close = function () {
            this.$mdDialog.hide();
        };
        PreviewController.$inject = ['$mdDialog', 'doc', 'blob', '$sce'];
        return PreviewController;
    }());
    angular.module('app.chat').controller('previewController', PreviewController);
})(app || (app = {}));
//# sourceMappingURL=previewController.js.map
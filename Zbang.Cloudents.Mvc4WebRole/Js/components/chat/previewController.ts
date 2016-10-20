module app {
    "use strict";

    class PreviewController {
        static $inject = ['$mdDialog', 'doc', 'blob', '$sce'];

        private view;
        private downloadLink;
        private items;

        constructor(private $mdDialog: angular.material.IDialogService,
            // TODO
            doc: any,
            blob: string,
            // $scope: angular.IScope,
            $sce: angular.ISCEService
        ) {
            this.downloadLink = `/chat/download/?blobName=${blob}`;
            if (!doc || !doc.viewName) {
                this.view = 'preview-faild.html';
            }
            else {
                this.items = doc.content;
                if (doc.viewName === 'Text') {
                    this.items[0] = $sce.trustAsResourceUrl(this.items[0]);
                }
                this.view = `chat-${doc.viewName}.html`;
            }
            //$scope.$on('$stateChangeStart', function () {
            //    this.$mdDialog.hide();
            //});
        }
        close() {
            this.$mdDialog.hide();
        }

    }

    angular.module('app.chat').controller('previewController', PreviewController);
}


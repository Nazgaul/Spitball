module app {
    "use strict";

    export interface IShowToasterService {
        showToaster(text: string, parentId?: string): void;
        showTemplateToaster(template: string, controller: string, parentId: string): void;
    }

    class ShowToasterService implements IShowToasterService {
        static $inject = ['$document', '$mdToast'];
        constructor(private $document: angular.IDocumentService,
            private $mdToast: angular.material.IToastService) {
        }

        showToaster(text, parentId) {
            let element: Element = this.$document.find("header")[0];
            if (parentId) {
                element = this.$document[0].querySelector(`#${parentId}`);
            }
            var toaster: any = this.$mdToast.simple()
                .textContent(text)
                .position("top right")
                .parent(element)
                .hideDelay(2000);
            // typedef doesnt have definition of toastClass
            toaster.toastClass("angular-animate");
            this.$mdToast.show(toaster);
        };


        showTemplateToaster(template, controller, parentId) {
            let element: Element = this.$document.find("body")[0];
            if (parentId) {
                element = this.$document[0].querySelector(`#${parentId}`);
            }
            this.$mdToast.show({
                hideDelay: 0,
                position: 'top right',
                controller: controller,
                templateUrl: template,
                toastClass: 'angular-animate',
                parent: element
            });
        };

    }

    angular.module("app").service("showToasterService", ShowToasterService);
}




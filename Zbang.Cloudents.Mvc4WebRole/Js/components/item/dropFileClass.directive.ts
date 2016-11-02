module app {
    "use strict";
    var dragTimer: angular.IPromise<any>;
    class DropFileClass implements angular.IDirective {

        constructor(private $timeout: angular.ITimeoutService) {

        }
        link = (scope, element: JQuery) => {
            $(document).on("dragover", e => {
                var dt = (e.originalEvent as DragEvent).dataTransfer;
                if (dt.types && ( (dt.types as any).indexOf ? (dt.types as any).indexOf('Files') !== -1 : dt.types.contains('Files'))) {
                    element.addClass("drop");
                    this.$timeout.cancel(dragTimer);
                }
            })

                .on('dragleave',
                e => {
                    dragTimer = this.$timeout(() => {
                        element.removeClass("drop");
                    }, 25);
                });
        }

        static factory(): angular.IDirectiveFactory {
            const directive = ($timeout) => {
                return new DropFileClass($timeout);
            };
            directive["$inject"] = ["$timeout"];
            return directive;
        }
    }
    angular
        .module("app.item")
        .directive("dropFile", DropFileClass.factory());
}
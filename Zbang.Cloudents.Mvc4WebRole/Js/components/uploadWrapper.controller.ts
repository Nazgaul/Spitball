module app {
    "use strict";

    class UploadWrapper {
        html;
        open;
        static $inject = ["ajaxService2", "$scope", "$rootScope"];
        constructor(private ajaxService: IAjaxService2, private $scope: angular.IScope, private $rootScope: angular.IRootScopeService) {
            $scope.$on('open_upload',
                () => {
                    $rootScope.$broadcast('close-collapse');
                    this.open = true;
                });
            $scope.$on("close_upload",
            () => {
                this.open = false;
            });
        }
        uploadOpen() {
            if (this.html) {
                return;
            }
            return this.ajaxService.getHtml('/item/uploaddialog/').then(response => {
                this.html = response;
            });
        }
        uploadCollapsed() {
            this.$scope.$broadcast("uploadCollapsed");
        }

    }

    angular.module("app").controller("UploadWrapper", UploadWrapper);
}


(function () {
    angular.module('app').directive('compileHtml', compileHtml);
    compileHtml.$inject = ['$sce', '$parse', '$compile'];
    function compileHtml($sce, $parse, $compile) {
        return {
            link: function (scope, element, attr) {
                var parsed = $parse(attr.compileHtml);

                function getStringValue() {
                    return (parsed(scope) || '').toString();
                }

                scope.$watch(getStringValue, function () {
                    var el = $compile(parsed(scope) || '')(scope);
                    element.empty();
                    element.append(el);
                });
            }
        };
    }
})();
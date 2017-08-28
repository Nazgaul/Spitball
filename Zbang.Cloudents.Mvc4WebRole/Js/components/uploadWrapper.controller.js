"use strict";
var app;
(function (app) {
    "use strict";
    var UploadWrapper = (function () {
        function UploadWrapper(ajaxService, $scope, $rootScope) {
            var _this = this;
            this.ajaxService = ajaxService;
            this.$scope = $scope;
            this.$rootScope = $rootScope;
            $scope.$on('open_upload', function () {
                $rootScope.$broadcast('close-collapse');
                _this.open = true;
            });
            $scope.$on("close_upload", function () {
                _this.open = false;
            });
        }
        UploadWrapper.prototype.uploadOpen = function () {
            var _this = this;
            if (this.html) {
                return;
            }
            return this.ajaxService.getHtml('/item/uploaddialog/').then(function (response) {
                _this.html = response;
            });
        };
        UploadWrapper.prototype.uploadCollapsed = function () {
            this.$scope.$broadcast("uploadCollapsed");
        };
        UploadWrapper.$inject = ["ajaxService2", "$scope", "$rootScope"];
        return UploadWrapper;
    }());
    angular.module("app").controller("UploadWrapper", UploadWrapper);
})(app || (app = {}));
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
//# sourceMappingURL=uploadWrapper.controller.js.map
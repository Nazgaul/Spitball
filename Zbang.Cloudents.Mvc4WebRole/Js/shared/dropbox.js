var app;
(function (app) {
    "use strict";
    var loaded;
    var DropboxService = (function () {
        function DropboxService($document, $q, $timeout, $interval) {
            this.$document = $document;
            this.$q = $q;
            this.$timeout = $timeout;
            this.$interval = $interval;
        }
        DropboxService.prototype.init = function () {
            var _this = this;
            var self = this;
            var defer = this.$q.defer();
            if (loaded) {
                this.$timeout(function () { defer.resolve(); });
                return defer.promise;
            }
            function load() {
                var js = self.$document[0].createElement('script');
                js.id = "dropboxjs";
                js.setAttribute('data-app-key', 'cfqlue614nyj8k2');
                js.src = "https://www.dropbox.com/static/api/1/dropins.js";
                self.$document[0].getElementsByTagName('head')[0].appendChild(js);
            }
            if (document.readyState === "complete") {
                load();
            }
            else {
                window.addEventListener("load", load, false);
            }
            var interval = this.$interval(function () {
                if (window["Dropbox"] !== undefined && window["Dropbox"]) {
                    loaded = true;
                    defer.resolve();
                    _this.$interval.cancel(interval);
                }
            }, 20);
            return defer.promise;
        };
        ;
        DropboxService.prototype.choose = function () {
            var defer = this.$q.defer();
            window["Dropbox"].choose({
                success: function (files) {
                    defer.resolve(files);
                },
                error: function () {
                    defer.reject('dropbox choose error');
                },
                linkType: "direct",
                multiselect: false
            });
            return defer.promise;
        };
        DropboxService.factory = function () {
            var factory = function ($document, $q, $timeout, $interval) {
                return new DropboxService($document, $q, $timeout, $interval);
            };
            factory["$inject"] = ['$document', '$q', '$timeout', "$interval"];
            ;
            return factory;
        };
        return DropboxService;
    }());
    angular.module("app")
        .factory("dropboxService", DropboxService.factory());
})(app || (app = {}));

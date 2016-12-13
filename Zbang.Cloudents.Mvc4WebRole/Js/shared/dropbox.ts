
module app {
    "use strict";
    var loaded;

    export interface IDropboxService {
        init(): angular.IPromise<any>;
        choose(): angular.IPromise<any>;
    }

    class DropboxService implements IDropboxService {
        constructor(private $document: angular.IDocumentService,
            private $q: angular.IQService,
            private $timeout: angular.ITimeoutService,
            private $interval: angular.IIntervalService) {
        }
        init() {
            var self = this;
            var defer = this.$q.defer();

            if (loaded) {
                this.$timeout(() => { defer.resolve(); });
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
            } else {
                window.addEventListener("load", load, false);
            }


            var interval = this.$interval(() => {
                if (window["Dropbox"] !== undefined && window["Dropbox"]) {
                    loaded = true;
                    defer.resolve();
                    this.$interval.cancel(interval);
                    //window.clearInterval(interval);
                }
            }, 20);
            return defer.promise;
        };
        choose() {
            var defer = this.$q.defer();
            window["Dropbox"].choose({
                success: (files) => {
                    defer.resolve(files);
                },
                error: () => {
                    defer.reject('dropbox choose error');
                },
                linkType: "direct",
                multiselect: false
            });

            return defer.promise;
        }

        public static factory() {
            const factory = ($document: angular.IDocumentService,
                $q: angular.IQService,
                $timeout: angular.ITimeoutService,
                $interval: angular.IIntervalService) => {
                return new DropboxService($document, $q, $timeout, $interval);
            };

            factory["$inject"] = ['$document', '$q', '$timeout', "$interval"];
            ;
            return factory;
        }
    }

    angular.module("app")
        .factory("dropboxService", DropboxService.factory());
}



//  # OLD dropbox.js

//(function () {
//    'use strict';
//    angular.module('app').factory('dropboxService',
//        ['$document', '$q', '$timeout', "$interval",
//            function ($document, $q, $timeout, $interval) {
//                "use strict";
//                var loaded;
//                return {
//                    init: function () {
//                        var defer = $q.defer();

//                        if (loaded) {
//                            $timeout(function () { defer.resolve(); });
//                            return defer.promise;
//                        }




//                        function load() {
//                            var js = $document[0].createElement('script');
//                            js.id = "dropboxjs";
//                            js.setAttribute('data-app-key', 'cfqlue614nyj8k2');
//                            js.src = "https://www.dropbox.com/static/api/1/dropins.js";
//                            $document[0].getElementsByTagName('head')[0].appendChild(js);
//                        }
//                        if (document.readyState === "complete") {
//                            load();
//                        } else {
//                            window.addEventListener("load", load, false);
//                        }


//                        var interval = $interval(function () {
//                            if (window.Dropbox !== undefined && window.Dropbox) {
//                                loaded = true;
//                                defer.resolve();
//                                $interval.cancel(interval);
//                                //window.clearInterval(interval);
//                            }
//                        }, 20);
//                        return defer.promise;
//                    },
//                    choose: function () {
//                        var defer = $q.defer();
//                        window.Dropbox.choose({
//                            success: function (files) {
//                                defer.resolve(files);
//                            },
//                            error: function () {
//                                defer.reject('dropbox choose error');
//                            },
//                            linkType: "direct",
//                            multiselect: false
//                        });

//                        return defer.promise;
//                    }
//                }
//            }
//        ]);

//})();

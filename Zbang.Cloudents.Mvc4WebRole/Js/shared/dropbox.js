(function() {
    'use strict';
    angular.module('app').factory('dropboxService',
    ['$document', '$q', '$timeout',"$interval",
    function ($document, $q, $timeout, $interval) {
        "use strict";
        var loaded;
        return {
            init: function () {
                var defer = $q.defer();

                if (loaded) {
                    $timeout(function () { defer.resolve(); });
                    return defer.promise;
                }


               

                function load() {
                    var js = $document[0].createElement('script');
                    js.id = "dropboxjs";
                    js.setAttribute('data-app-key', 'cfqlue614nyj8k2');
                    js.src = "https://www.dropbox.com/static/api/1/dropins.js";
                    $document[0].getElementsByTagName('head')[0].appendChild(js);
                }
                if (document.readyState === "complete") {
                    load();
                } else {
                    window.addEventListener("load", load, false);
                }


                var interval = $interval(function () {
                    if (window.Dropbox !== undefined && window.Dropbox) {
                        loaded = true;
                        defer.resolve();
                        $interval.cancel(interval);
                        //window.clearInterval(interval);
                    }
                }, 20);
                return defer.promise;
            },
            choose: function () {
                var defer = $q.defer();
                window.Dropbox.choose({
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
            }
        }
    }
    ]);

})();

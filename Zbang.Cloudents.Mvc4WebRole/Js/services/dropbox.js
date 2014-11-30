
mBox.factory('sDropbox',
    ['$document', '$q', '$timeout',
    function ($document, $q, $timeout) {
        "use strict";
        var loaded;
        return {
            init: function () {
                var defer = $q.defer();

                if (loaded) {
                    $timeout(function () { defer.resolve(); });
                    return defer.promise;
                }

                var js = $document[0].createElement('script');
                js.id = "dropboxjs";
                js.setAttribute('data-app-key', 'gppwajedn90rv81');
                js.src = "https://www.dropbox.com/static/api/1/dropins.js";
                $document[0].getElementsByTagName('head')[0].appendChild(js);



                var interval = window.setInterval(function () {
                    if (window.Dropbox !== undefined && window.Dropbox) {
                        defer.resolve();
                        window.clearInterval(interval);
                    }
                }, 20);


                return defer.promise;
            },
            choose: function () {
                var defer = $q.defer();
                Dropbox.choose({
                    success: function (files) {
                        defer.resolve(files);
                    },
                    error: function () {
                        defer.reject('dropbox choose error');
                    },
                    linkType: "direct",
                    multiselect: true
                });

                return defer.promise;
            }
        }
    }
    ]);


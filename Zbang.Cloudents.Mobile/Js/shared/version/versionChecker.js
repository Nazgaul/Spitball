angular.module('app').
    factory('sVerChecker', ['$http', '$angularCacheFactory', '$analytics',
    function ($http, $angularCacheFactory, $analytics) {
        "use strict";
        var clientVersion = document.querySelector('[data-version]').getAttribute('data-version'),
            currentVersion,
            timeInterval = 300000;//five minutes

        setInterval(checkVersion, timeInterval);

        return {
            checkVersion: checkVersion,
            currentVersion: function () {
                return clientVersion;
            }
        };

        function checkVersion() {
            try {
                $http.get('/home/version/').success(function (response) {
                    if (!response.success) {
                        return;
                    }
                    currentVersion = response.payload;

                    if (clientVersion === currentVersion) {
                        return;
                    }
                    $angularCacheFactory.removeAll();


                    $analytics.eventTrack('Version', {
                        category: 'Change',
                        label: 'Version change mismatch popup'
                    });


                    alert('Cloudents has updated, refreshing page');
                    setTimeout(function () {
                        window.location.reload(true);
                    }, 150);
                });
            }
            catch (ex) {

            }
            
        }

    }
    ]);
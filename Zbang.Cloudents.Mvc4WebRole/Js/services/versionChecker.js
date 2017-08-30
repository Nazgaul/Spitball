app.factory('sVerChecker', ['$http', '$angularCacheFactory', '$analytics', '$timeout', 'sNotify',
    function ($http, $angularCacheFactory, $analytics, $timeout, sNotify) {
        "use strict";
        var clientVersion = $('[data-version]').attr('data-version'),
            currentVersion,
            timeInterval = 300000;//five minutes

        //setInterval(checkVersion, timeInterval);

        return {
            checkVersion: checkVersion,
            currentVersion: function () {
                return clientVersion;
            }
        };

        function checkVersion() {
            $.ajax({
                type: 'GET',
                url: '/home/version/',
                contentType: 'application/json',
                success: function (response) {
                    if (response.success) {
                        currentVersion = response.payload;

                        if (clientVersion === currentVersion) {
                            return;
                        }
                        $angularCacheFactory.removeAll();


                        $analytics.eventTrack('Change', {
                            category: 'Version',
                            label: 'Version change mismatch popup'
                        });


                        sNotify.alert('Spitball has updated, refreshing page');
                        $timeout(function () {
                            window.location.reload(true);
                        }, 150);


                    }
                }
            });
        }

    }
]);
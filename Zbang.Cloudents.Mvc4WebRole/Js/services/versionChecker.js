app.factory('sVerChecker', ['$http', '$angularCacheFactory', '$analytics', '$timeout', 'sNotify',
    function ($http, $angularCacheFactory, $analytics, $timeout, sNotify) {
        "use strict";
        var clientVersion = $('[data-version]').attr('data-version'),
            currentVersion,
            timeInterval = 600000;

        setInterval(checkVersion, timeInterval);

        return {
            checkVersion: checkVersion
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


                        $analytics.eventTrack('Version', {
                            category: 'Change',
                            label: 'Version change mismatch popup'
                        });


                        sNotify.alert('Version mismatch, page will refresh');
                        $timeout(function () {
                            window.location.reload(true);
                        }, 150);


                    }
                }
            });
        }

    }
]);
﻿app.factory('sVerChecker', ['$http', '$angularCacheFactory', '$analytics',
    function ($http, $angularCacheFactory, $analytics) {
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
                url: '/Home/Version/',
                contentType: 'application/json',
                success: function (response) {
                    if (response.success) {
                        currentVersion = response.payload;

                        if (clientVersion === currentVersion) {
                            return;
                        }

                        
                        $analytics.eventTrack('Version', {
                            category: 'Change',
                            label: 'Version change mismatch popup'
                        });


                        alert('Version mismatch, page will refresh');
                        $angularCacheFactory.removeAll();
                        window.location.reload(true);



                    }
                }
            });
        }

    }
]);
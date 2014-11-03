"use strict";
app.factory('sVerChecker', ['$http', '$angularCacheFactory',
    function ($http, $angularCacheFactory) {
        var clientVersion = $('[data-version]').attr('data-version'),
            currentVersion,
            timeInterval = 600000;

        setInterval(checkVersion, timeInterval);

        return {
            checkVersion: checkVersion
        };

        function checkVersion() {
            $http.get('/Home/Version/').then(function (response) {
                var data = response.data;
                if (data.success) {
                    currentVersion = data.payload;

                    if (clientVersion !== currentVersion) {
                        alert('Version mismatch, page will refresh');
                        $angularCacheFactory.removeAll();
                        window.location.reload(true);
                    }
                }
            });

        }

    }
]);
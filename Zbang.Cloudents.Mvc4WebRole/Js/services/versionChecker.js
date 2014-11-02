"use strict";
app.factory('sVerChecker', ['$http',
    function ($http) {
        var clientVersion = $('[data-version]').attr('data-version'),
            currentVersion;

        setInterval(checkVersion, 10000);

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
                        window.location.reload(true);
                    }
                }
            });

        }

    }
]);
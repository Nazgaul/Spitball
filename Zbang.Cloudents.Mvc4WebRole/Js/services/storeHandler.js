app.factory('storeHandler',
    ['sModal', 'sUserDetails', 'sLogin', '$q',
    function (sModal, sUserDetails, sLogin, $q) {
        "use strict";

        var data = {},
            cancelled = false;

        return {
            couponPrompt: function () {
                var dfd = $q.defer();

                if (sUserDetails.isAuthenticated() || cancelled) {
                    dfd.reject();
                    return dfd.promise;
                }

                if (data.code) {
                    dfd.resolve(data.code);
                    return dfd.promise;
                }
                
                sModal.open('coupon', {
                    callback: {
                        close: function (response) {
                            if (response.signup) {
                                sLogin.register();
                                return;
                            }

                            if (response.code) {
                                data.code = response.code;
                                dfd.resolve(response.code);
                            }

                        },
                        dismiss: function () {
                            cancelled = true;
                        }
                    }
                });

                return dfd.promise;
            }
        }
    }
    ]);


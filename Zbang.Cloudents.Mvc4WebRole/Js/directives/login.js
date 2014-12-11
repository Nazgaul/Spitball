app.directive('loginBtn',
    ['sLogin','sFacebook',
        function (sLogin,sFacebook) {
            "use strict";
            return {
                restrict: "A",                
                link: function (scope, elem, attrs) {
                    elem.click(function () {
                        switch (attrs.loginBtn) {
                            case 'connect':
                                sLogin.connect();
                                break;
                            case 'register':
                                sLogin.register();
                                break;
                            case 'action':
                                sLogin.registerAction();
                                break;
                            case 'facebook':
                                sFacebook.registerFacebook();
                                break;
                            default:
                                sLogin.connect();
                                break;

                        }
                    });
                }
            };
        }
    ]);

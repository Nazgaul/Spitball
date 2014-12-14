app.directive('loginBtn',
    ['sLogin', 'sFacebook', '$routeParams',
        function (sLogin, sFacebook, $routeParams) {
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
                                sFacebook.registerFacebook({ boxId: $routeParams.boxId });
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

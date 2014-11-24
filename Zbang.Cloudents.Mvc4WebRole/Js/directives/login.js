app.directive('loginBtn',
    ['sLogin',
        function (sLogin) {
            "use strict";
            return {
                restrict: "A",                
                link: function (scope, elem, attrs) {
                    elem.click(function () {
                        switch (attrs.loginBtn) {
                            case 'connect':                                
                                break;
                            case 'register':
                                sLogin.register();
                                break;
                            case 'action':
                                sLogin.registerAction();
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

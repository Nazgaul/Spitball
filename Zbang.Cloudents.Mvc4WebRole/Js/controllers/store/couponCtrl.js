app.controller('CouponCtrl',
    ['$scope', 'Store', '$modalInstance', 'sFacebook', 'sNotify',

    function ($scope, sStore, $modalInstance, sFacebook, sNotify) {
        "use strict";

        $scope.formData = {};

        var buttonDisabled;

        $scope.validateCoupon = function () {
            var invalidCouponMessage = 'קופון שגוי';
            if (!$scope.formData.code) {
                return;
            }

            if (isNaN($scope.formData.code)) {
                sNotify.alert(invalidCouponMessage);
                return;
            }

            if (buttonDisabled) {
                return;
            }

            buttonDisabled = true;
            sStore.validateCoupon({ code: $scope.formData.code}).then(function (response) {
                if (response.isValid) {
                    $modalInstance.close({ code: $scope.formData.code });
                    return;
                }
                sNotify.alert(invalidCouponMessage);
            }).finally(function () {
                buttonDisabled = false;
            });
        };

        $scope.signup = function () {
            $modalInstance.close({ signup: true });
        };

        $scope.facebookLogin = function () {
            sFacebook.registerFacebook();
        };

        $scope.cancel = function () {
            $modalInstance.dismiss();
        };
    }]
);

sStore.controller('CouponCtrl',
    ['$scope','sStore','$modalInstance',

    function ($scope, sStore,$modalInstance) {
        "use strict";

        $scope.formData = {};

        $scope.validateCoupon = function () {
            var invalidCouponMessage = 'קופון שגוי';
            if (!$rootScope.params.store.coupon.code) {
                return;
            }

            var isNumber = /^\d+$/.test($rootScope.params.store.coupon.code);

            if (!isNumber) {
                sNotify.alert(invalidCouponMessage);
                return;
            }

            $rootScope.params.store.coupon.buttonDisabled = true;

            sStore.validateCoupon({ code: parseInt($rootScope.params.store.coupon.code, 10) }).then(function (response) {
                $rootScope.params.store.coupon.buttonDisabled = false;
                if (response.isValid) {
                    $rootScope.params.store.coupon.valid = true;
                    $rootScope.params.store.coupon.code = $rootScope.params.store.coupon.code;
                    sLogin.reset();
                    return;
                }
                sNotify.alert(invalidCouponMessage);
            }).finally(function () {
                $rootScope.params.store.coupon.buttonDisabled = false;
            });
        };
    }]
);

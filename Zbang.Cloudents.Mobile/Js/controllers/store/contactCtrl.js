app.controller('ContactCtrl',
    ['$scope', '$timeout', 'Store', '$analytics', 'sNotify',

    function ($scope, $timeout, Store, $analytics, sNotify) {
        "use strict";
        $timeout(function () {
            $scope.$emit('viewContentLoaded');
        });

        $scope.formData = {};

        $scope.submit = function (isValid) {
            if (!isValid) {
                return;
            }

            $analytics.eventTrack('Store Contact', {
                category: 'Submit Form'
            });


            Store.contact($scope.formData).then(function () {

                sNotify.alert('Thank you');
                $scope.submitted = false;

                $scope.contactForm.$setPristine();
                $scope.formData = {
                    name: null,
                    phone: null,
                    email: null,
                    university: null,
                    text: null
                };

            }, function () {
                sNotify.alert(response);
            });


        };

    }]
);

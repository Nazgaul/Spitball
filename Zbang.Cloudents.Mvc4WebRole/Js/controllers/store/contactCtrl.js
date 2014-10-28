"use strict";
app.controller('ContactCtrl',
    ['$scope', '$timeout','Store',
    function ($scope, $timeout,Store) {
        $timeout(function () {
            $scope.$emit('viewContentLoaded');
        });

        $scope.formData = {};

        $scope.submit = function (isValid) {
            if (!isValid) {
                return;
            }

            Store.contact($scope.formData).then(function (response) {
                if (!response.success) {
                    alert(response.payload);
                    return;
                }

                alert('Thank you');
                $scope.submitted = false;

                $scope.contactForm.$setPristine();
                $scope.formData = {
                    name: null,
                    phone: null,
                    email: null,
                    university: null,
                    text: null
                };

            });


        };

    }]
);

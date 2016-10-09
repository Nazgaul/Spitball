(function () {
    'use strict';
    angular.module('app.user.details').controller('UserDetailsController', userDetailsController);
    userDetailsController.$inject = ['accountService', '$scope', 'userDetailsFactory'];

    function userDetailsController(accountService, $scope, userDetails) {
        var ud = this;
       
        var response = userDetails.get();
        if (response.id) {
            assignValues(response);
            $scope.$on('userDetailsChange',
                function() {
                    assignValues(userDetails.get());
                });
            
            
           
        }
        else {
            ud.signup = function(e) {
                var url = getParameterByName('returnUrl', e.target.href);
                e.target.href = e.target.href.replace(encodeURIComponent(url), encodeURIComponent(window.location.href));
            }
        }
        function assignValues(response) {
            ud.id = response.id;
            ud.name = response.name;
            ud.image = response.image;
            ud.score = response.score;
            ud.url = response.url;
        }
        function getParameterByName(name, url) {
            if (!url) url = window.location.href;
            name = name.replace(/[\[\]]/g, "\\$&");
            var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
                results = regex.exec(url);
            if (!results) return null;
            if (!results[2]) return '';
            return decodeURIComponent(results[2].replace(/\+/g, " "));
        }


    }
})();




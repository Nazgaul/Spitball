(function() {
    angular.module('app').controller('adController', ads);
    ads.$inject = ['ajaxService', 'userDetailsFactory'];

    function ads(ajaxService, userDetailsFactory) {
        var ad = this;
        ad.show = true;
        if (userDetailsFactory.get().university.country.toLowerCase() !== 'il') {
            ad.show = false;
            return;
        }
        ajaxService.get('home/studentad').then(function (response) {
            if (!response) {
                ad.show = false;
            }
            ad.content = response;
        });
    }

})();
app.factory('Store',
    ['ajaxService',

    function (ajaxService) {
        function buildPath(path) {
            return '/Store/' + path + '/';
        }
        return {
            products: function (data) {
                return ajaxService.get(buildPath('Products'), data);
            },
            search: function (data) {
                return ajaxService.get(buildPath('Search'), data);
            },
            order: function (data) {
                return ajaxService.post(buildPath('Order'), data);
            },
            validateCoupon: function (data) {
                return ajaxService.get(buildPath('ValidCodeCoupon'), data);
            },
            contact: function (data) {
                return ajaxService.post(buildPath('Contact'), data, true);
            }
        };
    }
    ]);

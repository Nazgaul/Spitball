app.factory('Store',
    ['ajaxService',

    function (ajaxService) {
        function buildPath(path) {
            return '/store/' + path + '/';
        }
        return {
            products: function (data) {
                return ajaxService.get(buildPath('products'), data);
            },
            search: function (data) {
                return ajaxService.get(buildPath('search'), data);
            },
            order: function (data) {
                return ajaxService.post(buildPath('order'), data);
            },
            validateCoupon: function (data) {
                return ajaxService.get(buildPath('validcodecoupon'), data);
            },
            contact: function (data) {
                return ajaxService.post(buildPath('contact'), data, true);
            }
        };
    }
    ]);

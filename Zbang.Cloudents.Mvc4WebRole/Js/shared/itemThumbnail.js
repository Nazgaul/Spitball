(function () {
    angular.module('app').service('itemThumbnail', i);

    function i() {
        var self = this;
        self.get = function (name, width, height) {
            return 'https://az779114.vo.msecnd.net/preview/' + encodeURIComponent(name) + '.jpg?width=' + width + '&height=' + height + '&mode=crop&scale=both';
        }
    }

})();
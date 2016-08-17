var app;
(function (app) {
    var HideChatOnMobile = (function () {
        function HideChatOnMobile($mdMedia) {
            var _this = this;
            this.$mdMedia = $mdMedia;
            this.restrict = 'A';
            this.link = function (scope, element, attrs) {
                if (_this.$mdMedia('xs')) {
                    element.on(attrs['hideChatOnMobile'], function () {
                        $('html').removeClass('expanded-chat');
                    });
                }
            };
        }
        HideChatOnMobile.factory = function () {
            var directive = function ($mdMedia) {
                return new HideChatOnMobile($mdMedia);
            };
            directive['$inject'] = ['$mdMedia'];
            return directive;
        };
        return HideChatOnMobile;
    }());
    angular
        .module("app.chat")
        .directive("hideChatOnMobile", HideChatOnMobile.factory());
})(app || (app = {}));

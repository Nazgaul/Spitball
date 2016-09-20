var app;
(function (app) {
    'use strict';
    var ItemThumbnailService = (function () {
        function ItemThumbnailService() {
            this.logo = 'https://az32006.vo.msecnd.net/zboxprofilepic/S100X100/universityEmptyState.png';
        }
        ItemThumbnailService.prototype.get = function (name, width, height) {
            return "https://az779114.vo.msecnd.net/preview/" + encodeURIComponent(name) + ".jpg?width=" + width + "&height=" + height + "&mode=crop&scale=both";
        };
        ItemThumbnailService.prototype.assignValues = function (elements, widthElement, heightElement) {
            for (var i = 0; i < elements.length; i++) {
                if (!elements[i].thumbnail) {
                    var retVal = this.assignValue(elements[i].source, widthElement, heightElement);
                    elements[i].thumbnail = retVal.thumbnail;
                }
            }
            return elements;
        };
        ItemThumbnailService.prototype.assignValue = function (source, widthElement, heightElement) {
            widthElement = widthElement || 300;
            heightElement = heightElement || 424;
            var thumbnail = this.get(source, widthElement, heightElement);
            return {
                thumbnail: thumbnail
            };
        };
        ItemThumbnailService.prototype.getChat = function (name) {
            if (!name) {
                return "";
            }
            return "https://az779114.vo.msecnd.net/preview/chat/" + encodeURIComponent(name) + ".jpg?width=256&height=138&scale=both&mode=crop";
        };
        ItemThumbnailService.prototype.getUniversityPic = function (name, width, height) {
            if (!name) {
                name = "DefaultUni5.jpg";
            }
            return "https://az779114.vo.msecnd.net/universities/cover/" + encodeURIComponent(name) + "?mode=crop&cropxunits=100&cropyunits=100&crop=0,12,0,0&anchor=topcenter&width=" + width + "&height=" + height;
        };
        return ItemThumbnailService;
    }());
    angular.module('app').service('itemThumbnailService', ItemThumbnailService);
})(app || (app = {}));

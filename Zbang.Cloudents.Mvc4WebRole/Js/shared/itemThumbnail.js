'use strict';
(function () {
    angular.module('app').service('itemThumbnailService', itemThumbnailService);

    function itemThumbnailService() {
        var self = this;
        self.get = get;

        self.assignValues = assignValues;

        self.assignValue = assignValue;
        self.getChat = getChat;

        function getChat(name) {
            if (!name) {
                return;
            }
            return 'https://az779114.vo.msecnd.net/preview/chat/' + encodeURIComponent(name) + '.jpg?width=256&height=170&scale=both';
        }
        //self.getIcon = getIcon;
        function get(name, width, height) {
            return 'https://az779114.vo.msecnd.net/preview/' + encodeURIComponent(name) + '.jpg?width=' + width + '&height=' + height + '&mode=crop&scale=both';
        }
        function assignValues(elements, widthElement, heightElement) {
            for (var i = 0; i < elements.length; i++) {
                if (!elements[i].thumbnail) {
                    var retVal = assignValue(elements[i].source, widthElement, heightElement);
                    elements[i].thumbnail = retVal.thumbnail;
                }
            }
            return elements;

        }
        function assignValue(source, widthElement, heightElement) {
            widthElement = widthElement || 300;
            heightElement = heightElement || 424;
            var thumbnail = get(source, widthElement, heightElement);

            return {
                thumbnail: thumbnail
            };
        }
       
    }

})();
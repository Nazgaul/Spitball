module app {
    'use strict';
    export interface IItemThumbnailService {
        get(name: string, width: number, height: number): string;
        assignValues(elements: Array<any>, widthElement: number, heightElement: number): Array<any>;
        assignValue(source: string, widthElement: number, heightElement: number): Object;
        getChat(name?: string): string;
        getUniversityPic(name: string, width: number, height: number): string;
        logo: string;
    }

    class ItemThumbnailService implements IItemThumbnailService {
        get(name: string, width: number, height: number) {
            return `https://az779114.vo.msecnd.net/preview/${encodeURIComponent(name)}.jpg?width=${width}&height=${height}&mode=crop&scale=both`;
        }

        assignValues(elements, widthElement, heightElement) {
            for (let i = 0; i < elements.length; i++) {
                if (!elements[i].thumbnail) {
                    const retVal = this.assignValue(elements[i].source, widthElement, heightElement);
                    elements[i].thumbnail = retVal.thumbnail;
                }
            }
            return elements;

        }
        assignValue(source, widthElement, heightElement) {
            widthElement = widthElement || 300;
            heightElement = heightElement || 424;
            const thumbnail = this.get(source, widthElement, heightElement);

            return {
                thumbnail: thumbnail
            };
        }
        getChat(name) {
            if (!name) {
                return "";
            }
            return `https://az779114.vo.msecnd.net/preview/chat/${encodeURIComponent(name)}.jpg?width=256&height=138&scale=both&mode=crop`;
        }
        getUniversityPic(name, width, height) {
            if (!name) {
                name = "DefaultUni5.jpg";
            }
            return `https://az779114.vo.msecnd.net/universities/cover/${encodeURIComponent(name)
                }?mode=crop&cropxunits=100&cropyunits=100&crop=0,12,0,0&anchor=topcenter&width=${width}&height=${height}`;
        }
        logo = 'https://az32006.vo.msecnd.net/zboxprofilepic/S100X100/universityEmptyState.png';
    }
    angular.module('app').service('itemThumbnailService', ItemThumbnailService);

}

//(function () {

//    function itemThumbnailService() {
//        var self = this;
//        //self.get = get;

//        //self.assignValues = assignValues;

//        //self.assignValue = assignValue;
//        //self.getChat = getChat;
//        //self.getUniversityPic = getUniversityPic;
//        self.logo = 'https://az32006.vo.msecnd.net/zboxprofilepic/S100X100/universityEmptyState.png';



//        function getChat(name) {
//            if (!name) {
//                return;
//            }
//            return 'https://az779114.vo.msecnd.net/preview/chat/' + encodeURIComponent(name) + '.jpg?width=256&height=138&scale=both&mode=crop';
//        }
//        //self.getIcon = getIcon;
//        //function get(name, width, height) {
//        //    return 'https://az779114.vo.msecnd.net/preview/' + encodeURIComponent(name) + '.jpg?width=' + width + '&height=' + height + '&mode=crop&scale=both';
//        //}
//        //function assignValues(elements, widthElement, heightElement) {
//        //    for (var i = 0; i < elements.length; i++) {
//        //        if (!elements[i].thumbnail) {
//        //            var retVal = assignValue(elements[i].source, widthElement, heightElement);
//        //            elements[i].thumbnail = retVal.thumbnail;
//        //        }
//        //    }
//        //    return elements;

//        //}
//        //function assignValue(source, widthElement, heightElement) {
//        //    widthElement = widthElement || 300;
//        //    heightElement = heightElement || 424;
//        //    var thumbnail = get(source, widthElement, heightElement);

//        //    return {
//        //        thumbnail: thumbnail
//        //    };
//        //}

//    }

//})();
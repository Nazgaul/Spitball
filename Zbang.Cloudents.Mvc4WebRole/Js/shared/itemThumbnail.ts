module app {
    "use strict";

    interface IThumbnail {
        thumbnail: string;
    }
    export interface IItemThumbnailService {
        get(name: string, width: number, height: number): string;
        assignValues(elements: Array<any>, widthElement: number, heightElement: number): Array<any>;
        assignValue(source: string, widthElement?: number, heightElement?: number): IThumbnail;
        getChat(name?: string): string;
        getUniversityPic(name: string, width: number, height: number): string;
        logo: string;
    }

    class ItemThumbnailService implements IItemThumbnailService {
        get(name: string, width: number, height: number) {
            return `https://az779114.vo.msecnd.net/preview/${encodeURIComponent(name)}.jpg?width=${width}&height=${height}&mode=crop&scale=both`;
        }

        assignValues(elements, widthElement: number, heightElement:number) {
            for (let i = 0; i < elements.length; i++) {
                if (!elements[i].thumbnail) {
                    const retVal = this.assignValue(elements[i].source, widthElement, heightElement);
                    elements[i].thumbnail = retVal.thumbnail;
                }
            }
            return elements;

        }
        assignValue(source, widthElement: number, heightElement: number) {
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
        getUniversityPic(name, width: number, height: number) {
            if (!name) {
                name = "DefaultUni5.jpg";
            }
            return `https://az779114.vo.msecnd.net/universities/cover/${encodeURIComponent(name)
                }?mode=crop&cropxunits=100&cropyunits=100&crop=0,12,0,0&anchor=topcenter&width=${width}&height=${height}`;
        }
        logo = "https://az32006.vo.msecnd.net/zboxprofilepic/S100X100/universityEmptyState.png";
    }
    angular.module("app").service("itemThumbnailService", ItemThumbnailService);

}


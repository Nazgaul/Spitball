(function () {
    angular.module('app').service('itemThumbnailService', itemThumbnailService);

    function itemThumbnailService() {
        var self = this;
        self.get = get;

        self.assignValues = assignValues;

        self.assignValue = assignValue;
        self.getIcon = getIcon;
        function get(name, width, height) {
            return 'https://az779114.vo.msecnd.net/preview/' + encodeURIComponent(name) + '.jpg?width=' + width + '&height=' + height + '&mode=crop&scale=both';
        }
        function assignValues(elements, widthElement, heightElement) {


            for (var i = 0; i < elements.length; i++) {
                var retVal = assignValue(elements[i].source, widthElement, heightElement);
                elements[i].thumbnail = retVal.thumbnail;
                elements[i].icon = retVal.icon;
            }
            return elements;

        }
        function assignValue(source, widthElement, heightElement) {
            widthElement = widthElement || 368;
            heightElement = heightElement || 520;
            var thumbnail = get(source, widthElement, heightElement),
            icon = getIcon(source);

            return {
                thumbnail: thumbnail,
                icon: icon

            };
        }


        function getIcon(source) {
            var prefix = "/images/site/box-icons.svg?1#";
            if (source.startsWith('http://')) {
                return prefix + 'item-link';
            }
            var wordExtension = [".rtf", ".docx", ".doc", ".odt"],
                excelExtension = [".xls", ".xlsx", ".xlsm", ".xltx", ".ods", ".csv"],
                imageExtension = [".jpg", ".gif", ".png", ".jpeg", ".bmp", ".tiff", ".tif"],
                pptExtension = [".ppt", ".pot", ".pps", ".pptx", ".potx", ".ppsx", ".odp", ".pptm"],
                videoExtension = [".3gp", ".3g2", ".3gp2", ".asf", ".mts", ".m2ts", ".mod", ".dv", ".ts", ".vob", ".xesc", ".mp4", ".mpeg", ".mpg", ".m2v", ".ismv", ".wmv"],
                textExtension = [".txt", ".cpp", ".c", ".h", ".manifest", ".vcproj", ".java", ".sql", ".cs", ".css", ".less", ".log", ".vpp", ".xaml", ".xml", ".ini", ".suo", ".sln", ".php", ".js", ".config", ".htm", ".svg", ".html"];

            var extension = source.substr(source.lastIndexOf('.')).toLowerCase();
            if (wordExtension.indexOf(extension) > -1) {
                return prefix + "item-word";
            }
            if (excelExtension.indexOf(extension) > -1) {
                return prefix + "item-excel";
            }
            if (imageExtension.indexOf(extension) > -1) {
                return prefix + "item-image";
            }
            if (extension === ".pdf") {
                return prefix + "item-pdf";
            }
            if (pptExtension.indexOf(extension) > -1) {
                return prefix + "item-ppt";
            }
            if (videoExtension.indexOf(extension) > -1) {
                return prefix + "item-video";
            }
            if (textExtension.indexOf(extension) > -1) {
                return prefix + "item-text";
            }
            if (extension === ".mp3") {
                return prefix + "item-audio";
            }
            return prefix + "item-audio";

        }
    }

})();
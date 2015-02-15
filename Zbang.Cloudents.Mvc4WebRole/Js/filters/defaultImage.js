app.filter('defaultImage',
    [function () {
        "use strict";
        return function (image,type) {
            if (image) {
                return image;
            }
          
            switch (type) { 
                case 'uni':
                    return 'https://az32006.vo.msecnd.net/zboxprofilepic/S100X100/Lib1.jpg';                    
                case 'item':
                    break;


            }
        };
    }
]);
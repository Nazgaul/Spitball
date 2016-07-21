'use strict';
(function () {
    angular.module('app').config(config);
    config.$inject = ['DoubleClickProvider'];
    function config(doubleClickProvider) {
        var topDashboard = 'div-gpt-ad-1461243129238-0', boxMenu = 'div-gpt-ad-1461243129238-1', menu = 'div-gpt-ad-1461243129238-2', topBox = 'div-gpt-ad-1461243129238-3', itemSide = 'div-gpt-ad-1461244713254-0', itemBetween = 'div-gpt-ad-1459950737650-5', topItem = 'div-gpt-ad-1461568291501-0', searchTop = 'div-gpt-ad-1461243129238-6';
        doubleClickProvider.defineSlot('/107474526/Dash_Top_Banner', [[300, 75], [964, 100], [468, 60]], topDashboard)
            .defineSlot('/107474526/Box_Square_Banner', [[234, 60], [220, 200]], boxMenu)
            .defineSlot('/107474526/Box_Top_Banner', [[300, 75], [964, 100], [468, 60]], topBox)
            .defineSlot('/107474526/Dash_Square_Banner', [[234, 60], [220, 200]], menu)
            .defineSlot('/107474526/Item_Side_Banner', [160, 600], itemSide)
            .defineSlot('/107474526/Item_300x250_UTF', [[728, 90], [234, 60], [300, 600]], itemBetween)
            .defineSlot('/107474526/Item_Top_Banner', [[300, 75], [468, 60], [970, 300]], topItem)
            .defineSlot('/107474526/Search_Top_Banner', [[300, 75], [964, 100], [468, 60]], searchTop);
        doubleClickProvider.defineSizeMapping(topDashboard)
            .addSize([1000, 768], [964, 100])
            .addSize([640, 480], [468, 60])
            .addSize([0, 0], [300, 75]);
        doubleClickProvider.defineSizeMapping(topBox)
            .addSize([1000, 768], [964, 100])
            .addSize([640, 480], [468, 60])
            .addSize([0, 0], [300, 75]);
        doubleClickProvider.defineSizeMapping(menu)
            .addSize([960, 650], [220, 200])
            .addSize([0, 0], [234, 60]);
        doubleClickProvider.defineSizeMapping(boxMenu)
            .addSize([960, 650], [220, 200])
            .addSize([0, 0], [234, 60]);
        doubleClickProvider.defineSizeMapping(searchTop)
            .addSize([1000, 768], [964, 100])
            .addSize([640, 480], [468, 60])
            .addSize([0, 0], [300, 75]);
        doubleClickProvider.defineSizeMapping(topItem)
            .addSize([1000, 768], [964, 100])
            .addSize([640, 480], [468, 60])
            .addSize([0, 0], [300, 75]);
        doubleClickProvider.defineSizeMapping(itemBetween)
            .addSize([1000, 768], [964, 100])
            .addSize([640, 480], [468, 60])
            .addSize([0, 0], [300, 75]);
        doubleClickProvider.defineSizeMapping(itemSide)
            .addSize([0, 0], [160, 600]);
    }
})();

/// <reference path="../scripts/typings/angularjs/angular.d.ts" />

(() => {
    angular.module('app').config(config);
    config.$inject = ['DoubleClickProvider'];

    function config(doubleClickProvider) {
        var topDashboard = 'div-gpt-ad-1459950737650-0', //top of dashboard
            boxMenu = 'div-gpt-ad-1459950737650-1',
            menu = 'div-gpt-ad-1459950737650-3',
            topBox = 'div-gpt-ad-1459950737650-2',
            itemSide = 'div-gpt-ad-1459950737650-4',
            itemBetween = 'div-gpt-ad-1459950737650-5',
            topItem = 'div-gpt-ad-1459950737650-6',
            searchTop = 'div-gpt-ad-1459950737650-7';



         doubleClickProvider.defineSlot('/107474526/Dash(N)_728x90_ATF', [[300, 75], [728, 90], [468, 60]], topDashboard)
             .defineSlot('/107474526/Box_250x250_ATF', [[234, 60], [250, 250]], boxMenu)
            .defineSlot('/107474526/Box_728x90_ATF', [[300, 75], [728, 90], [468, 60]], topBox)
            .defineSlot('/107474526/Dash_250x250_ATF', [[234, 60], [250, 250]], menu)
            .defineSlot('/107474526/Item_160x300_Side', [160, 600], itemSide)
            .defineSlot('/107474526/Item_300x250_UTF', [[728, 90], [234, 60], [300, 600]], itemBetween)
            .defineSlot('/107474526/Item_728x90_ATF', [[300, 75], [468, 60], [970, 300]], topItem)
             .defineSlot('/107474526/search_728x90_ATF', [[300, 75], [728, 90], [468, 60]], searchTop);


        doubleClickProvider.defineSizeMapping(topDashboard)
            .addSize([1050, 768], [728, 90])
            .addSize([640, 480], [468, 60])
             .addSize([0, 0], [300, 75]);
        doubleClickProvider.defineSizeMapping(topBox)
            .addSize([1050, 768], [728, 90])
            .addSize([640, 480], [468, 60])
            .addSize([0, 0], [300, 75]);

        doubleClickProvider.defineSizeMapping(menu)
            .addSize([1050, 850], [250, 250])
            .addSize([0, 0], [234, 60]);

        doubleClickProvider.defineSizeMapping(boxMenu)
            .addSize([1050, 850], [250, 250])
            .addSize([0, 0], [234, 60]);

        doubleClickProvider.defineSizeMapping(searchTop)
            .addSize([1050, 768], [728, 90])
            .addSize([640, 480], [468, 60])
            .addSize([0, 0], [300, 75]);

        doubleClickProvider.defineSizeMapping(topItem)
            .addSize([1050, 768], [728, 90])
            .addSize([640, 480], [468, 60])
            .addSize([0, 0], [300, 75]);

        doubleClickProvider.defineSizeMapping(itemBetween)
            .addSize([1050, 768], [728, 90])
            .addSize([640, 480], [468, 60])
            .addSize([0, 0], [300, 75]);

        doubleClickProvider.defineSizeMapping(itemSide)
            .addSize([0, 0], [160, 600]);

        doubleClickProvider.setRefreshInterval(60000);
    }
})();
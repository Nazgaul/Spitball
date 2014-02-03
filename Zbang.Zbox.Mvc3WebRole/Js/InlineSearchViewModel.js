(function (mmc, $) {
    "use strict";
    if (mmc.page.dashboard) {
        mmc.dashboardInlineSearch = function () {
            ko.applyBindings(new InlineSearchViewModel(), document.getElementById('InlineSearch'));
        };
    }
    function InlineSearchViewModel() {
        var self = this,
        searchelem = $('#InlineSearch').find('input[type=text]'),
        placeholder = searchelem.attr('placeholder'),
        pagename = $('div.memberDetails').find('h2').text();
        searchelem.attr('placeholder', placeholder + ' ' + pagename);

        self.search = ko.observable($.bbq.getState('search'));
        //self.isSearchable = ko.observable($.bbq.getState('search') === undefined);

        var typeTime, doneTypeing = 500;

        searchelem.keyup(function () {
            typeTime = setTimeout(function () {
                self.inlineSearch();
            }, doneTypeing);
        });
        searchelem.keydown(function() {
            clearTimeout(typeTime);
        });

        self.inlineSearch = function () {
            //if (self.isSearchable()) {
            // self.isSearchable(false);
            var search = self.search();
            if ($.trim(search) === '') {

                return removeState();
            }
            $.bbq.pushState({ 'search': self.search() });
            //}
            //else {
            //    return removeState();
            //}
        };

        function removeState() {
            $.bbq.removeState('search');
            //self.isSearchable(true);
        }


    }
}(window.mmc = window.mmc || {}, jQuery));
/// <reference path="/Scripts/knockout.js" />
//function SelectedTagsViewModelWrapper() {
(function (mmc, $) {
    "use strict";
    if (mmc.page.dashboard) {
        mmc.selectedTags = function () {
            var self = this;
            ko.applyBindings(new SelectedTagsViewModel(), document.getElementById('TagSelectedSection'));
        };
    }

    function SelectedTagsViewModel() {
        var self = this;
        var Delimiter = ';';
        var HashState = 'tags';

        self.selectedTags = ko.observableArray(Zbox.GetHashStateToArray(HashState, Delimiter));
        self.dontshowSection = ko.computed(function () {
            return self.selectedTags().length === 0;
        }, self);

        self.removeTag = function (tag) {
            self.selectedTags.remove(tag);
            if (self.selectedTags().length) {
                $.bbq.pushState({ 'tags': self.selectedTags().join(Delimiter) });
            }
            else {
                $.bbq.removeState(HashState);
            }
        };

        $(window).bind('hashchange', function (e) {
            self.selectedTags(Zbox.GetHashStateToArray(HashState, Delimiter));
        });
    }
}(window.mmc = window.mmc || {}, jQuery));
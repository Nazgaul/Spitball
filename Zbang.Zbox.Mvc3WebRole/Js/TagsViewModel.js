/// <reference path="/Scripts/knockout.js" />
(function (mmc, $) {
    "use strict";
    if (mmc.page.dashboard) {
        mmc.dashboardTags = function () {
            var self = this;
            ko.applyBindings(new TagViewModel(), document.getElementById('TagsSection'));
        };
    }
    function TagViewModel() {
        var self = this;
        var Delimiter = ';';
        var HashState = 'tags';

        function UserTag(data) {
            var self = this;
            self.name = data.Name;
            self.count = data.Count;
        }
        self.tags = ko.observableArray([]);

        self.changeState = function (tag) {
            var tags = Zbox.GetHashStateToArray(HashState, Delimiter);
            var exists = ko.utils.arrayFirst(tags, function (t) {
                return t === tag.name;
            });
            if (exists) {
                return;
            }
            tags.push(tag.name);

            $.bbq.pushState({ 'tags': tags.join(Delimiter) });
        };
        self.moreTags = function () {
            page++;
            tagsList(true);
        };

        var friendId = location.pathname.split('/')[2];
        var search = $.bbq.getState('search');
        var tags = Zbox.GetHashStateToArray(HashState, Delimiter);
        var page = 0;
        self.moreResultExists = ko.observable(true);
        var cache = {};

        tagsList();
        registerEvents();
        function tagsList(moreTags) {
            var term = JSON.stringify({ friendUid: friendId, tags: tags, search: search, page: page });

            if (term in cache) {
                generateModel(moreTags, cache[term]);
            }
            var request = new ZboxAjaxRequest({
                url: "/Dashboard/TagsList",
                contentType: 'application/json; charset=utf-8',
                data: term,
                success: function (data) {
                    var mappedtags = $.map(data, function (tag) { return new UserTag(tag); });
                    cache[term] = mappedtags;
                    generateModel(moreTags, mappedtags);
                }
            });
            request.Post();
        }
        function generateModel(moreTags, data) {
            if (data.length < 15) {
                self.moreResultExists(false);
            }
            if (moreTags) {
                self.tags.push.apply(self.tags, data);
                return;
            }
            self.tags(data);

        }

        function registerEvents() {
            $(window).bind('hashchange', function (e) {
                tags = Zbox.GetHashStateToArray(HashState, Delimiter);
                search = $.bbq.getState('search');
                self.moreResultExists(true);
                page = 0;
                tagsList();
            });

            var cache = {}, lastXhr;
            $('#tagFiller').autocomplete({
                delay: 500,
                source: function (request, response) {
                    var term = request.term;
                    if (term in cache) {
                        response(cache[term]);
                        return;
                    }
                    var request2 = new ZboxAjaxRequest({
                        url: "/Box/Tags",
                        data: request,
                        success: function (data, status, xhr) {
                            if (xhr === lastXhr) {
                                var filterdata = data;
                                cache[term] = filterdata;
                                response(filterdata);
                            }
                        }
                    });
                    request2.Post();
                },
                select: function (event, ui) {
                    var tag = new UserTag({ Name: ui.item.label, Count: 0 });

                    //self.selectedTags.push(tag);
                    // getData();
                    self.changeState(tag);
                    $('#tagFiller').val('');
                    return false;//due to autocomplete event
                }
            });
        }
    }
}(window.mmc = window.mmc || {}, jQuery));
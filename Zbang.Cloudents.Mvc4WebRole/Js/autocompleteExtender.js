(function ($) {
    var proto = $.ui.autocomplete.prototype,
        create = proto._create,
    suggets = proto._suggest,
    renderItem = proto._renderItem;

    $.extend(proto, {
        _renderItem: function (ul, item) {
            if (item.type) {
                return $("<li>").addClass("ui-menu-item new").append($("<span>").text(item.label))
                    .append($("<button>").addClass("btn1").text(ZboxResources.CreateNew)
                    )
                    .appendTo(ul);
            }
            return renderItem.call(this, ul, item);
            //return $("<li>").append($("<a>").text(item.label)).appendTo(ul);

        },
        _create: function () {
            create.call(this);
            var that = this;
            var elem = this.options.appendTo || 'body';
            $(elem).on('click', '.ui-menu-item>button', function (e) {
                that.menu.select(e);
            });
        }
        //,
        //_suggest: function (items) {
        //    suggets.call(this, items);
        //    this.menu.element.hide().slideDown();
        //}
    });


})(jQuery)
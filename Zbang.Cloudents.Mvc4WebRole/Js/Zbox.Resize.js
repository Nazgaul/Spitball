Resize = function () {
};
Resize.ClaculateHeightByOffSet = function (elem) {
    var height = 0;
    height += $('#footer').outerHeight(true);
    height += elem.offset().top;

    /*height += parseInt(elem.css('margin-top').replace('px', ''));*/
    height += parseInt(elem.css('padding-top').replace('px', ''));
    height += parseInt(elem.css('padding-bottom').replace('px', ''));
    height += parseInt(elem.css('margin-bottom').replace('px', ''));
    var x = parseInt(elem.css('border-top-width').replace('px', ''));
    if (!isNaN(x))
        height += x;

    return height;
};
Resize.HeightChange = function (elem, extraHeight) {
    var height = this.ClaculateHeightByOffSet($(elem));
    height += (extraHeight || 0);
    Resize.HeightCalculate(height, elem);
    elem.addClass('overflowy');
    $(window).resize(function () {
        Resize.HeightCalculate(height, elem);
    });
    $(elem).bind('HeightChange', function (event, newHeight) {
        Resize.HeightCalculate(height + newHeight, elem);
    });
};
Resize.MainContentHeightChange = function () {
    var $MainContent = $('#MainContent');
    this.HeightChange($MainContent);
};



Resize.HeightCalculate = function (height, elem) {
    var elemHeight = $(window).height() - height;
    elem.height(elemHeight);
    elem.css('max-height', elemHeight);
};


Resize.CommentHeightChange = function () {
    var $CommentList = $('#CommentListWrapper');
    this.HeightChange($CommentList);
};


Resize.CommentItemHeightChange = function () {
    var $CommentList = $('#CommentList');
    this.HeightChange($CommentList);
};

Resize.ItemHeightChage = function () {
    var $ItemViewside = $('#ItemViewside');
    this.HeightChange($ItemViewside);
};


Resize.TagsHeightChage = function () {
    var $leftTreeWrap = $('.leftTreeWrap');
    this.HeightChange($leftTreeWrap);
};
/*
Resize.boxSetWrapHeightChange = function () {
    var $boxSetWrap = $('#boxSetWrap');
    this.HeightChange($boxSetWrap);
};
*/




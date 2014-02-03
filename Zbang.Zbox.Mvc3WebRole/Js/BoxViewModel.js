(function (mmc, $) {
    "use strict";
    var box = false;
    if (!mmc.page.box) {
        return;
    }
    mmc.box = function () {
        box = true;
        var self = this;
        var boxid = Zbox.getParameterByName('BoxUid');
        if (boxid === '') {
            throw 'boxid cannot be null';
        }

        registerEvents();
        //var lastSync = new Date();
        //var update = function () {
        //    var request = new ZboxAjaxRequest({
        //        url: "/Box/Updates",
        //        contentType: 'application/json; charset=utf-8',
        //        data: JSON.stringify({ BoxUid: boxid, UserLastSync: lastSync }),
        //        success: function (data) {
        //            if (data.Comments.length) {
        //                mmc.UpdateComments(data.Comments);
        //            }
        //            if (data.Items.length) {
        //                mmc.UpdateBoxItemModel(data.Items);
        //            }
        //            lastSync = new Date();
        //        }
        //    });
        //    request.Post();
        //};

        //var interval = window.setInterval(update, 15000);

        //$(document).idleTimer(30000);
        //$(document).bind("idle.idleTimer", function () {
        //    window.clearInterval(interval);
        //});
        //$(document).bind("active.idleTimer", function () {
        //    var timediff = new Date() - lastSync;
        //    if (timediff > 15000) {
        //        //   update();
        //    }
        //    interval = window.setInterval(update, 15000);
        //});

        function registerEvents() {

            //$('#esUpload').click(function () {
            // $('li[data-strap="upload"]').click();
            //});
            //$('#esLink').click(function () {
            // $('li[data-strap="link"]').click();
            //});
            if (!sec.ConnectedToBox()) {
                $('#FollowBox').show();
                $('#bFollowBox').click(function () {
                    var request = new ZboxAjaxRequest({
                        url: "/Share/SubscribeToBox",
                        data: { BoxUid: boxid },
                        success: function () {
                            window.location.reload();
                        }
                    });
                    request.Post();
                });
                $('#bDecline').click(function () {
                    var request = new ZboxAjaxRequest({
                        url: "/Share/DeclineInvatation",
                        data: { BoxUid: boxid },
                        success: function () {
                            window.location.replace('/');
                        },
                        error: function () {
                            $('#FollowBox').hide();
                        }
                    });
                    request.Post();
                });
            }
            if (mmc.register) {
                //regular page
                $('div.addthis_toolbox').find('a').click(function () { Boxes.ChangeBoxPrivacy(); });
            }
        }
    };


    
    /*tags popup section */
    //var $tagsElem;
    //mmc.tags = function (elem) {
    //    $tagsElem = $(elem);
    //}
    //mmc.moretags = function (elem) {
    //    $(elem).click(function () {
    //        $tagsElem.show();
    //        mmc.modal(function () {
    //            $tagsElem.hide();
    //        }, 'tags');
    //    });

    //}


}(window.mmc = window.mmc || {}, jQuery))
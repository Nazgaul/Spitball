(function (mmc, $) {
    "use strict";
    if (!mmc.page.box) {
        return;
    }
    mmc.box = function () {
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
            $('#downloadAll').click(function () {
                if (!$('#BoxItemList').children(':not(.boxAddItem)').length) {
                    mmc.notification('need to have items in that box');
                    return false;
                }
            });

            $('#removeBox').click(function () {
                if (!confirm("Are you sure you want to delete this box?")) {
                    return;
                }
                new ZboxAjaxRequest({
                    url: "/box/delete",
                    data: { BoxUid: boxid, userType: $('#DUbox').attr('data-connection') },
                    success: function () {
                        window.location = '/';
                    }
                }).Post();
            });
            if (!sec.ConnectedToBox()) {
                var $followBox = $('#FollowBox');
                $followBox.show();
                $('#bFollowBox').click(function () {
                    new ZboxAjaxRequest({
                        url: "/Share/SubscribeToBox",
                        data: { BoxUid: boxid },
                        success: function () {
                            window.location.reload();
                        }
                    }).Post();
                });
                $('#bDecline').click(function () {
                    if (!mmc.register) {
                        $followBox.hide();
                        return;
                    }
                    new ZboxAjaxRequest({
                        url: "/Share/DeclineInvatation",
                        data: { BoxUid: boxid },
                        success: function () {
                            window.location.replace('/');
                        },
                        error: function () {
                            $followBox.hide();
                        }
                    }).Post();
                });
            }
            if (mmc.register) {
                //regular page
                $('div.addthis_toolbox').find('a').click(function () { Boxes.ChangeBoxPrivacy(); });
            }
        }
    };
}(window.mmc = window.mmc || {}, jQuery))
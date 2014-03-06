(function (cd, analytics, ZboxResources) {
    "use strict";


    if (window.scriptLoaded.isLoaded('bts')) {
        return;
    }

    $(function () {
        cd.putPlaceHolder();
    });

    (function () {
        //cd.loader.registerFacebook();
        //$('#logOut').click(function (e) {
        //});


        //drop downs
        var slideSpeed = 150;
        var $userMenu = $('ul.userMenu');
        $('[data-menu="user"]').click(function (e) {
            $('#invitesList').slideUp(); //close invite - maybe should be in class
            if ($userMenu.is(':visible')) {
                $userMenu.slideUp(slideSpeed);
                return;
            }
            //e.stopPropagation();
            e.stopImmediatePropagation();
            $userMenu.slideDown(slideSpeed);
        });
        $('body').click(function () {
            $userMenu.slideUp(slideSpeed);
        });

        $userMenu.find('[data-navigation="account"]').click(function () {
            analytics.trackEvent('User Menu', 'Settings', 'User clicked settings on the user menu');
        });



        textDirection();
        function textDirection() {
            //wikipedia
            var rtlChars = '\u0600-\u06FF' + '\u0750-\u077F' + '\u08A0-\u08FF' + '\uFB50-\uFDFF' + '\uFE70-\uFEFF';//arabic
            rtlChars += '\u0590-\u05FF' + '\uFB1D-\uFB4F';//hebrew

            var controlChars = '\u0000-\u0020';
            controlChars += '\u2000-\u200D';

            //Start Regular Expression magic
            var reRTL = new RegExp('[' + rtlChars + ']', 'g');
            var reNotRTL = new RegExp('[^' + rtlChars + controlChars + ']', 'g');


            $("body").on('keypress', 'textarea', function () {
                var value = $(this).val();
                for (var i = 0; i < value.length; i++) {

                }
                var rtls = value.match(reRTL);
                if (rtls !== null)
                    rtls = rtls.length;
                else
                    rtls = 0;

                var notrtls = value.match(reNotRTL);
                if (notrtls !== null)
                    notrtls = notrtls.length;
                else
                    notrtls = 0;

                if (rtls > notrtls)
                    $(this).css('direction', 'rtl').css('text-align', 'right');
                else
                    $(this).css('direction', 'ltr').css('text-align', 'left');
            });
            $(document).on('submit', 'form', function () {
                $(this).find('textarea').css('direction', '').css('text-align', '');
            });

        }

        //copy link
        //var clipboard = window.clipboardData, flash = true;
        //$(document).on('click', '.copylk', function () {
        //    //$('.copylk').click(function () {
        //    if (clipboard) {
        //        clipboardData.setData('Text', location.href);
        //        cd.notification(ZboxResources.UrlCopied);
        //        trackCopyLink();
        //        return;
        //    }
        //    if (!flash) {
        //        alert('Install flash');
        //    }
        //});
        //if (!clipboard) {
        //    initClipboard();
        //}
        //var boxClip, itemClip; 
        //cd.pubsub.subscribe('destroy_clipboard', function (d) {
        //    var id = d.attr('id');
        //    if (id === 'box_CL') {
        //        boxClip = null;
        //    } else if (id === 'item_CL') {                
        //        itemClip = null;
        //    }
        //    if (window.clipboardData)
        //        d.off('click');
        //});
        //cd.pubsub.subscribe('init_clipboard', function (d) {
        //    initClipboard(d);

        //});
        //function initClipboard(d) {            
        //    if (window.clipboardData) {
        //        $(d).click(function () {
        //            clipboardData.setData('Text', cd.location());
        //            trackCopyLink();
        //            cd.notification(ZboxResources.UrlCopied);
        //        });
        //        return;
        //    }
        //    var temp, id = d.attr('id');
        //    temp = new ZeroClipboard(d, {
        //        moviePath: "/Scripts/ZeroClipboard.swf",
        //        useNoCache: false
        //    });


        //    var elements = document.querySelectorAll('.copylk');
        //    for (var i = 0; i < elements.length; i++) {
        //        elements[i].setAttribute('init', 'true');
        //    }
        //    temp.on('noflash', function () {
        //        //flash = false;
        //    });
        //    temp.on('dataRequested', function () {
        //        var cp = boxClip || itemClip;
        //        cp.setText(cd.location());
        //    });
        //    temp.on('complete', function () {
        //        //trackEvent('copy link');
        //        trackCopyLink();
        //        $(d).removeClass('zeroclipboard-is-hover');
        //        $('body').click();
        //        cd.notification(ZboxResources.UrlCopied);
        //    });
        //    temp.glue(d[0]);
        //    if (id === 'box_CL') {
        //        boxClip = temp;
        //    } else if (id === 'item_CL') {
        //        itemClip = temp;
        //    }

        //    temp = null;
        //}
        //function trackCopyLink() {
        //    analytics.trackEvent('Share', 'Copy link');
        //}



        //tooltip
        //.on('mouseenter', '.tooltip', function () {
        //    timerTooltip = window.setTimeout(function () {

        //    });
        //});

        //resize
        var timer = 0;
        var resizeFunc = cd.debounce(function () {
            cd.pubsub.publish('windowChanged');
        },50);
        $(window).resize(resizeFunc);

        //search
        //var g_searchQ = document.getElementById('g_searchQ'), timer2 = 0;
        //g_searchQ.onclick = function (e) {
        //    if (!cd.register()) {
        //        cd.unregisterAction(this);
        //        return;
        //    }
        //};
        //$('#g_search').submit(function (e) {
        //    e.preventDefault();
        //    this.querySelector('button').disabled = 1;
        //    analytics.trackEvent('Search', inputVal);

        //    var inputVal = g_searchQ.value;
        //    if (!Modernizr.input.placeholder) {
        //        if (g_searchQ.value === g_search.getAttribute('placeholder')) {
        //            g_searchQ.value = '';
        //        }
        //    }
        //    clearTimeout(timer2);
        //    if (!inputVal) {
        //        cd.pubsub.publish('nav', 'dashboard');
        //        return;
        //    }

        //    cd.pubsub.publish('nav', 'dashboard/search/' + encodeURIComponent(inputVal));
        //});
        //$(g_searchQ).keyup(function () {
        //    var $this = $(this);
        //    clearTimeout(timer2);
        //    timer2 = setTimeout(function () {
        //        $this.parent('form').submit();
        //    }, 300);
        //});


        //closeDialog
        $(document).on('click', '[data-closeDiag]', function () {
            var form = this.parentNode.querySelector('form');
            if (form) {
                cd.resetForm(form);
            }

            $(this).parents('[data-diag]').hide();
        })

        //contenteditable
        .on('keypress', '[contenteditable=true]', function (e) {
            if (e.keyCode === 13) {
                e.preventDefault();
            }
        })
        //textarea elastic
        .on('focus', 'textarea', function () {
            if ($(this).val() === $(this).attr('placeholder')) {
                $(this).val(''); //ie issue with elastic
            }
            $(this).elastic();

        });
    })();

    ////regPopup


    //cd.pubsub.subscribe('register', function () {
    //    registerpopup.dialog('show');
    //});
    //var registerpopup = $('#regPopup').dialog({
    //    submitCallBack: function () {
    //        location.href = '/account';
    //    }
    //});


})(cd, cd.analytics, ZboxResources);

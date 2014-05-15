(function (cd, analytics, dataContext) {
    "use strict";
    if (window.scriptLoaded.isLoaded('events')) {
        return;
    }
    var $window = $(window),
        $document = $(document),
        $body = $('body'),
        $html = $('html');

    //#region dropdowns
    $window.unload(function () {  //firefoxfix
        $('[data-ddcbox]').prop('checked', false);
    });
    $body.on('click', function (e) {
        var $target = $(e.target);

        if (e.target.nodeName === "LABEL") {
            return;
        }

        if ($target.attr('data-dropdown') || $target.parents('[data-dropdown]').length > 0) {
            return;
        }

        $('[data-ddcbox]').not('[data-ddcbox="' + $target.attr('data-ddcbox') + '"]').prop('checked', false);
        //$('[data-dropdown]').not('[data-dropdown="' + $target.attr('data-ddcbox') + '"]').removeClass('showOtakim');

        if ($target.attr('data-ddcbox') === undefined) {
            $('[data-ddcbox]').prop('checked', false);
            //$('.dropDown').removeClass('showOtakim');
        }
    });

    //#region dropdown
    var slideSpeed = 150;
    var $userMenu = $('ul.userMenu');
    $('[data-menu="user"]').click(function (e) {
        $('#notifyWpr').slideUp(); //close invite - maybe should be in class
        if ($userMenu.is(':visible')) {
            $userMenu.slideUp(slideSpeed);
            return;
        }
        //e.stopPropagation();
        e.stopImmediatePropagation();
        $userMenu.slideDown(slideSpeed);
    });
    $body.click(function () {
        $userMenu.slideUp(slideSpeed);
    });

    $userMenu.find('[data-navigation="account"]').click(function () {
        analytics.trackEvent('User Menu', 'Settings', 'User clicked settings on the user menu');
    });
    //#endregion
    //#endregion

    //#region window resize 
    var resizeFunc = cd.debounce(function () {
        cd.pubsub.publish('windowChanged');
    }, 50);
    $window.resize(resizeFunc);
    //#endregion

    //#region close dialog
    $document.on('click', '[data-closeDiag]', function () {
        var form = this.parentNode.querySelector('form');
        if (form) {
            cd.resetForm(form);
        }

        $(this).parents('[data-diag]').hide();
    });
    //#endregion

    //#region contenteditable
    $document.on('keypress', '[contenteditable=true]', function (e) {
        if (e.keyCode === 13) {
            e.preventDefault();
        }
    });
    //#endregion

    //#region textarea elastic
    $document.on('focus', 'textarea', function () {
        if ($(this).val() === $(this).attr('placeholder')) {
            $(this).val(''); //ie issue with elastic
        }
        $(this).elastic();

    });
    //#endregion

    //#region placeholder
    $(function () {
        cd.putPlaceHolder();
    });

    //#endregion

    //#region text direction
    //wikipedia
    var rtlChars = '\u0600-\u06FF' + '\u0750-\u077F' + '\u08A0-\u08FF' + '\uFB50-\uFDFF' + '\uFE70-\uFEFF';//arabic
    rtlChars += '\u0590-\u05FF' + '\uFB1D-\uFB4F';//hebrew

    var controlChars = '\u0000-\u0020';
    controlChars += '\u2000-\u200D';

    //Start Regular Expression magic
    var reRTL = new RegExp('[' + rtlChars + ']', 'g'),
        reNotRTL = new RegExp('[^' + rtlChars + controlChars + ']', 'g'),
        textAlign = $('html').css('direction') === 'ltr' ? 'left' : 'right';

    function checkRTLDirection(value) {

        if (!value) {
            return;
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

        return rtls > notrtls;
    }
    $body.on('keyup', 'textarea', function () {
        if (checkRTLDirection(this.value)) {
            $(this).css('direction', 'rtl').css('text-align', 'right');
        } else {
            $(this).css('direction', 'ltr').css('text-align', 'left');
        }
    });

    $document.on('submit', 'form', function () {
        $(this).find('textarea').css('direction', '').css('text-align', '');
    });

    var setElementDirection = function (element) {
        if (checkRTLDirection(element.textContent)) {
            $(element).css({ direction: 'rtl', 'text-align': textAlign });
        } else {
            $(element).css({'direction': 'ltr', 'text-align': textAlign });
        }
    }

    cd.setElementDirection = setElementDirection;
    //#endregion

    //#region hover intent
    (function ($) {
        $.fn.hoverIntent = function (handlerIn, handlerOut, selector) {

            // default configuration values
            var cfg = {
                interval: 100,
                sensitivity: 7,
                timeout: 0
            };

            if (typeof handlerIn === "object") {
                cfg = $.extend(cfg, handlerIn);
            } else if ($.isFunction(handlerOut)) {
                cfg = $.extend(cfg, { over: handlerIn, out: handlerOut, selector: selector });
            } else {
                cfg = $.extend(cfg, { over: handlerIn, out: handlerIn, selector: handlerOut });
            }

            // instantiate variables
            // cX, cY = current X and Y position of mouse, updated by mousemove event
            // pX, pY = previous X and Y position of mouse, set by mouseover and polling interval
            var cX, cY, pX, pY;

            // A private function for getting mouse position
            var track = function (ev) {
                cX = ev.pageX;
                cY = ev.pageY;
            };

            // A private function for comparing current and previous mouse position
            var compare = function (ev, ob) {
                ob.hoverIntent_t = clearTimeout(ob.hoverIntent_t);
                // compare mouse positions to see if they've crossed the threshold
                if ((Math.abs(pX - cX) + Math.abs(pY - cY)) < cfg.sensitivity) {
                    $(ob).off("mousemove.hoverIntent", track);
                    // set hoverIntent state to true (so mouseOut can be called)
                    ob.hoverIntent_s = 1;
                    return cfg.over.apply(ob, [ev]);
                } else {
                    // set previous coordinates for next time
                    pX = cX; pY = cY;
                    // use self-calling timeout, guarantees intervals are spaced out properly (avoids JavaScript timer bugs)
                    ob.hoverIntent_t = setTimeout(function () { compare(ev, ob); }, cfg.interval);
                }
            };

            // A private function for delaying the mouseOut function
            var delay = function (ev, ob) {
                ob.hoverIntent_t = clearTimeout(ob.hoverIntent_t);
                ob.hoverIntent_s = 0;
                return cfg.out.apply(ob, [ev]);
            };

            // A private function for handling mouse 'hovering'
            var handleHover = function (e) {
                // copy objects to be passed into t (required for event object to be passed in IE)
                var ev = jQuery.extend({}, e);
                var ob = this;

                // cancel hoverIntent timer if it exists
                if (ob.hoverIntent_t) { ob.hoverIntent_t = clearTimeout(ob.hoverIntent_t); }

                // if e.type == "mouseenter"
                if (e.type == "mouseenter") {
                    // set "previous" X and Y position based on initial entry point
                    pX = ev.pageX; pY = ev.pageY;
                    // update "current" X and Y position based on mousemove
                    $(ob).on("mousemove.hoverIntent", track);
                    // start polling interval (self-calling timeout) to compare mouse coordinates over time
                    if (ob.hoverIntent_s != 1) { ob.hoverIntent_t = setTimeout(function () { compare(ev, ob); }, cfg.interval); }

                    // else e.type == "mouseleave"
                } else {
                    // unbind expensive mousemove event
                    $(ob).off("mousemove.hoverIntent", track);
                    // if hoverIntent state is true, then call the mouseOut function after the specified delay
                    if (ob.hoverIntent_s == 1) { ob.hoverIntent_t = setTimeout(function () { delay(ev, ob); }, cfg.timeout); }
                }

                $(ob).off('click.title').on('click.title',function () {
                    ob.hoverIntent_t = clearTimeout(ob.hoverIntent_t);
                    hideTooltipTitle();
                });
            };

        

            // listen for mouseenter and mouseleave
            return this.on({ 'mouseenter.hoverIntent': handleHover, 'mouseleave.hoverIntent': handleHover }, cfg.selector);

            //listen for mouseclick
        
        };
    })(jQuery);
    //#endregion

    //#region mouse position

    cd.mouse = {};
    $document.mousemove(function (e) {
        cd.mouse.posX = e.pageX;
        cd.mouse.posY = e.pageY;
        cd.mouse.target = e.target;        
    });

    //#endregion 

    //#region title tooltip    
    
    $(document).hoverIntent({
        over: showTooltipTitle,
        out: hideTooltipTitle,
        selector: '[data-title]',
        timeout: 100,
        interval: 500
    });

    
    function showTooltipTitle(e) {        
    
        var $element = $(this),
            tooltipTitle = $element.attr('data-title'),
            $html = $(cd.attachTemplateToData('titleToolTipTempalte', { title: tooltipTitle })),
            $arrow = $html.find('.ttArrow'), arrowMargin = 15, offsetY = 3,
            pos;
        $body.append($html);
        pos = calcualtePosition();
        $html.css('top', pos.y + this.offsetHeight).css('left', pos.x);
        cd.setElementDirection($html[0]);

        function calcualtePosition() {
            var elemPos = cd.getElementPosition($element[0]),
                screenWidth = $('html').width(), screenHeight = $('html').height(), positionX,
                tooltipWidth = $html.outerWidth(), triggerWidth = $element.outerWidth(),
                middle = tooltipWidth / 2 - triggerWidth / 2;

            if (tooltipWidth > elemPos.left) {
                positionX = elemPos.left - (arrowMargin + $arrow.outerWidth() / 2) + triggerWidth / 2;
                $arrow.css({ 'left': arrowMargin + 'px', right: 'auto' });
            } else if (screenWidth - elemPos.left < tooltipWidth) {
                positionX = elemPos.left - tooltipWidth + (arrowMargin + $arrow.outerWidth() / 2) + triggerWidth / 2;
                $arrow.css({ right: arrowMargin + 'px', left: 'auto' });
            } else {
                positionX = elemPos.left - middle;

            }

            //scroll fix
            var scrollTop = 0;
            if ($element.parents('.siteHeader').length > 0 ||
                $element.parents('.boxTop').length > 0) {

                scrollTop = $window.scrollTop();
            }
            //

            return { x: positionX - $window.scrollLeft(), y: elemPos.top + scrollTop + offsetY };
        }
    }

    cd.pubsub.subscribe('clearTooltip', function () {
        hideTooltipTitle();
    });

    function hideTooltipTitle() {
        $('.titleTooltip').remove();
    }

    //#endregion

    //#region scroll
    $document.on("scroll", function () {
        //#region tooltips 
        cd.pubsub.publish('clearTooltip');

        //#endregion

        //#region scroll effect
        if ($window.scrollTop() > 0) {
            $html.addClass('scrolling');

        } else {
            $html.removeClass('scrolling');

        }
        //#endregion


    });
    //#endregion scrolling

    //#region new to the user
    (function () {
        var updates, updating = false,
            userId = cd.userDetail().nId;

        cd.pubsub.subscribe('getUpdates', function () {
            getData();
        });

        cd.newUpdates = {};

        function deleteUpdate(update) {
            if (!updates) {
                return;
            }
            var updateIndex;

            update.boxId = parseInt(update.boxId, 10);

            if (update.annotationId) {
                update.itemId = parseInt(update.itemId, 10);
                var annotations = updateIndex = updates[userId][update.boxId].annotations;
                updateIndex = annotations[update.itemId].indexOf(update.annotationId);
                annotations[update.itemId].splice(updateIndex, 1);
                return;
            }

            if (update.itemId || update.quizId) {
                update.id = parseInt(update.id, 10);
            }
            updateIndex = updates[userId][update.boxId][update.type].indexOf(update.id);
            updates[userId][update.boxId][update.type].splice(updateIndex, 1);
        }

        function deleteUpdates(boxId) {

            dataContext.deleteUpdates({
                data: { boxId: boxId }
            });

        }

        function deleteLocalUpdates(boxId) {
            boxId = parseInt(boxId, 10);
            if (updates[userId][boxId]) {
                updates[userId][boxId] = null;
            }
        }

        function getData() {
            if (updates && updates[userId]) {
                if (new Date().getTime() - updates[userId].ttl > cd.OneDay) {
                    updates = null;
                    getData();
                    return;
                }
                cd.pubsub.publish('updates', updates[userId])
                return;
            }

            if (updating) {
                return;
            }
            updating = true;
            dataContext.newUpdates({
                success: function (data) {
                    data = data || [];
                    parseData(data);
                    cd.pubsub.publish('updates', updates[userId])
                }
            });

            function parseData(data) {
                updates = {};
                updates[userId] = {};

                if (!data.length) { //no updates
                    return;
                }

                var currentUpdate;
                for (var i = 0, l = data.length; i < l; i++) {
                    currentUpdate = data[i];

                    addUpdate(currentUpdate)
                }

                updates[userId].ttl = new Date().getTime();

            }
        }
        function addUpdate(update) {
            if (!updates) {
                return;
            }
            update.boxId = parseInt(update.boxId, 10);

            if (!updates[cd.userDetail().nId][update.boxId]) {
                updates[userId][update.boxId] = {};
                updates[userId][update.boxId].items = [];
                updates[userId][update.boxId].quizzes = [];
                updates[userId][update.boxId].questions = [];
                updates[userId][update.boxId].answers = [];
                updates[userId][update.boxId].annotations = {};
            }
            if (update.itemId) {
                if (updates[userId][update.boxId].items.indexOf(update.itemId) === -1) {
                    updates[userId][update.boxId].items.push(parseInt(update.itemId, 10));
                }
            } else if (update.annotationId) {
                update.itemId = parseInt(update.itemId, 10);
                if (!updates[userId][update.boxId].annotations[update.itemId]) {
                    updates[userId][update.boxId].annotations[update.itemId] = [];
                }
                if (updates[userId][update.boxId].annotations[update.itemId].indexOf(update.annotationId) === -1) {
                    updates[userId][update.boxId].annotations[update.itemId].push(update.annotationId);
                }
            } else if (update.quizId) {
                if (updates[userId][update.boxId].quizzes.indexOf(update.quizId) === -1) {
                    updates[userId][update.boxId].quizzes.push(parseInt(update.quizId, 10));
                }
            } else if (update.questionId) {
                if (updates[userId][update.boxId].questions.indexOf(update.questionId) === -1) {
                    updates[userId][update.boxId].questions.push(update.questionId);
                }

            } else if (update.answerId) {
                if (updates[userId][update.boxId].answers.indexOf(update.answerId) === -1) {
                    updates[userId][update.boxId].answers.push(update.answerId);
                }

            } else {
                //no update error?
            }
        }

        cd.newUpdates.deleteAll = deleteUpdates;
        cd.newUpdates.deleteLocalUpdates = deleteLocalUpdates;
        cd.newUpdates.deleteUpdate = deleteUpdate;
        cd.newUpdates.addUpdate = addUpdate //signalR
    })();


    //#endregion
})(cd, cd.analytics, cd.data);

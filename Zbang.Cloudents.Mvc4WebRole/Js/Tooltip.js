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

            $(ob).off('click.title').on('click.title', function () {
                ob.hoverIntent_t = clearTimeout(ob.hoverIntent_t);
                hideTooltipTitle();
            });
        };



        // listen for mouseenter and mouseleave
        return this.on({ 'mouseenter.hoverIntent': handleHover, 'mouseleave.hoverIntent': handleHover }, cfg.selector);

        //listen for mouseclick

    };
})(jQuery);
(function (cd, dataContext, $) {
    "use strict";
    if (window.scriptLoaded.isLoaded('t')) {
        return;
    }
    var tooltipHTML, tooltip,
        consts = { OFFSETX: 5, OFFSETY: 5 };


    $(document).hoverIntent({
        over: getTooltipData,
        out: hideTooltip,
        selector: '.calloutTrgr',
        timeout: 200,
    });

    function TooltipData(data) {
        var that = this;
        that.id = data.id;
        that.name = data.name;
        that.image = data.image;
        that.university = data.universityName || '';
        that.score = data.score || 0;
        that.url = data.url;// + '?r=' + cd.getParameterFromUrl(0) + '&s=tooltip';
    }

    function getTooltipData(e) {
        if ($('.userTooltip').length > 0) {
            return;
        }
        dataContext.minProfile({
            data: { userId: e.currentTarget.getAttribute('data-uid') },
            success: function (data) {
                var tooltipData = data || {};
                showTooltip(e.currentTarget, tooltipData, e.clientX, e.clientY);
            }
        });
    }


    function showTooltip(target, tooltipData, mouseX, mouseY) {
        var position, html;

        tooltip = new TooltipData(tooltipData);
        //clear old tooltips
        removeTooltip();

        tooltipHTML = document.createElement('div');
        tooltipHTML.className = 'userTooltip';
        html = cd.attachTemplateToData('userToolTipTempalte', tooltip); //We create the div and then append it, so we don't need to fetch the tooltip element
        tooltipHTML.innerHTML = html;
        if (tooltip.id === cd.userDetail().nId) {
            tooltipHTML.querySelector('button').style.display = 'none';
        }
        document.getElementsByTagName('body')[0].appendChild(tooltipHTML);
        position = calculateTooltipPosition(mouseX, mouseY, tooltipHTML.offsetHeight, tooltipHTML.offsetWidth);
        tooltipHTML.style.top = position.y + 'px';
        tooltipHTML.style.left = position.x + 'px';

        if (tooltip.score <= 0) {
            tooltipHTML.classList.add('hidePoints');
        }
        tooltipHTML.classList.add('showTooltip'); //We add this now to enable animation
    }


    function hideTooltip(e) {
        var target;

        target = e.relatedTarget;

        var $target = $(target),
            isTooltip = target.className.indexOf('userTooltip') > -1,
            isTooltipChild = $target.parents('.userTooltip').length > 0,
            closeTimeout;

        if (!(isTooltip || isTooltipChild)) {
            removeTooltip();
            return;
        }


        $('.userTooltip').on('mouseleave', function (e) {
            e.stopPropagation();
            closeTimeout = setTimeout(function () {
                removeTooltip();
            }, 400);
        }).on('mouseenter', function (e) {
            e.stopPropagation();
            clearTimeout(closeTimeout);
        });
    }
    function calculateTooltipPosition(mouseX, mouseY, height, width) {
        var screenWidth = $('html').width(), screenHeight = $('html').height(), positionX, positionY,
            isLtr = $('html').css('direction') === 'ltr',
            right = mouseX + consts.OFFSETX, left = mouseX - width - consts.OFFSETX;

        if (isLtr) {
            if (screenWidth > width) {
                if (width > screenWidth - mouseX) {
                    positionX = calcMiddle();
                } else {
                    positionX = right;
                }
            } else {
                positionX = calcMiddle();
            }
        } else {
            if (screenWidth > width) {
                if (width > mouseX) {
                    positionX = right;
                } else {
                    positionX = calcMiddle();
                }
            } else {
                positionX = calcMiddle();
            }
        }


        if (height > mouseY - $(window).scrollTop()) {
            positionY = mouseY;

        } else {
            positionY = mouseY - height;
        }
        return { x: positionX - $(window).scrollLeft(), y: positionY - $(window).scrollTop() };


        function calcMiddle() {
            if (mouseX < ((screenWidth + $(window).scrollLeft()) / 2)) {
                return right;
            } else {
                return left;

            }
        }
    }

    function removeTooltip() {
        $('.userTooltip').remove();
    }

    $(document).on('click', '.userTooltip .btn3', function (e) {
        if (!tooltip) {
            return;
        }
        e.preventDefault();
        
        
        removeTooltip();                            
        window.location.href = tooltip.url;
        
    })

    cd.pubsub.subscribe('clearTooltip', removeTooltip);
})(cd, cd.data, jQuery);
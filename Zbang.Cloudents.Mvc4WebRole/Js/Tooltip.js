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
        that.url = data.url + '?r=' + cd.getParameterFromUrl(0) + '&s=tooltip';
    }

    function getTooltipData(e) {
        if ($('.userTooltip').length > 0) {
            return;
        }
        dataContext.minProfile({
            data: { userId: e.currentTarget.getAttribute('data-uid') },
            success: function (data) {
                var tooltipData = data || {};
                showTooltip(e.currentTarget, tooltipData, cd.mouse.posX, cd.mouse.posY);
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

        if (Modernizr.touch) {
            target = e.target;
        } else {
            target = cd.mouse.target;
        }

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
    
    $(document).on('click touchend', '.userTooltip .btn3', function (e) {
        e.preventDefault();
        if (!cd.register()) {
            cd.pubsub.publish('register');
            removeTooltip();
            return;
        }
        removeTooltip();
        cd.pubsub.publish('message', { id: '', data: [{ name: tooltip.name, id: tooltip.id, userImage: tooltip.image }] });
    })    
    .on('touchend', '.calloutTrgr', function (e) {
        var event = e.originalEvent;
        e.preventDefault();
        e.stopPropagation();     
        dataContext.minProfile({
            data: { userId: this.getAttribute('data-uid') },
            success: function (data) {
                var tooltipData = data || {};
                showTooltip(tooltipData,
                    event.changedTouches[event.changedTouches.length - 1].clientX,
                    event.changedTouches[event.changedTouches.length - 1].clientY);
            }
        });        
    });

    $(document).on('touchend', '.userTooltip', function (e) {

        e.preventDefault();
        e.stopPropagation();

    })
    .on('touchend', 'body', removeTooltip);

    cd.pubsub.subscribe('clearTooltip', removeTooltip);
})(cd, cd.data, jQuery);
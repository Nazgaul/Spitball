(function (cd, dataContext, $) {
    if (window.scriptLoaded.isLoaded('t')) {
        return;
    }

    var showTooltipTimeout, hideTooltipTimeout, tooltipHTML, isVisible = false, firstTime = true,
        tooltip, DELAY = 500, NO_DELAY = 0, target, mouseX, mouseY,
        consts = { OFFSETX: 5, OFFSETY: 5 };

    function TooltipData(data) {
        var that = this;
        that.id = data.id;
        that.name = data.name;
        that.image = data.image;
        that.university = data.universityName;
        that.score = data.score;
        that.url = data.url;
    }

    function showTooltip(tooltipData, mouseX, mouseY) {
        var position, html;

        isVisible = true;
        tooltip = new TooltipData(tooltipData);
        //clear old tooltips
        $('.userTooltip').remove();

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

        target.classList.add('hover');
    }


    function hideTooltip(delay, toHide) {

        firstTime = true;
        clearTimeout(showTooltipTimeout);
        isVisible = toHide;
        hideTooltipTimeout = setTimeout(function () {
            if (target) {
                target.classList.remove('hover');
            }
            $('.userTooltip').remove();

        }, delay);
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
        function calcMiddle() {
            if (mouseX < ((screenWidth + $(window).scrollLeft()) / 2)) {
                return right;
            } else {
                return left;

            }
        }

        if (height > mouseY - $(window).scrollTop()) {
            positionY = mouseY;

        } else {
            positionY = mouseY - height;
        }
        return { x: positionX - $(window).scrollLeft(), y: positionY - $(window).scrollTop() };
    }


    $(document).on('mousemove', '.calloutTrgr', function (e) {
        mouseX = e.pageX;
        mouseY = e.pageY;
        if (isVisible) {
            return;
        }
        if (!firstTime) {
            return;
        }
        target = this;
        firstTime = false;
        if (target === this) {
            showTooltipTimeout = setTimeout(function () {
                dataContext.minProfile({
                    data: { userId: target.getAttribute('data-uid') },
                    success: function (data) {
                        var tooltipData = data || {};
                        showTooltip(tooltipData, mouseX, mouseY);
                    },
                    error: function () {
                        clearTimeout(showTooltipTimeout);
                        firstTime = true;
                    }
                });
            }, DELAY);
        }
    })
    .on('mouseleave', '.calloutTrgr', function () {
        if (isVisible) {
            hideTooltip(DELAY, false);
        }
        else {
            firstTime = true;
            clearTimeout(showTooltipTimeout);
        }

    })
    .on('mouseenter', '.userTooltip', function () {
        clearTimeout(hideTooltipTimeout);
    })
    .on('mouseleave', '.userTooltip', function (e) {
        var relatedTarget = e.relatedTarget || e.toElement;
        
        if (!relatedTarget) {
            hideTooltip(NO_DELAY, false);
        }

        if ($(relatedTarget).parents('.hover').length > 0 ||
            relatedTarget.classList.contains('hover')) {
            isVisible = true;
            return;
        }
        hideTooltip(DELAY, false);

    })
    .on('click touchend', '.userTooltip .btn3', function (e) {
        e.preventDefault();
        if (!cd.register()) {
            cd.pubsub.publish('register');
            return;
        }
        hideTooltip(NO_DELAY, false);
        cd.pubsub.publish('message', { id: '', data: [{ name: tooltip.name, id: tooltip.id, userImage: tooltip.image }] });
    })
    .on('scroll', function () {
        hideTooltip(NO_DELAY, false);
    })
    .on('touchend', '.calloutTrgr', function (e) {
        var event = e.originalEvent;
        e.preventDefault();
        e.stopPropagation();

        //showTooltip()
        ////if (isVisible) {
        ////    //if (e.target.className.indexOf('userTooltip') === -1) {
        ////    //    hideTooltip(NO_DELAY, false);
        ////    //}
        ////    return;
        ////}
        //if (!firstTime) {
        //    return;
        //}
        target = this;
        //firstTime = false;
        //if (target === this) {
        dataContext.minProfile({
            data: { userId: this.getAttribute('data-uid') },
            success: function (data) {
                var tooltipData = data || {};
                showTooltip(tooltipData,
                    event.changedTouches[event.changedTouches.length - 1].clientX,
                    event.changedTouches[event.changedTouches.length - 1].clientY);
            }
        });
        //}
    });
    //.on('tap', function (e) {
    //    alert(e.target.className);


    //});
    $(document).on('touchend', '.userTooltip', function (e) {
        /// <summary></summary>
        /// <param name="e" type="Event"></param>
        e.preventDefault();
        e.stopPropagation();

    })
    .on('touchend', 'body', function (e) {
        hideTooltip(NO_DELAY, false);
    });
    //document.addEventListener("touchend", function (e) {
    //    if ($(e.target).parents('.userTooltip').length) {
    //        return;
    //    }
    //    //alert($(e.target).parents('.userTooltip').length);
    //    //if (e.target.className.indexOf('userTooltip') === -1 ||
    //    //    $(e.target).parents('.userTooltip').length === -1) {
    //   // 
    //    //}
    //}, false);

    cd.pubsub.subscribe('clearTooltip', function () {
        hideTooltip(NO_DELAY, false);
    });
})(cd, cd.data, jQuery);
(function (cd, pubsub, $) {
    "use strict";

    if (window.scriptLoaded.isLoaded('tpGide')) {
        return;
    }

    var eById = document.getElementById.bind(document),
        $guideContainer, stepIndex = 0, arrowSize = 14;

    pubsub.subscribe('tooltipGuide', function (obj) {
        obj.guideId = obj.guideId.length ? obj.guideId + 'GuideTemplate' : 'genericTooltipGuide';
        $guideContainer = $(eById(obj.guideId));
        initFirstStep();
    
    });

    function initFirstStep() {
        var $firstStep = $guideContainer.find('[data-step="0"]');

        $guideContainer.show();


        setTooltipStep($firstStep);
        registerEvents();
    }


    function setTooltipStep() {
        var $tooltipStep = $guideContainer.find('[data-step="' + stepIndex + '"]'),
            $relativeElement = $($tooltipStep[0].getAttribute('data-tt-position'));

        setPosition();
        checkNextTooltip();
        toggleStep();
        scrollToView();


        function setPosition() {
            
            var arrowPosition = $tooltipStep[0].getAttribute('data-arrow-position');


            if (!$relativeElement.length) {
                return;
            }
            var elementPosition = cd.getElementPosition($relativeElement[0]),
                elementWidth = $relativeElement.outerWidth(),
                elementHeight = $relativeElement.outerHeight(),
                top, left,
                inverse = ($('html').css('direction') === 'rtl');

            switch (arrowPosition) {
                case 'left':
                    top = elementPosition.top - 40 - arrowSize / 2 + elementHeight / 2;
                    if (inverse) {
                        left = elementPosition.left - arrowSize - $tooltipStep.outerWidth(true);
                    } else {
                        left = elementPosition.left + elementWidth + arrowSize;
                    }                    
                    break;
                case 'right':
                    top = elementPosition.top - 40 - arrowSize / 2 + elementHeight / 2;
                    if (inverse) {
                        left = elementPosition.left + elementWidth + arrowSize;
                    } else {
                        left = elementPosition.left - arrowSize - $tooltipStep.outerWidth(true);
                    }          
                    break;
                case 'top':
                    top = elementPosition.top + elementHeight + arrowSize;
                    left = elementPosition.left - $tooltipStep.outerWidth(true) / 2 + elementWidth / 2;
                    break;
                case 'bottom':
                    top = elementPosition.top - $tooltipStep.outerHeight(true) - arrowSize;
                    left = elementPosition.left - $tooltipStep.outerWidth(true) / 2 + elementWidth / 2;
                    break;
            }

            $tooltipStep.css({ top: top, left: left });            
        }

        function checkNextTooltip() {
            var $nextStep = $guideContainer.find('[data-step="' + (stepIndex + 1) + '"]');
            if (!$nextStep.length) {
                return;
            }

            var $nextRelativeElement = $($nextStep[0].getAttribute('data-tt-position'));

            if (!$nextRelativeElement.length) {
                $tooltipStep.addClass('lastStep');
            }

        }

        function toggleStep() {
            $guideContainer.find('.tooltip').hide();
            $tooltipStep.show();
        }

        function scrollToView() {
            var $window = $(window),
                top = cd.getElementPosition($relativeElement[0]).top;
            if (top > $window.height()) {
                $window.scrollTop(top - $relativeElement.outerHeight(true));
            }
            

        }
    }

    function registerEvents() {

        //close
        $guideContainer.on('click', '.closeDialog,[data-done],[data-cancel]', closeGuide);

        if ($guideContainer)
            //next step
            $guideContainer.on('click', '[data-next]', function () {
                if (stepIndex + 1 === $guideContainer.children().length) { //laststep ?
                    return;
                }

                stepIndex++;

                setTooltipStep();

            });

        //prev step
        $guideContainer.on('click', '[data-prev]', function () {
            if (!stepIndex) {  //firststep ?
                return;
            }

            stepIndex--;

            setTooltipStep();
        });


        cd.pubsub.subscribe('clearTooltip', function (type) {
            if (type.scroll) {
                return;
            }
            $guideContainer = $guideContainer || [];
            if (!$guideContainer.length) {
                return;
            }

            $guideContainer.remove();
        });
    }

    function closeGuide() {
        $guideContainer.remove();
    }

})(cd, cd.pubsub, jQuery);
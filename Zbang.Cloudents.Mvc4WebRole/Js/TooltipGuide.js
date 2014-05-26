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
        initFirstStep(obj.buttonTest);
    });

    function initFirstStep(buttonTestFunc) {
        var $firstStep = $guideContainer.find('[data-step="0"]');

        $guideContainer.show();

        if (!buttonTestFunc()) {
            $firstStep.removeClass('btns');
        }
        setTooltipStep($firstStep);
        registerEvents();
    }


    function setTooltipStep() {
        var $tooltipStep = $guideContainer.find('[data-step="' + stepIndex + '"]');

        setPosition();
        if (stepIndex > 0) {
            toggleStep();
        }


        function setPosition() {
            var arrowPosition = $tooltipStep[0].getAttribute('data-arrow-position');

            setArrowPosition();
            setStepPosition();
            
            function setArrowPosition() {
                $tooltipStep.addClass(arrowPosition);
            }
            function setStepPosition() {
                var $relativeElement = $($tooltipStep[0].getAttribute('data-tt-position'));

                if (!$relativeElement.length) {
                    return;
                }
                var elementPosition = cd.getElementPosition($relativeElement[0]),
                    elementWidth = $relativeElement.outerWidth(),
                    elementHeight = $relativeElement.outerHeight(),
                    top, left;

                switch (arrowPosition) {
                    case 'left':
                        top = elementPosition.top - 40 - arrowSize / 2 + elementHeight / 2;
                        left = elementPosition.left + elementWidth + arrowSize;
                        break;
                    case 'right':
                        top = elementPosition.top - 40 - arrowSize / 2 + elementHeight / 2;
                        left = elementPosition.left - elementWidth + arrowSize;
                        break;
                    case 'top':
                        top = elementPosition.top + elementHeight + arrowSize;
                        left = elementPosition.left - $tooltipStep.outerWidth(true) / 2 + elementWidth / 2;
                        break;
                    case 'bottom':
                        top = elementPosition.top - arrowSize;
                        left = elementPosition.left - $tooltipStep.outerWidth(true) / 2 + elementWidth / 2;
                        break;
                }
                $tooltipStep.css({ top: top, left: left });
            }
        }

        function toggleStep() {
            $guideContainer.find('.tooltip').hide();
            $tooltipStep.show();
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
    }

    function closeGuide() {
        $guideContainer.remove();
    }
})(cd, cd.pubsub, jQuery);
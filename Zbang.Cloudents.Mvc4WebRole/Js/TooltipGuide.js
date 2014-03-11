(function (cd, pubsub, $) {
    "use strict";

    if (window.scriptLoaded.isLoaded('tpGide')) {
        return;
    }

    var eById = document.getElementById.bind(document),
        $guideContainer, stepIndex = 0;

    pubsub.subscribe('tooltipGuide', function (guideId) {
        if (!(guideId)) {
            return;
        }

        guideId = guideId || 'genericTooltipGuide';
        $guideContainer = $(eById(guideId));
        initFirstStep();
    });

    function initFirstStep() {
        var $firstStep = $guideContainer.find('[data-step="0"]');

        $guideContainer.show();

        setTooltipStep($firstStep);
        registerEvents();
    }


    function setTooltipStep() {
        var $tooltipStep = $guideContainer.find('[data-step="' + stepIndex + '"]');

        setPosition();
        toggleStep();

        function setPosition() {
            $guideContainer.css({
                top: '50%',
                left:'50%'
            });
        }

        function toggleStep() {
            $('.stepTip').hide();
            $tooltipStep.show();
        }
    }
    
    function registerEvents() {

        //next step
        $guideContainer.on('click', '.nextStep', function () {
            if (stepIndex + 1 === $guideContainer.children().length) { //laststep ?
                return;
            }

            stepIndex++;

            setTooltipStep();

        });

        //prev step
        $guideContainer.on('click', '.prevStep', function () {
            if (!stepIndex) {  //firststep ?
                return;
            }

            stepIndex--;

            setTooltipStep();
        });

        //done step
        $guideContainer.one('click', '.doneStep',  closeGuide);               

        //close
        $guideContainer.on('click', '.closeTip', closeGuide);       
    }

    function closeGuide() {
        $guideContainer.remove();
    }
})(cd, cd.pubsub,jQuery);
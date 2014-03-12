(function ($, cd) {
    if (window.scriptLoaded.isLoaded('w')) {
        return;
    }

    var $welcomeDialog = $('#welcomeDialog'),
        popup = $welcomeDialog.find('.popupWrppr'),
        $btnPrev = $welcomeDialog.find('.btnPrev'),
        $btnShow = $welcomeDialog.find('.btnNext'),
        position = 1,
        $main = $('#main');
    $main.css('visibility', 'hidden');
    $btnPrev.click(function () {
        position--;
        if (position === 1) {
            $btnPrev.hide();
        }
        $btnShow.show();
        changeScreen();
    });
    $btnShow.click(function () {
        position++;
        if (position === 4) {
            $btnShow.hide();
        }
        $btnPrev.show();
        changeScreen();
    });
    function changeScreen() {
        $welcomeDialog.find('[data-step]').hide();
        $('#rad' + position).prop('checked', true);
        $welcomeDialog.find('.step' + position).show();
    }
    $welcomeDialog.find('.closeDialog,#wellClose').click(function () {
        $welcomeDialog.remove();
        $main.css('visibility', 'visible');
        cd.pubsub.publish('welcome');
    });

    $welcomeDialog.on('click', 'input[type="radio"]', function (e) {
        position = e.target.id.replace(/[A-Za-z$-]/g, "");
        changeScreen();
    });

})(jQuery, cd);
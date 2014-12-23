//#region feedback
var feedback = function ($, analytics, cd) {

    if (window.scriptLoaded.isLoaded('esi')) {
        return;
    }

    $('.contactLink').click(function () {
        if (window.UserVoice === undefined) {
            var uv = document.createElement('script');
            uv.type = 'text/javascript';
            uv.async = true;
            uv.src = '//widget.uservoice.com/G2In5CQZfezX7sC95Dx2w.js';
            var s = document.getElementsByTagName('script')[0];
            s.parentNode.insertBefore(uv, s);
        }
        var UserVoice = window.UserVoice || [];
        var isLtr = $('html').css('direction') === 'ltr' ? 'right' : 'left';
        UserVoice.push(['set', {
            accent_color: '#0f9d58',
            position: 'top-' + isLtr
        }]);
        UserVoice.push(['identify', {
            name:       cd.userDetail().name, // User’s real name
            id:         cd.userDetail().id, // Optional: Unique id of the user (if set, this should not change)
           
        }]);
        UserVoice.push(['show', { mode: 'contact' }]);
       
    });
}(jQuery, cd.analytics , cd);
//#endregion





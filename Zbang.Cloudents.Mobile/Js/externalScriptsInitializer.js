//#region feedback
var feedback = function ($, analytics, cd) {

    if (window.scriptLoaded.isLoaded('esi')) {
        return;
    }

    $('.contactLink').click(function () {
        analytics.trackEvent('Dashboard', 'Feedback', 'Number of Clicks on User Feedback');
        if (window.UserVoice === undefined) {
            var uv = document.createElement('script');
            uv.type = 'text/javascript';
            uv.async = true;
            uv.src = '//widget.uservoice.com/G2In5CQZfezX7sC95Dx2w.js';
            var s = document.getElementsByTagName('script')[0];
            s.parentNode.insertBefore(uv, s);
        }
        UserVoice = window.UserVoice || [];

        //UserVoice.push(['showLightbox', 'classic_widget', {
        //    mode: 'full',
        //    primary_color: '#952262',
        //    link_color: '#007dbf',
        //    default_mode: 'feedback',
        //    forum_id: 180976
        //}]);
        var isLtr = $('html').css('direction') === 'ltr' ? 'right' : 'left';
        
        UserVoice.push(['set', {
            accent_color: '#0f9d58',
            position: 'top-' + isLtr
            //trigger_color: 'white',
            //trigger_background_color: 'rgba(46, 49, 51, 0.6)'
        }]);

        UserVoice.push(['identify', {
            //email:      'john.doe@example.com', // User’s email address
            name:       cd.userDetail().name, // User’s real name
            //created_at: 1364406966, // Unix timestamp for the date the user signed up
            id:         cd.userDetail().id, // Optional: Unique id of the user (if set, this should not change)
            //type:       'Owner', // Optional: segment your users by type
            //account: {
            //  id:           123, // Optional: associate multiple users with a single account
            //  name:         'Acme, Co.', // Account name
            //  created_at:   1364406966, // Unix timestamp for the date the account was created
            //  monthly_rate: 9.99, // Decimal; monthly rate of the account
            //  ltv:          1495.00, // Decimal; lifetime value of the account
            //  plan:         'Enhanced' // Plan name for the account
            //}
        }]);
        UserVoice.push(['show', { mode: 'contact' }]);
        // Add default trigger to the bottom-right corner of the window:
        //UserVoice.push(['addTrigger', { mode: 'contact', trigger_position: 'bottom-right' }]);

        // Or, use your own custom trigger:
        //UserVoice.push(['addTrigger', '#id', { mode: 'contact' }]);

        // Autoprompt for Satisfaction and SmartVote (only displayed under certain conditions)
        //UserVoice.push(['autoprompt', {}]);
    });
}(jQuery, cd.analytics , cd);
//#endregion





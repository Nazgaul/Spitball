const analyticsService = {

    sb_unitedEvent: (category, action, label = '') => {
        if (category && action) {
            global.ga( 'send', 'event', `${category}`, `${action}`, `${label}`);
            if(!!global.dataLayer){
                global.dataLayer.push({
                    'event': 'Spitball',
                    'category': `${category}`,
                    'action': `${action}`,
                    'label': `${label}`
                });
            }
        } else {
            console.error('united event does not have data required');
        }
    },

    sb_fireTimingAnalytic: (timingCategory, timingVar, timingValue, timingLabel) => {
        // ga('send', 'timing', 'JS Dependencies', 'load', 3549);
        global.ga('send', 'timing', timingCategory, timingVar, timingValue, timingLabel);
    },

    sb_setUserId: (userId) => {
        global.ga('set', 'userId', userId);
    }


};
export default analyticsService;
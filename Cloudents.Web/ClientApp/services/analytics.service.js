const analyticsService = {
    
    sb_unitedEvent: (category, action, label = '') => {
        if(!!global.ga){
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
        }
    },

    sb_fireTimingAnalytic: (timingCategory, timingVar, timingValue, timingLabel) => {
        if(!!global.ga){
            global.ga('send', 'timing', timingCategory, timingVar, timingValue, timingLabel);
        }
    },

    sb_setUserId: (userId) => {
        if(!!global.ga){
            global.ga('set', 'userId', userId);
        }
    }


};
export default analyticsService;
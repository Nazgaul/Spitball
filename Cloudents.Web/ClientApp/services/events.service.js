const eventService = {

    sb_unitedEvent: (category, action, label = '') => {
        if (category && action) {
            window.ga( 'send', 'event', `${category}`, `${action}`, `${label}`);
            window.dataLayer.push({
                'category': `${category}`,
                'action': `${action}`,
                'label': `${label}`
            });
        } else {
            console.error('united event does not have data required')
        }
    },

    sb_fireTimingAnalytic: (timingCategory, timingVar, timingValue, timingLabel) => {
        // ga('send', 'timing', 'JS Dependencies', 'load', 3549);
        window.ga('send', 'timing', timingCategory, timingVar, timingValue, timingLabel);
    }


};
export default eventService;
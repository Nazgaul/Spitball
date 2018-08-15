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
    }
};
export default eventService;
export default {
    methods: {
        trackOutBound(callback) {
            console.log(this.url);
            this.$ga.query('send', 'event', {
                eventCategory: 'Outbound Link',
                eventAction: 'click',
                eventLabel: this.url
            });
            callback();
        }
    }
}
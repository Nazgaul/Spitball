import mainHeader from '../helpers/header.vue'
import verticalsTabs from './verticalsTabs.vue'
export default {
    components: {
        mainHeader,verticalsTabs
    },
    beforeRouteUpdate(to, from, next) {
        const toName = to.path.slice(1);
        let tabs = this.$el.querySelector('.v-tabs__wrapper');
        let currentItem = this.$el.querySelector(`#${toName}`);
        if (currentItem)
            tabs.scrollLeft = currentItem.offsetLeft - (tabs.clientWidth / 2);

        next();
    },
    mounted() {
        let tabs = this.$el.querySelector('.v-tabs__wrapper');
        let currentItem = this.$el.querySelector(`#${this.currentSelection}`);
        if (currentItem)
            tabs.scrollLeft = currentItem.offsetLeft - (tabs.clientWidth / 2);
    },
    props: {
        currentSelection: {type: String},
        submitRoute: {String},
        userText: {String}
    }
}

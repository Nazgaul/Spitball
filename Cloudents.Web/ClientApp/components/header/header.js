import mainHeader from '../helpers/header.vue'
import verticalsTabs from './verticalsTabs.vue'
export default {
    components: {
        mainHeader,verticalsTabs
    },
    beforeRouteUpdate(to, from, next) {
        const toName = to.path.slice(1);
        let tabs = this.$el.querySelector('.tabs__wrapper');
        let currentItem = this.$el.querySelector(`#${toName}`);
        if (currentItem)
            tabs.scrollLeft = currentItem.offsetLeft - (tabs.clientWidth / 2);

        next();
    },
    mounted() {
        let tabs = this.$el.querySelector('.tabs__wrapper');
        let currentItem = this.$el.querySelector(`#${this.currentSelection}`);
        if (currentItem)
            tabs.scrollLeft = currentItem.offsetLeft - (tabs.clientWidth / 2);
    },
    props: {
        currentSelection: {type: String},
        submitRoute: {String},
        userText: {String}
    }
    // },
    // methods: {
    //     ...mapActions(["setCurrentVertical","getAIDataForVertical"]),
    //     submit: function () {
    //         this.$nextTick(() => {
    //             this.$el.querySelector('input').blur();
    //             this.$el.querySelector('form').blur();
    //         });
    //     },
    //     $_updateType(result) {
    //         this.$ga.event("Vertical_Tab",result);
    //         let tabs = this.$el.querySelector('.tabs__wrapper');
    //         let currentItem = this.$el.querySelector(`#${result}`);
    //         if (currentItem) {
    //             tabs.scrollLeft = currentItem.offsetLeft - (tabs.clientWidth / 2);
    //         }
    //         this.setCurrentVertical(result);
    //         this.getAIDataForVertical(result).then(({text="",course})=>{
    //             this.$router.push({ path: '/' + result, query: { q: text,course } });
    //         })
    //     }
    // }
}

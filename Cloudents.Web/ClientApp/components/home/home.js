import featuresSection from './features.vue';
import stripsSection from './horizontalStrip.vue';
import PageLayout from './layout.vue';
import sbSearch from "./search.vue";

import { features, bottomIcons, strips, sites, testimonials } from './consts'

var components = {
    sbSearch,
    PageLayout,
    menuIcon: () => import("./svg/menu-icon.svg"),
    recordingIcon: () => import("./svg/recording.svg"),
    featuresSection,
    stripsSection
};

for (var t in bottomIcons) {
    const item = bottomIcons[t];
    components[item.img+"-icon"] = item.svg;
}

export default {
    components: components,
    computed: {
        placeholder: function () {
            if (this.$vuetify.breakpoint.smAndUp) {
                return "Find study documents, textbooks, deals, tutors and more…";
            }
            return "Study documents, textbooks, tutors …";
        }

    },
    data() {
        return {
            // items: homeSuggest,
            bottomIcons: bottomIcons,
            drawer: null,
            strips: strips,
            features: features,

            sites: sites,
            testimonials: testimonials,
            scrollTop: false

        };
    },
    //methods: {
    //    search() {
    //        this.$router.push({ name:"result", query: {q:this.msg} });
    //    },
    //    selectos(item) {
    //        this.msg = item;
    //        this.search();
    //    },
    //    //onScroll(e) {
    //    //    this.scrollTop = window.pageYOffset || document.documentElement.scrollTop > 100;
    //    //}
    //    //$_imageUrl(image) { return require.context(`~/img/${image}.png`);}
    //},

    props: {
        metaText: { type: String }
    }

};

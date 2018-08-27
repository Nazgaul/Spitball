import { USER } from '../../store/mutation-types'
import {mapMutations} from 'vuex'

let sortOptions = ['buy', 'sell'];
export default {
    props: {id: {Number}},
    data: () => {
        return {sortOptions, isLoad: false}
    },
    methods: {
        ...mapMutations(["UPDATE_LOADING", "UPDATE_CURRENT_SORT"]),
        updateSort(mobile){
            let options = mobile? null : sortOptions;
            this.UPDATE_CURRENT_SORT(options);
        },
        $_changeTab(val) {
            this.isLoad = true;
            let _this = this;
            this.$store.dispatch("bookDetails", {
                pageName: "bookDetails",
                isbn13: _this.id,
                type: val
            }).then((data) => {
                    _this.$root.$children[0].$refs.mainPage.pageData = data;
                    _this.$root.$children[0].$refs.mainPage.sortVal = val;
                    this.isLoad = false;
                });
        }
    },
    created() {
        updateSort(this.$vuetify.breakpoint.xsOnly);
    }

}
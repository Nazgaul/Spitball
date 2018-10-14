import { USER } from '../../store/mutation-types'
import {mapMutations} from 'vuex'
import { LanguageService } from "../../services/language/languageService";

let sortOptions = [
    {key: "buy", value: LanguageService.getValueByKey("book_sort_buy")},
    {key: "sell", value: LanguageService.getValueByKey("book_sort_sell")}
    ];
export default {
    props: {id: {Number}},
    data: () => {
        return {sortOptions, isLoad: false}
    },
    methods: {
        ...mapMutations(["UPDATE_LOADING", "UPDATE_CURRENT_SORT"]),
        setStoreSort(mobile){
            let options = mobile ? null : sortOptions;
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
        this.setStoreSort(this.$vuetify.breakpoint.xsOnly);
    }

}
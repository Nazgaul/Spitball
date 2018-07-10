let sortOptions = [{ id: 'buy', name: 'BUY/RENT' }, { id: 'sell', name: 'SELL' }];
import { mapMutations } from 'vuex'
export default {
    data: () => { return { sortOptions,isLoad:false } },
    methods: {
        ...mapMutations(["UPDATE_LOADING"]),
        $_changeTab(val) {
            this.isLoad=true;
            let _this = this;
            this.$store.dispatch("bookDetails", {
                pageName: "bookDetails",
                isbn13: _this.id,
                type: val
            }).then(({ data }) => {
                _this.$root.$children[0].$refs.mainPage.pageData = data;
                _this.$root.$children[0].$refs.mainPage.sortVal = val;
                this.isLoad=false;
            });
        }
    },
    created() {
    },
    props: { id: { Number } }
}
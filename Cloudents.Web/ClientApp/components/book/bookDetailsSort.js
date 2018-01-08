let sortOptions = [{ id: 'buy', name: 'BUY / RENT' }, { id: 'sell', name: 'SELL' }];
import { mapMutations } from 'vuex'
export default {
    data: () => { return { sortOptions } },
    methods: {
        ...mapMutations(["UPDATE_LOADING"]),
        $_changeTab(val) {
            this.UPDATE_LOADING(true);
            let _this = this;
            this.$store.dispatch("bookDetails", {
                pageName: "bookDetails",
                isbn13: _this.id,
                type: val
            }).then(({ data }) => {
                _this.$root.$children[0].$refs.mainPage.pageData = data;
                this.UPDATE_LOADING(false);
            });
        }
    },
    created() {
        console.log(this.id);
    },
    props: { id: { Number } }
}
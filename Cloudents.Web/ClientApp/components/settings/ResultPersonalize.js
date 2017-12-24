import searchItem from './searchItem.vue'
import { mapGetters, mapActions } from 'vuex'
export default {
    components: { searchItem },
    data() {
        return { showDialog: false, isSearch: false, type: "", keep: true, isfirst: false }
    },

    computed: {
        ...mapGetters(['isFirst'])
    },

    watch: {
        showDialog(val) {
            !val && this.isfirst ? this.isfirst = false : "";
        }
    },
    created() {
        if (this.isFirst) {
            this.isfirst = true;
            this.updateFirstTime("isFirst");
            setTimeout(()=>this.showDialog = true,5000);
        }
    },
    methods: {
        ...mapActions(["updateFirstTime"]),
        $_personalize() {
            this.type = "university";
            this.isSearch = true;
        }
    },
}
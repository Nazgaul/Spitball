import searchItem from './searchItem.vue'
import { settingMenu } from '../settings/consts';
import { mapGetters, mapActions } from 'vuex'
export default {
    components: { searchItem },
    data() {
        return {
            showDialog: false,
            isSearch: false,
            type: "",
            keep: true,
            isfirst: false
        }
    },

    computed: {
        ...mapGetters(['isFirst', 'getUniversityName'])
    },

    watch: {
        showDialog(val) {
            !val && this.isfirst ? this.isfirst = false : "";
            if(!val){
                this.$root.$emit("closePersonalize");
            }
        }
    },
    created() {
        if (this.isFirst) {
            this.isfirst = true;
            this.updateFirstTime("isFirst");
            setTimeout(() => this.showDialog = true, 10);
        }
        this.$root.$on("personalize",
            (type) => {

                settingMenu.find(i => i.id === type).click.call(this, this.getUniversityName);
                this.showDialog = true;
            });
    },
    methods: {
        ...mapActions(["updateFirstTime"]),
        $_personalize() {
            this.type = "university";
            this.isSearch = true;
        }
    },
}
import searchItem from '../settings/searchItem.vue'
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
            isfirst: false,
            currentUni: ""
        }
    },
    props:{'value':Boolean},
    computed: {
        ...mapGetters(['isFirst', 'getUniversityName']),
    },

    watch: {
        showDialog(val) {
            this.isfirst = this.isFirst;
            !val && this.isfirst ? this.isfirst = false : "";
            if (!val) {
                this.type = "";
                if (this.isFirst) { this.updateFirstTime("isFirst"); }
                this.$root.$emit("closePersonalize");
                if (this.currentUni !== this.getUniversityName && this.$route.name === "result") {
                    window.location.reload();
                }
            } else {
                this.currentUni = this.getUniversityName;
            }

        }
    },
    methods: {
        ...mapActions(["updateFirstTime"]),
        $_personalize() {
            this.type = "university";
            this.isSearch = true;
        },
        openDialog(type){
            settingMenu.find(i => i.id === type).click.call(this, this.getUniversityName);
            this.showDialog = true;
        }
    },
    beforeDestroy(){
        clearTimeout(this.$options.timeout)
    }
}
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
            isfirst: false,
            isAcademinc: false
        }
    },

    computed: {
        ...mapGetters(['isFirst', 'getUniversityName']),
    },

    watch: {
        showDialog(val) {
            !val && this.isfirst ? this.isfirst = false : "";
            if (!val) {
                if (this.isFirst) { this.updateFirstTime("isFirst"); }
                this.$root.$emit("closePersonalize");
            }
        },
        isAcademinc(val) {
            if (val && this.isFirst) {
                setTimeout(() => {
                    if (!this.$root.$el.querySelector(".dialog__content__active"))
                        this.showDialog = true
                }, 5000);
            }
        }
    },
    beforeRouteUpdate(to, from, next) {
        this.$_isAcademic(to);
        next()
    },
    created() {
        this.$_isAcademic(this.$route);
        //if ( this.isAcademic) {
        console.log(this.isAcademic);
        //setTimeout(() => {
        //    if(!this.$root.$el.querySelector(".dialog__content__active"))
        //            this.showDialog = true
        //}, 5000);
        //}
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
        },

        $_isAcademic(to) {
            let newName = to.path.slice(1);
            this.isAcademinc = (newName != "job" && newName != "food")
        }
    }
}
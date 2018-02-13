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
            isAcademinc: false,
            currentUni: ""
        }
    },

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

        },
        isAcademinc(val) {
            debugger;
            if (val && this.isFirst) {
                setTimeout(() => {
                    if (this.isAcademinc && !this.$root.$el.querySelector(".dialog__content__active"))
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
        //setTimeout(() => {
        //    if(!this.$root.$el.querySelector(".dialog__content__active"))
        //            this.showDialog = true
        //}, 5000);
        //}
        this.$root.$on("personalize",
            (type) => {
                this.openDialog(type)
            });
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
        },
        $_isAcademic(to) {
            let newName = to.path.slice(1);
            this.isAcademinc = (newName !== "job" && newName !== "food" && !to.path.includes('result'))
        }
    }
}
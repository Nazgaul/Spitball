import logo from "../../../wwwroot/Images/logo-spitball.svg";
//import navBar from "../navbar/TheNavbar.vue"
import { mapActions, mapGetters } from 'vuex';
import { settingMenu } from '../settings/consts';
import { micMixin } from '../helpers/mic';


export default {
    mixins: [micMixin],
    components: {
        logo
    },
    data() {
        return {
            settingMenu,
            qFilter: this.userText,
        };
    },
    computed: {
        ...mapGetters(['luisTerm', 'getUniversityName', 'isFirst'])
        //isMobileSize: function () {
        //    return this.$vuetify.breakpoint.xsOnly;
        //}
    },
    watch: {
        userText(val) {
            this.qFilter = val;
        },
        //update text according mic update
        msg(val) {
            this.qFilter = val;
        }
    },
    beforeRouteUpdate(to, from, next) {
        const toName = to.path.slice(1);
        let tabs = this.$el.querySelector('.tabs__wrapper');
        let currentItem = this.$el.querySelector(`#${toName}`);
        if (currentItem)
            tabs.scrollLeft = currentItem.offsetLeft - (tabs.clientWidth / 2);

        next();
    },
    mounted() {
        //if(this.isMobileSize){
        let tabs = this.$el.querySelector('.tabs__wrapper');
        let currentItem = this.$el.querySelector(`#${this.currentSelection}`);
        if (currentItem)
            tabs.scrollLeft = currentItem.offsetLeft - (tabs.clientWidth / 2);
        //}
    },
    props: {
        $_calcTerm: { type: Function },
        verticals: { type: Array },
        callbackFunc: { type: Function },
        currentSelection: { type: String },
        userText: { type: String },
        currentPath: { type: String },
        luisType: { type: String },
        getLuisBox: { type: Function },
        name: { type: String },
        myClasses: {}
    },
    methods: {
        ...mapActions(["updateSearchText"]),
        submit: function () {
            this.updateSearchText(this.qFilter).then(({ term: luisTerm, docType }) => {
                let result = this.currentPath;
                this.$route.meta[this.luisType] = {
                    term: this.qFilter,
                    luisTerm, docType
                };
                this.$nextTick(() => {
                    this.$router.push({ path: result, query: { q: this.qFilter }, meta: { ...this.$route.meta } });
                });
            });
        },
        //callback for mobile submit mic
        submitMic() {
            this.submit();
        },
        menuToggle: function () {
            this.$emit("input", !this.value);
        },
        $_currentClick(item) {
            this.$root.$emit("personalize", item.id);
        },
        $_updateType(result) {
            //if(this.isMobileSize){
            let tabs = this.$el.querySelector('.tabs__wrapper');
            let currentItem = this.$el.querySelector(`#${result}`);
            if (currentItem)
                tabs.scrollLeft = currentItem.offsetLeft - (tabs.clientWidth / 2);
            //}
            if (this.name !== "result") {
                if (this.callbackFunc) {
                    this.callbackFunc.call(this, result);
                } else {
                    this.$router.push({ path: '/' + result, query: { q: this.userText } });
                }
            }
            else if (this.$route.meta[this.$_calcTerm(result)]) {
                let query = { q: this.getLuisBox(result).term };
                if (this.currentPath.includes(result)) query = { ...this.$route.query, ...query };
                if (this.myClasses && (result.includes('note') || result.includes('flashcard'))) query.course = this.myClasses;
                this.$router.push({ path: '/' + result, query })
            } else {
                if (!this.getUniversityName && (result !== 'food' && result !== 'job')) {
                    this.$root.$children[0].$refs.personalize.showDialog = true;
                    return;
                }
                this.$router.push({ path: '/' + result, query: { q: "" } });
            }
        }
    }
}

import logo from "../../../wwwroot/Images/logo-spitball.svg";
import navBar from "../navbar/TheNavbar.vue"
import { mapActions, mapGetters } from 'vuex';
import { settingMenu } from '../settings/consts';
import { micMixin } from '../helpers/mic';


export default {
    mixins:[micMixin],
    components: {
        logo, navBar
    },
    data() {
        return {
            settingMenu,
            currentName:"",
            qFilter: this.$route.query.q,
            keep:false,
            type: ""
            
        };
    },
    computed: {
        ...mapGetters(['luisTerm','getUniversityName','isFirst']),
        name: function () {
            
            let currentPage = this.$route.meta.pageName ? this.$route.meta.pageName : this.$route.path.split("/")[1];
            if (this.currentName !== currentPage) {
                this.currentName = currentPage;
                    if (this.$route.query.q) {
                        this.qFilter = this.$route.query.q;
                        this.$emit("update:userText", this.qFilter);
                    }
                }
                return this.currentName;
        },
        isMobileSize: function () {
            return this.$vuetify.breakpoint.xsOnly;
        }
    },
    watch:{
      '$route':function(val){
          this.qFilter=val.query.q;
      },
      msg(val){
          this.qFilter=val;
      }
    },
    props: { $_calcTerm: { type: Function }, verticals: { type: Array }, callbackFunc: { type: Function }, currentSelection: { type: String } },

    //props:{showMoreOptions:{type:Boolean,default:true},showSingleLine:{type:Boolean,default:false}},
    methods: {
        ...mapActions(["updateSearchText","createCourse","updateFirstTime"]),
        submit: function () {
            this.updateSearchText(this.qFilter).then(({term:luisTerm,docType}) => {
                let result=this.$route.name==="bookDetails"?"/book":this.$route.path;
                this.$route.meta[result.includes('food')?'foodTerm':result.includes('job')?'jobTerm':'term']={
                    term: this.qFilter,
                    luisTerm,docType
                };
                this.$nextTick(()=>{
                this.$router.push({path:result, query: { q: this.qFilter },meta:{...this.$route.meta} });
                });
            });
            this.$emit("update:overlay", false);
        },
        menuToggle: function() {
            this.$emit("input",!this.value);
        },
        $_currentClick(item){
            let itemToUpdate=this.$parent.$children.find(i=>i.$refs.person);
            item.click.call(itemToUpdate,this.getUniversityName);
        },
        $_currentTerm(type) {
            let term = type.includes('food') ? this.$route.meta.foodTerm : type.includes('job') ? this.$route.meta.jobTerm : this.$route.meta.term;
            return term || {};
        },
        $_updateType(result) {
            if (this.$route.name !== "result") {
                if (this.callbackFunc) {
                    this.callbackFunc.call(this, result);
                } else {
                    this.$router.push({ path: '/' + result, query: { q: this.$route.query.q } });
                }
            }
            else if (this.$route.meta[this.$_calcTerm(result)]) {
                let query = { q: this.$_currentTerm(result).term };
                if (this.currentPage === result) query = { ...this.$route.query, ...query };
                if (this.$route.meta.myClasses && (result.includes('note') || result.includes('flashcard'))) query.course = this.$route.meta.myClasses;
                this.$router.push({ path: '/' + result, query })
            } else {

                if (!this.getUniversityName && (result !== 'food' && result !== 'job')) {
                    this.$root.$children[0].$refs.personalize.showDialog = true;
                    return;
                }
                this.$router.push({ path: '/' + result });
            }
        },
    }
}

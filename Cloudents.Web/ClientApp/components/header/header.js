import logo from "../../../wwwroot/Images/logo-spitball.svg";
import { mapActions, mapGetters } from 'vuex';
import { settingMenu } from '../settings/consts';
import {micMixin} from '../helpers/mic'

export default {
    mixins:[micMixin],
    components: {
        logo
    },
    data() {
        return {
            settingMenu,
            currentName:"",
            qFilter: this.$route.query.q,
            keep:false,
            type:"",
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
    props:{value:{type:Boolean},showMoreOptions:{type:Boolean},showSingleLine:{type:Boolean,default:false}},
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
        }
    }
}

import {  names } from "../../data"
import logo from "../../../wwwroot/Images/logo-spitball.svg";
const ResultPersonalize = () => import('./ResultPersonalize.vue');
import { mapActions, mapGetters } from 'vuex';
import { settingMenu } from '../settings/consts';
import searchItem from '../settings/searchItem.vue';
//import 'vue-awesome/icons/search';
//import VIcon from 'vue-awesome/components/Icon.vue'

export default {
    components: {
        logo,searchItem,ResultPersonalize
    },
    data() {
        return {
            settingMenu,
            showDialog:false,
           // placeholders: placeholders,
            names: names,
            currentName:"",
            qFilter: this.$route.query.q,
            keep:false,
            type:"",
            isfirst:false
        };
    },
    created(){
        this.isfirst = this.isFirst;
        this.courseFirst=this.courseFirstTime;
        this.$nextTick(() => {
            if (this.isFirst) this.updateFirstTime("isFirst");
            if(this.courseFirst)this.updateFirstTime("courseFirstTime");
        });
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
        },
        voiceAppend(){
           return ("webkitSpeechRecognition" in window)?'sbf-mic':'';
        }
    },
    watch:{
      '$route':function(val){
          this.qFilter=val.query.q;
      },
        showDialog(val){
          if(!val){
              this.isfirst=false;
          }
        }
    },
    props:{value:{type:Boolean}},
    methods: {
        ...mapActions(["updateSearchText","createCourse","updateFirstTime"]),
        submit: function () {
            this.updateSearchText(this.qFilter).then((response) => {
                let result=this.$route.path;
                this.$route.meta[result.includes('food')?'foodTerm':result.includes('job')?'jobTerm':'term']={
                    term: this.qFilter,
                    luisTerm: response.term
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
        mic: function () {
            //TODO: YIFAT need to add mic handle here.
            console.log("hi")
        },
        $_currentClick(item){
            item.click.call(this,this.getUniversityName);
        }
    }
}

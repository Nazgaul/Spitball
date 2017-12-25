<template>
    <v-tabs class="verticals-bar" :value="currentSelection">
         <v-tabs-bar>
            <v-tabs-item v-for="tab in verticals" 
            :key="tab.id" :href="tab.id" :id="tab.id"
            @click="$_updateType(tab.id)"
            :class="['spitball-text-'+tab.id,tab.id==currentSelection?'tabs__item--active':'']"
                                    class="mr-4 vertical">
                {{tab.name}}
            </v-tabs-item>
            <v-tabs-slider :color="`color-${currentSelection}`"></v-tabs-slider>
        </v-tabs-bar>
    </v-tabs>
    <!-- <v-tabs class="verticals-bar" :value="currentSelection" :scrollable="false">
        <v-tabs-bar>
            <v-tabs-item v-for="tab in verticals" :key="tab.id" :href="tab.id" :id="tab.id"
             @click="$_updateType(tab.id)"
              :class="['spitball-text-'+tab.id,tab.id==currentSelection?'tabs__item--active':'']"
                         class="mr-4 vertical">
                {{tab.name}}
            </v-tabs-item>
            <v-tabs-slider :color="`color-${currentSelection}`"></v-tabs-slider>
        </v-tabs-bar>
    </v-tabs> -->
</template>
<script>
import { mapMutations, mapGetters } from "vuex";
export default {
  data() {
    //console.log(this.$route.props.verticals);
    return {
      newVertical: ""
      // offsetTop: 0
      //verticals : this.$route.props.verticals
    };
  },
  mounted() {
    //if(this.isMobileSize){
    let tabs = this.$el.querySelector(".tabs__wrapper");
    let currentItem = this.$el.querySelector(`#${this.currentSelection}`);
    if (currentItem)
      tabs.scrollLeft = currentItem.offsetLeft - tabs.clientWidth / 2;
    //}
  },
  props: {
    //$_calcTerm: { type: Function },
    verticals: { type: Array },
    //callbackFunc: { type: Function },
    currentSelection: { type: String },
    name:{type:String}
  },

  methods: {
      $_updateType(result) {
          console.log(result);
            //if(this.isMobileSize){
                let tabs=this.$el.querySelector('.tabs__wrapper');
                let currentItem=this.$el.querySelector(`#${result}`);
                if(currentItem)
                     tabs.scrollLeft=currentItem.offsetLeft-(tabs.clientWidth/2);
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
                this.$router.push({ path: '/' + result,query:{q:""} });
            }
        }
    // $_currentTerm(type) {
    //     let term = type.includes('food') ? this.$route.meta.foodTerm : type.includes('job') ? this.$route.meta.jobTerm : this.$route.meta.term;
    //     return term || {};
    // },
    // $_updateType(result) {
    //     if (this.$route.name !== "result") {
    //         if(this.callbackFunc){
    //             this.callbackFunc.call(this,result);
    //         }else {
    //             this.$router.push({path: '/' + result, query: {q: this.$route.query.q}});
    //         }
    //     }
    //     else if (this.$route.meta[this.$_calcTerm(result)]) {
    //         let query = { q: this.$_currentTerm(result).term };
    //         if (this.currentPage === result) query = { ...this.$route.query, ...query };
    //         if(this.$route.meta.myClasses&&(result.includes('note')||result.includes('flashcard')))query.course=this.$route.meta.myClasses;
    //         this.$router.push({ path: '/' + result, query })
    //     } else {
    //         if (!this.getUniversityName && (result !== 'food' && result !== 'job')) {
    //             this.$root.$children[0].$refs.personalize.showDialog=true;
    //             return;
    //         }
    //         this.$router.push({ path: '/' + result });
    //     }
    // },
    //$_onScroll: function () {
    //    this.offsetTop = window.pageYOffset || document.documentElement.scrollTop;
    //}
  },
  computed: {
    ...mapGetters(["getUniversityName"])
  }
};
</script>
<style src="./TheNavbar.less" lang="less"></style>


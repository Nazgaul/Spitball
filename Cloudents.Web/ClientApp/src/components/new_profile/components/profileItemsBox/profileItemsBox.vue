<template>
   <div id="profileItemsBox">
      <div
         v-for="(item, index) in items"
         :key="index"
      >
         <div>{{item.courseName}}</div>
         <v-slide-group
            v-model="model"
            class="profileitemsWrap pa-4"
            show-arrows
         >
            <v-slide-item v-for="(result) in item.result" :key="result.id">
               <itemCard v-if="$vuetify.breakpoint.smAndUp" :item="result" />
               <resultNote v-else :item="result" class="pa-3 mb-2" />
            </v-slide-item>
         </v-slide-group>
      </div>
   </div>
</template>

<script>
const itemCard = () => import(/* webpackChunkName: "itemCard" */ '../../../carouselCards/itemCard.vue');
import resultNote from "../../../results/ResultNote.vue";

import { mapGetters } from 'vuex'
export default {
   name:'profileItemsBox',
   components:{
      itemCard,
      resultNote
   },
   data() {
      return {
         model: false,
      }
   },
   watch: {
      query:{
         deep:true,
         handler() {
            this.updateItems()
         }
      }
   },
   computed: {
      ...mapGetters(['getProfile']),
      // pageCount(){
      //    return Math.ceil(this.$store.getters.getProfileDocuments?.count / this.query.pageSize);
      // },
      items(){
         return this.$store.getters.getProfileDocuments
      }
   },
   methods: {
      updateItems(){
         let type = 'documents'
         let params = {
            page: this.query.page -1,
            pageSize: this.query.pageSize,
            documentType: this.query.documentType,
            course: this.query.course,
         }
         Object.keys(params).forEach((key) => (params[key] === '') && delete params[key]);
         this.getItems(type,params).then(()=>{
            let scrollDiv = document.getElementById("profileItemsBox").offsetTop;
            window.scrollTo({ top: scrollDiv, behavior: 'smooth'});
         })

      },
      getItems(type,params){
         let dataObj = {
               id: this.$route.params.id,
               type,
               params
         }
         return this.$store.dispatch('updateProfileItemsByType',dataObj)
      }
   }
}
</script>

<style lang="less">
@import '../../../../styles/mixin.less';
#profileItemsBox{
   width: 100%;
   max-width: 1006px;
   margin: 0 auto;
   color: #43425d;

   .profileitemsWrap {
      .v-slide-group__prev, .v-slide-group__next {
         cursor: pointer;
         position: absolute;
         top: 50%;
         height: 40px;
         z-index: 1;
         background: white;
         border-radius: 50%;
         box-shadow: 0 1px 8px 0 rgba(0, 0, 0, 0.3);
         min-width: 40px !important;
      }
   }
}
</style>
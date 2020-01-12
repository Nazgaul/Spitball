<template>
<div id="profileItemsBox" v-if="items">
   <div class="profileItemsBox_title" v-text="$Ph('profile_study_materials',getProfile.user.firstName)"/>
   <!-- <div class="profileItemsBox_filters">
      <v-flex xs1 sm4 pr-4>
         <v-select class="profileItemsBox_filters_select"
            :append-icon="'sbf-arrow-fill'"
            v-model="selectedTypeItem"
            :items="typeItems"
            item-text="name"
            height="36" dense hide-details solo>
         </v-select>
      </v-flex>
      <v-flex xs11 sm9 d-none>
         <v-select class="profileItemsBox_filters_select"
            :append-icon="'sbf-arrow-fill'"
            :items="['Documents','Documents','Documents','Documents']"
            label="Solo field" height="36" dense
            solo>
         </v-select>
      </v-flex>
   </div> -->
   <!-- {{items.length}} -->
   <div class="profileItemsBox_content" v-if="$vuetify.breakpoint.smAndUp">
      <itemCard v-for="(item, index) in items" :key="index" :item="item"/>
   </div>
   <div v-if="$vuetify.breakpoint.xsOnly" class="profileItemsBox_content_mobile">
      <resultNote v-for="(item, index) in items" :key="index" :item="item" class="pa-3 mb-2"/>
   </div>
   <div class="profileItemBox_pagination" v-if="pageCount > 1">
      <v-pagination circle
         total-visible=7 
         v-model="query.page" 
         :length="pageCount"
         @input="goSelected"
         :next-icon="`sbf-arrow-right-carousel`"
         :prev-icon="`sbf-arrow-left-carousel`"/>
   </div>
</div>
</template>

<script>
import itemCard from '../../../carouselCards/itemCard.vue';
import resultNote from "../../../results/ResultNote.vue";

import { mapGetters } from 'vuex'
export default {
   name:'profileItemsBox',
   components:{
      itemCard,
      resultNote
   },
   props:{
      globalFunctions:{
         type: Object,
         required:true
      }
   },
   data() {
      return {
         typeItems:[
            {name:'Document',value:'documents'},
            {name:'Video',value:'videos'},
            {name:'Answer',value:'answers'},
            {name:'Question',value:'questions'},
         ],
         selectedTypeItem:'documents',
         query:{
            type:'documents',
            page: 1,
            pageSize:6,
         }
      }
   },
   computed: {
      ...mapGetters(['getProfile']),
      pageCount(){
         return Math.ceil(this.getProfile[this.selectedTypeItem].count / this.query.pageSize);
      },
      items(){
         return this.getProfile[this.selectedTypeItem].result;
      },
   },
   methods: {
      goSelected(){
         let itemsObj = {
            type: this.query.type,
            page: this.query.page -1,
            pageSize: this.query.pageSize
         }
         this.globalFunctions.getItems(itemsObj).then(()=>{
            let scrollIntoViewOptions = {
                behavior: 'smooth',
                block: 'start',
            }
            document.getElementById('profileItemsBox').scrollIntoView(scrollIntoViewOptions);
         })

      },
   },
   created() {
      this.query.pageSize = this.$vuetify.breakpoint.xsOnly? 3 : 6
   },
}
</script>

<style lang="less">
@import '../../../../styles/mixin.less';

#profileItemsBox{
   @media (max-width: @screen-xs) {
      // padding: 0 14px;
   }
   width: 100%;
   color: #43425d;
   .profileItemsBox_title{
      font-size: 18px;
      font-weight: 600;
      @media (max-width: @screen-xs) {
         padding: 10px 14px;
         background: white;
         font-size: 16px;
         font-weight: bold;
      }
   }
   .profileItemsBox_filters{
      display: flex;
      justify-content: space-between;
      padding-top: 12px;
      
      .profileItemsBox_filters_select{
         height: 36px;
         .v-input__control{
            min-height: auto !important;
            display: unset;
            height: 36px;
            .v-input__slot{
               // padding: 0 16px;
               border-radius: 8px;
               box-shadow: 0 1px 4px 0 rgba(0, 0, 0, 0.15);
               margin: 0;
               .v-select__slot{
                  font-size: 14px;
                  .v-select__selections{
                     padding-left: 10px;
                  }
                  .v-input__append-inner{
                     .v-input__icon{
                           i{
                              font-size: 6px;
                              color: #43425d;
                           }
                     }
                  }
               }
            }
         }
      }
   }
   .profileItemsBox_content_mobile{
      width: 100%;
      padding-bottom: 20px;
      .note-block{
         border-radius: unset;
      }
   }
   .profileItemsBox_content{
      width: 100%;
      display: flex;
      flex-flow: row wrap;
      // justify-content: space-between;

      display: grid;
      box-sizing: border-box;
      grid-gap: 14px;
      padding-bottom: 20px;
      grid-template-columns: repeat(auto-fill, 230px);
      .itemCarouselCard{
         flex: 0 0 32%;
         margin-top: 20px;
         width: 230px;
      }
      // .itemCarouselCard:nth-child(3n-1) {
      //    margin-left: 2%;
      //    margin-right: 2%;
      // }
   }
   .profileItemBox_pagination{
      text-align: center;
      .v-pagination__item{
         background-color: initial !important;
         box-shadow: none !important;
         outline:none;
         &.v-pagination__item--active{
            color: initial !important;
            background-color: initial !important;
            border: none !important;
            border: 1.5px solid rgb(68, 82, 252) !important;
            outline:none;
            color: rgb(68, 82, 252) !important;
            font-weight: bold;
         }
      }
      .v-pagination__navigation{
         background-color: initial !important;
         box-shadow: none !important;
         outline:none;
         i{
            transform: scaleX(1)/*rtl:scaleX(-1)*/; 
            color: rgb(68, 82, 252) !important;
            font-size: 16px;
         }
      }
   }
}
</style>
<template>
<div class="profileItemsBox" v-if="itemToPreview.length">
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
   <div class="profileItemsBox_content">
      <itemCard v-for="(item, index) in itemToPreview" :key="index" :item="item"/>
   </div>
   <!-- <div class="profileItemsBox_content hidden-sm-and-up">
      <resultNote class="pa-3 mb-3" v-for="(item, index) in itemToPreview" :key="index" :item="item"/>
   </div> -->
   <div class="profileItemBox_pagination" v-if="pagination.length > 1">
      <v-pagination circle
         total-visible=5 
         v-model="pagination.current" 
         :length="pagination.length"
         @next="goNext"
         @input="goSelected"
         @previous="goPrevious"
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
   data() {
      return {
         typeItems:[
            {name:'Document',value:'documents'},
            {name:'Video',value:'videos'},
            {name:'Answer',value:'answers'},
            {name:'Question',value:'questions'},
         ],
         selectedTypeItem:'documents',
         pagination:{
            length:0,
            current:1,
         },
         query:{
            page: 0,
            pageSize:6,
         }
      }
   },
   computed: {
      ...mapGetters(['getProfile']),
      items(){
         return this.getProfile[this.selectedTypeItem]
      },
      itemToPreview(){
         let startIdx = (this.pagination.current * this.query.pageSize) - this.query.pageSize;
         let endIdx = this.pagination.current * this.query.pageSize;
         return this.items.slice(startIdx,endIdx)
      }
   },
   methods: {
      goNext(){},
      goSelected(){},
      goPrevious(){},
   },
   watch: {
      items(){
         this.pagination.length = Math.ceil(this.items.length / this.query.pageSize);
      }
   },
}
</script>

<style lang="less">
@import '../../../../styles/mixin.less';

.profileItemsBox{
   @media (max-width: @screen-xs) {
      padding: 0 14px;
   }
   width: 100%;
   color: #43425d;
   .profileItemsBox_title{
      font-size: 18px;
      font-weight: 600;
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
   .profileItemsBox_content{
      width: 100%;
      display: flex;
      flex-flow: row wrap;
      justify-content: flex-start;
      padding-bottom: 20px;
      .itemCarouselCard{
         flex: 0 0 32%;
         margin-top: 20px;
         width: 230px;
      }
      .itemCarouselCard:nth-child(3n-1) {
         margin-left: 2%;
         margin-right: 2%;
      }
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
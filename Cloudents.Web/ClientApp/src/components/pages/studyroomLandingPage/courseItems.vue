<template>
   <div id="courseContentSection" :class="{'courseItemsSpace': isEnrolled}" class="courseItems pb-0 px-4 px-sm-4 px-lg-0" v-if="courseItemsList.length">
      <div class="courseTitle">{{courseItemsTitle}}</div>
      <v-divider class="mt-3" width="118" style="min-height:3px" color="#41c4bc"></v-divider>
      <div class="courseSubtitle pt-3 pb-6 pe-12">{{courseItemsSubtitle}}</div>
      <v-row class="itemsWrapper">
         <v-col class="px-0 px-sm-3 py-2 py-sm-3" v-for="(item) in itemToPreview" :key="item.id" cols="12" md="3" sm="6" >
            <itemCard :item="item"/>
         </v-col>
      </v-row>
      <div class="itemPagination" v-if="courseItemsList.length && pagination.length > 1">
         <v-pagination circle 
            v-model="pagination.current" 
            :length="pagination.length"
            total-visible=5
            :next-icon="`sbf-arrow-right-carousel`"
            :prev-icon="`sbf-arrow-left-carousel`"/>
      </div>
   </div>
</template>

<script>
import itemCard from '../../carouselCards/itemCard.vue';
export default {
   components:{
      itemCard
   },
   props: {
      isEnrolled: {
         type: Boolean
      }
   },
   data() {
      return {
         pagination:{
            length:0,
            current:1,
         },
      }
   },
   computed: {
      courseItemsTitle(){
         if(this.$store.getters.getCourseItemsContentTitlePreview){
            return this.$store.getters.getCourseItemsContentTitlePreview
         }else{
            if(this.isCourseEnrolled){
               return this.$t('courseItemsTitle_enrolled');
            }else{
               return this.$t('courseItemsTitle');
            }
         }
      },
      courseItemsSubtitle(){
         return this.$store.getters.getCourseItemsContentTextPreview || this.$t('courseItemsAccsess');
      },
      isCourseEnrolled(){
         return this.$store.getters.getIsCourseEnrolled;
      },
      pageSize(){
         if(this.$vuetify.breakpoint.xsOnly){
            return 9
         }else{
            return 12
         }
      },
      courseItemsList(){
         return this.$store.getters.getCourseItems;
      },
      itemToPreview(){
         let startIdx = (this.pagination.current * this.pageSize) - this.pageSize;
         let endIdx = this.pagination.current * this.pageSize;
         return this.courseItemsList.slice(startIdx,endIdx)
      },
   },
   watch: {
      courseItemsList(){
         this.pagination.length = Math.ceil(this.$store.getters.getCourseItems.length / this.pageSize)
      }
   },
}
</script>

<style lang="less">
   @import '../../../styles/mixin.less';
   .courseItems{
      background: white;
      width: 100%;
      color: #43425d;
      margin-top: 80px !important;
      @media (max-width: @screen-xs) {
         margin-top: 60px !important;
      }
      &.courseItemsSpace {
         margin-bottom: 80px !important;
         @media (max-width: @screen-xs) {
            margin-bottom: 60px !important;
         }
      }
      .courseTitle{
         font-size: 28px;
         font-weight: 600;
      }
      .courseSubtitle{
         font-size: 20px;
         line-height: 1.56;
      }
      .itemsWrapper{
         width: 100%;
         margin: 0 auto;
         max-width: 1100px;
      }
   }
   .itemPagination{
         padding: 12px 0 20px;
        text-align: center;
        button{
            outline: none;
        }
        .v-pagination__item{
            background-color: initial !important;
            box-shadow: none !important;
            &.v-pagination__item--active{
                color:  rgb(68, 82, 252) !important;
                font-weight: bold;
                background-color: initial !important;
                border: 1px solid rgb(68, 82, 252) !important;
            }
        }

        .v-pagination__navigation{
            background-color: initial !important;
            box-shadow: none !important;
            i{
                color: rgb(68, 82, 252) !important;
                font-size: 16px;
            }
        }
   }

</style>


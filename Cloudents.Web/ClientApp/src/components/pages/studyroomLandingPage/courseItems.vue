<template>
   <div class="courseItems mt-7 py-5" v-if="courseItemsList.length">
      <div class="courseTitle">{{$t('courseItemsTitle')}}</div>
      <v-divider class="mt-3" width="118" style="min-height:3px" color="#41c4bc"></v-divider>
      <div class="courseSubtitle pt-4 pb-11 pe-12">{{$t('courseItemsAccsess')}}</div>
      <v-row class="itemsWrapper">
         <v-col v-for="(item) in itemToPreview" :key="item.id" cols="12" md="3">
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
   data() {
      return {
         pagination:{
            length:0,
            current:1,
            pageSize:12,
         },
      }
   },
   computed: {
      courseItemsList(){
         return this.$store.getters.getCourseItems;
      },
      itemToPreview(){
         let startIdx = (this.pagination.current * this.pagination.pageSize) - this.pagination.pageSize;
         let endIdx = this.pagination.current * this.pagination.pageSize;
         return this.courseItemsList.slice(startIdx,endIdx)
      },
   },
   watch: {
      courseItemsList(){
         this.pagination.length = Math.ceil(this.$store.getters.getCourseItems.length / this.pagination.pageSize)
      }
   },
}
</script>

<style lang="less">
   .courseItems{
      background: white;
      width: 100%;
      color: #43425d;

      .courseTitle{
         font-size: 28px;
         font-weight: 600;
      }
      .courseSubtitle{
         font-size: 18px;
         line-height: 1.56;
      }
      .itemsWrapper{
         width: 100%;
         margin: 0 auto;
         max-width: 1100px;
      }
   }
   .itemPagination{
    padding: 20px 0;
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


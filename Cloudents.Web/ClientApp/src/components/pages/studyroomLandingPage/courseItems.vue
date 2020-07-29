<template>
   <div class="courseItems py-5">
      <div class="courseTitle">{{$t('courseItemsTitle')}}</div>
      <v-divider class="mt-3" width="118" style="min-height:3px" color="#41c4bc"></v-divider>
      <div class="courseSubtitle pt-4 pb-11 pe-12">{{$t('courseItemsAccsess')}}</div>
      <div class="itemsWrapper d-flex flex-wrap">
         <div style=" flex: 1 0 21%" v-for="(item) in courseItemsList" :key="item.id">
            <itemCard :item="item"/>
         </div>
      <!-- paging -->
      </div>

   </div>
</template>

<script>
import itemCard from '../../carouselCards/itemCard.vue';
export default {
   components:{
      itemCard
   },
   computed: {
      courseItemsList(){
         return this.$store.getters.getCourseItems;
      }
   },

}
</script>

<style lang="less">
   .courseItems{
      background: white;
      width: 100%;
      height: 500px;
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
      }
      
   }
</style>


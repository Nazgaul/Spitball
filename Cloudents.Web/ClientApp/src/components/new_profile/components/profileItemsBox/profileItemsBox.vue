<template>
   <div id="profileItemsBox">
      <div
         v-for="(item, index) in items"
         :key="index"
         class="itemsContainer"
      >
         <div class="itemBoxTitle">{{item.courseName}}</div>
         <v-slide-group
            v-model="model"
            class="profileitemsWrap"
         >
            <v-slide-item v-for="(result) in item.result" :key="result.id">
               <v-card class="profileItemCard mb-1 elevation-0">
                  <itemCard class="itemCard-profilePage" v-if="$vuetify.breakpoint.smAndUp" :item="result" />
                  <resultNote v-else :item="result" class="pa-3 mb-2" />
               </v-card>
            </v-slide-item>
         </v-slide-group>
      </div>
   </div>
</template>

<script>
// const itemCard = () => import(/* webpackChunkName: "itemCard" */ '../../../carouselCards/itemCard.vue');
import itemCard from '../../../carouselCards/itemCard.vue'
import resultNote from "../../../results/ResultNote.vue";

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
            this.getItems()
         }
      }
   },
   computed: {
      items(){
         return this.$store.getters.getProfileDocuments
      }
   },
   methods: {
      getItems(){
         this.$store.dispatch('updateProfileItemsByType', this.$route.params.id)
      }
   }
}
</script>

<style lang="less">
@import '../../../../styles/mixin.less';
#profileItemsBox {
   width: 100%;
   max-width: 960px;
   margin: 0 auto;
   color: #43425d;

   .itemsContainer {
      .itemBoxTitle {
         font-size: 26px;
         font-weight: 600;
         color: #363637;
   
         margin: 62px 0 29px;
      }

      &:first-child {
         .itemBoxTitle {
            margin-top: 0;
         }
      }

      .profileitemsWrap {
         // vuetify overide arrows buttons
         .v-slide-group__prev--disabled , .v-slide-group__next--disabled {
            display: none;
         }
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

            i {
               font-size: 16px;
               color: #ff6f30;
            }
         }
         .v-slide-group__prev {
            left: -20px;
         }
         .v-slide-group__next {
            right: -20px;
         }

         .profileItemCard {
            margin: 0 10px;
            border-radius: 6px;
            &:first-child {
               margin-left: 0;
            } 
            &:last-child {
               margin-right: 0;
            }
         }
         .itemCarouselCard{
            border: 1px solid #ddd;
            box-shadow: none;
            flex: 0 0 32%;
            width: 230px;
            height: 100%;
         }
      }
   }
}
</style>
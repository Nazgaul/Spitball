<template>
   <div id="profileItemsBox">
      <div
         v-for="(item, index) in filterItems"
         :key="index"
         class="itemsContainer"
         :id="item.courseName"
      >
         <div class="itemBoxTitle mx-4 mx-sm-0 pb-2 pb-sm-0">{{item.courseName}}</div>
         <v-slide-group
            v-model="model"
            class="profileitemsWrap"
            style="direction: ltr;"
         >
            <v-slide-item v-for="(result) in item.result" :key="result.id" >
               <v-card class="profileItemCard mb-1 elevation-0" >
                  <itemCard class="itemCard-profilePage" v-if="$vuetify.breakpoint.smAndUp" :item="result" />
                  <resultNote v-else :item="result" class="pa-3 mb-2" />
               </v-card>
            </v-slide-item>
         </v-slide-group>
         <div class="showMoreItem text-center mt-2 mt-sm-0" v-show="isMobile">
            <v-btn class="btnMore" color="#fff" fab depressed small dark @click="expandItems(item)" v-if="item.count > 2">
                <arrowDownIcon class="arrowIcon" :class="{'exapnd': item.isExpand}" width="22" />
            </v-btn>
        </div>
      </div>
   </div>
</template>

<script>
// const itemCard = () => import(/* webpackChunkName: "itemCard" */ '../../../carouselCards/itemCard.vue');
import itemCard from '../../../carouselCards/itemCard.vue'
import resultNote from "../../../results/ResultNote.vue";
import arrowDownIcon from '../profileLiveClasses/group-3-copy-16.svg'

export default {
   name:'profileItemsBox',
   components:{
      itemCard,
      resultNote,
      arrowDownIcon
   },
   data() {
      return {
         model: false
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
      isMobile() {
         return this.$vuetify.breakpoint.xsOnly
      },
      items(){
         return this.$store.getters.getProfileDocuments
      },
      filterItems() {
         if(!this.isMobile) return this.items
         return this.items.map(item => {
            return {
               ...item,
               result: !item.isExpand ? item.result.slice(0,2) : item.result.slice(0)
            }
         })
      }
   },
   methods: {
      getItems(){
         this.$store.dispatch('updateProfileItemsByType', this.$route.params.id)
      },
      expandItems(item) {
         this.$store.commit('setExpandItems', item)
         if(item.isExpand)  {
            this.$vuetify.goTo(`#${item.courseName}`)
         }
      }
   }
}
</script>

<style lang="less">
@import '../../../../styles/mixin.less';
#profileItemsBox {
   width: 100%;
   max-width: 960px;
   margin: 80px auto;
   color: #43425d;
   @media (max-width: @screen-xs) {
      margin: 50px auto;
   }

   .itemsContainer {
      .itemBoxTitle {
         .responsive-property(font-size, 22px, null, 20px);
         font-weight: 600;
         color: #363637;
         margin: 52px 0 16px;

         @media (max-width: @screen-xs) {
            border-bottom: 2px solid #ebecef;
            margin: 40px 0 8px;
         }
      }

      &:first-child {
         .itemBoxTitle {
            margin-top: 0;
         }
      }

      .profileitemsWrap {
         // vuetify overide arrows buttons
         @media (max-width: @screen-xs) {
            background: #f5f5f5;
         }
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
            /*rtl:ignore */
            left: -20px;
         }
         .v-slide-group__next {
            /*rtl:ignore */
            right: -20px;
         }

         .profileItemCard {
            margin: 0 10px;
            border-radius: 6px;

            @media (max-width: @screen-xs) {
               margin: 0;
               border-radius: 0;
            }
            &:first-child {
               /*rtl:ignore */
               margin-left: 0;
            } 
            &:last-child {
               /*rtl:ignore */
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

         // mobile
         .v-slide-group__content {
            @media (max-width: @screen-xs) {
               width: 100%;
               display: block;
            }
         }
      }
      .showMoreItem {
         margin-bottom: 60px;
         @media (max-width: @screen-xs) {
            margin-bottom: unset;
         }
         .btnMore {
            border: 1px solid #d4d6da !important;
         }
         .arrowIcon {
            padding-top: 1px;
            cursor: pointer;
            &.exapnd {
                  transform: scaleY(-1);
            }
         }
      }
   }
}
</style>
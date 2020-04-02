<template>
   <div class="myPurchases">
      <v-data-table 
            calculate-widths
            :page.sync="paginationModel.page"
            :headers="headers"
            :items="purchasesItems"
            :items-per-page="20"
            sort-by
            :item-key="'date'"
            class="elevation-1"
            :footer-props="{
               showFirstLastPage: false,
               firstIcon: '',
               lastIcon: '',
               prevIcon: 'sbf-arrow-left-carousel',
               nextIcon: 'sbf-arrow-right-carousel',
               itemsPerPageOptions: [20]
            }">
            <template v-slot:top >
               <div class="myPurchases_title">
                     {{$t('dashboardPage_my_purchases_title')}}
                     </div>
            </template>
            <template v-slot:item.preview="{item}">
               <router-link :to="{name: 'profile',params: {id: item.id, name: item.name}}">
                  <user-avatar v-if="item.name && !item.preview" 
                     :user-id="item.userId" 
                     :user-image-url="item.image" 
                     :size="'40'" 
                     :user-name="item.name" >
                  </user-avatar>
               </router-link>
               <v-avatar size="40" class="d-flex" style="margin:0 auto;" v-if="!item.name && !item.preview">
                  <img :src="item.image" alt="John">
               </v-avatar>  
               <router-link class="d-flex justify-center" v-if="item.url" :to="item.url">
                  <v-avatar size="40" v-if="item.preview">
                     <img :src="item.preview" alt="John">
                  </v-avatar>  
               </router-link>
            </template>
            <template v-slot:item.info="{item}">
               <tableInfoTd :item="item"/>
            </template>
            <template v-slot:item.type="{item}">
               {{dictionary.types[item.type]}}
            </template>
            <template v-slot:item.price="{item}">
               {{formatPrice(item.price,item.type)}}
            </template>
            <template v-slot:item.date="{item}">
               {{ $d(new Date(item.date)) }}
            </template>
            <template v-slot:item.action="{item}">
               <v-btn v-if="item.type !== 'TutoringSession' && item.type !== 'BuyPoints'" @click="dynamicAction(item)"
                  icon depressed rounded color="#69687d" small>
                  <v-icon v-text="item.type === 'Document'?'sbf-download':'sbf-watch'"/>
               </v-btn>
            </template>

            <slot slot="no-data" name="tableEmptyState"/>
      </v-data-table>
   </div>
</template>

<script>
import { mapActions, mapGetters } from 'vuex';

import tableInfoTd from '../global/tableInfoTd.vue';
export default {
   name:'myPurchases',
   components:{tableInfoTd},
   props:{
      dictionary:{
         type: Object,
         required: true
      }
   },
   data() {
      return {
         paginationModel:{
            page:1
         },
         headers:[
            this.dictionary.headers['preview'],
            this.dictionary.headers['info'],
            this.dictionary.headers['type'],
            this.dictionary.headers['price'],
            this.dictionary.headers['date'],
            this.dictionary.headers['action'],
         ],
      }
   },
   computed: {
      ...mapGetters(['getPurchasesItems','accountUser']),
      purchasesItems(){
         return this.getPurchasesItems
      },
   },
   methods: {
      ...mapActions(['updatePurchasesItems']),
      formatPrice(price,type){
         if(isNaN(price)) return;
         if(price < 0){
            price = Math.abs(price)
         }
         price = Math.round(+price).toLocaleString();
         if(type === 'Document' || type === 'Video' ){
            return `${price} ${this.$t('dashboardPage_pts')}`
         }
         if(type === 'TutoringSession' || type === 'BuyPoints'){
            return `${price} ${this.accountUser.currencySymbol}`
         }
      },
      dynamicAction(item){
         if(item.type === 'Document' || item.type === 'Video'){
            this.$router.push({path:item.url})
         }
      },
   },
   created() {
      this.updatePurchasesItems()
   },
}
</script>

<style lang="less">
.myPurchases{
   max-width: 1334px;
   .myPurchases_title{
      font-size: 22px;
      color: #43425d;
      font-weight: 600;
      padding: 30px;
      line-height: 1.3px;
      background: #fff;
      // box-shadow: 0 2px 1px -1px rgba(0,0,0,.2),0 1px 1px 0 rgba(0,0,0,.14),0 1px 3px 0 rgba(0,0,0,.12)!important;
   }
   tr{
      height:54px;
   }
   td{
      border: none !important;
   }
   td:first-child {
      width:1%;
      white-space: nowrap;
   }
   tr:nth-of-type(2n) {
      td {
         background-color: #f5f5f5;
      }
   }
   thead{
      tr{
         th{
            color: #43425d !important;
            font-size: 14px;
            padding-top: 14px;
            padding-bottom: 14px;
            font-weight: normal;
            min-width: 100px;
         }
         
      }
      color: #43425d !important;
   }
   .sbf-arrow-right-carousel, .sbf-arrow-left-carousel {
      transform: none /*rtl:rotate(180deg)*/;
      color: #43425d !important;
      height: inherit;
      font-size: 14px !important;
   }
   .v-data-footer {
      padding: 6px 0;
      .v-data-footer__pagination {
         font-size: 14px;
         color: #43425d;
      }
   }
}
</style>
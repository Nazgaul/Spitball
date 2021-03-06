<template>
   <div class="myPurchases">
      <div class="myPurchases_title" v-t="'dashboardPage_my_purchases_title'"/>
      <v-data-table 
            :headers="headers"
            :items="purchasesItems"
            sort-by
            :item-key="'date'"
            :items-per-page="5"
            mobile-breakpoint="0"
            hide-default-header
            :footer-props="{
               showFirstLastPage: false,
               firstIcon: '',
               lastIcon: '',
               prevIcon: 'sbf-arrow-left-carousel',
               nextIcon: 'sbf-arrow-right-carousel',
               itemsPerPageOptions: [5]
            }">
            <template v-slot:header="{props}">
               <thead>
                  <tr>
                     <th class="text-start"
                        v-for="header in props.headers"
                        :key="header.value"
                        :class="['column',{'sortable':header.sortable}]"
                        @click="changeSort(header.value)">
                        <span class="text-start">{{ header.text }}
                           <v-icon v-if="header.sortable" v-html="sortedBy !== header.value?'sbf-arrow-down':'sbf-arrow-up'" />
                        </span>
                     </th>
                  </tr>
               </thead>
            </template>

            <template v-slot:item="props">
               <tr class="myPurchases_table_tr">
                  <tablePreviewTd :item="props.item"/>
                  <td>
                     <tableInfoTd :item="props.item"/>
                  </td>
                  <td class="text-start" v-text="dictionary.types[props.item.type]"/>
                  <td class="text-start" v-text="formatPrice(props.item.price,props.item.type)"/>
                  <td class="text-start">{{$moment(props.item.date).format('MMM D, YYYY')}}</td> 
                  
                  
                  <td class="text-center">
                     <button v-if="props.item.type !== 'TutoringSession' && props.item.type !== 'BuyPoints' && props.item.type !== 'Course'" @click="dynamicAction(props.item)" class="myPurchases_action">
                        {{dynamicResx(props.item.type)}}
                     </button>
                  </td> 
               </tr> 
            </template>

            <slot slot="no-data" name="tableEmptyState"/>
      </v-data-table>
   </div>
</template>

<script>
import { mapActions, mapGetters } from 'vuex';

import tablePreviewTd from '../global/tablePreviewTd.vue';
import tableInfoTd from '../global/tableInfoTd.vue';
export default {
   name:'myPurchases',
   components:{tablePreviewTd,tableInfoTd},
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
         sortedBy:'',
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
      ...mapActions(['updatePurchasesItems','dashboard_sort']),
      formatPrice(price,type){
         if(isNaN(price)) return;
         if(price < 0){
            price = Math.abs(price)
         }
         price = Math.round(+price).toLocaleString();
         if(type === 'Document' || type === 'Video' ){
            return `${price} ${this.$t('dashboardPage_pts')}`
         }
         if(type === 'TutoringSession' || type === 'BuyPoints' || type === 'Course'){
            return this.$n(price, {'style':'currency','currency': this.accountUser.currencySymbol});
         }
      },
      dynamicResx(type){
         if(type === 'Document'){
            return this.$t('dashboardPage_action_download')
         }else{
            return this.$t('dashboardPage_action_watch')
         }
      },
      dynamicAction(item){
         if(item.type === 'Document' || item.type === 'Video'){
            this.$router.push({path:item.url})
         }
      },
      changeSort(sortBy){
         if(sortBy === 'info') return;

         let sortObj = {
            listName: 'purchasesItems',
            sortBy,
            sortedBy: this.sortedBy
         }
         this.dashboard_sort(sortObj)
         this.paginationModel.page = 1;
         this.sortedBy = this.sortedBy === sortBy ? '' : sortBy;
      }
   },
   created() {
      this.updatePurchasesItems()
   },
}
</script>

<style lang="less">
.myPurchases{
   // max-width: 1334px;
   .myPurchases_title{
      font-size: 22px;
      color: #43425d;
      font-weight: 600;
      padding: 30px;
      line-height: 1.3px;
      background: #fff;
      // box-shadow: 0 2px 1px -1px rgba(0,0,0,.2),0 1px 1px 0 rgba(0,0,0,.14),0 1px 3px 0 rgba(0,0,0,.12)!important;
   }
   thead{
      tr{
         height: auto;
         th{
            color: #43425d !important;
            font-size: 14px;
            // padding-top: 14px;
            // padding-bottom: 14px;
            font-weight: normal;
            min-width: 100px;
            // padding: 14px 24px
         }
      }
      color: #43425d !important;
   }
   .myPurchases_table_tr {
      td {
         // font-size: 13px !important;
         &:first-child {
            padding-right: 0;   
         }
         padding: 0 24px
      }
   }
   .myPurchases_action{
      outline: none;
      padding: 10px 0;
      width: 100%;
      max-width: 140px;
      border: 1px solid black;
      border-radius: 26px;
      text-transform: capitalize;
      font-weight: 600;
      font-size: 14px;
   }
   .v-data-footer {
      padding: 6px 0;
      .sbf-arrow-right-carousel, .sbf-arrow-left-carousel {
         color: #43425d !important;
         height: inherit;
         font-size: 14px;
      }
      .v-data-footer__pagination {
         font-size: 14px;
         color: #43425d;
      }
   }
}
</style>
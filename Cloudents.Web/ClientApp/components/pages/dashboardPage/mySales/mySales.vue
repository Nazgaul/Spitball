<template>
   <div class="mySales">
      <div class="mySales_title" v-language:inner="'dashboardPage_my_sales_title'"/>
      <v-layout wrap class="mySales_wallet mb-1 mb-md-7 mb-sm-4" v-if="!!accountUser && accountUser.id">
         <v-flex sm12 md6>
            <v-data-table
               :headers="balancesHeaders"
               :items="balancesItems"
               sort-by
               hide-default-footer
               hide-default-header
               :item-key="'date'"
               class="elevation-1 mySales_table">
               <template v-slot:header="{props}">
                  <thead>
                     <tr>
                        <th class="text-center"
                           v-for="header in props.headers"
                           :key="header.value">
                           {{header.text}}
                        </th>
                     </tr>
                  </thead>
               </template>
               <template v-slot:item="props">
                  <tr class="mySales_table_tr">
                     <td class="text-left">
                        <span>{{wallet[props.item.type]}}</span>
                     </td>
                     <td class="text-center">
                        <span>{{formatBalancePts(props.item.points)}}</span>
                     </td> 
                     <td class="text-center">
                        <span class="font-weight-bold">
                           {{$n(props.item.value, 'currency')}}
                        </span>
                     </td> 
                  </tr>
               </template>
            </v-data-table>
         </v-flex>
         <v-flex sm12 md6 :class="[{'mt-1':$vuetify.breakpoint.xsOnly},{'mt-3':$vuetify.breakpoint.smAndDown && !$vuetify.breakpoint.xsOnly}]">
            <div class="mySales_actions ml-md-6">
               <redeemPointsLayout class="my-2 my-md-0 mx-lg-2 "/>
               <buyPointsLayout class="my-2 my-md-0 mx-lg-2"/>
            </div>
         </v-flex>
      </v-layout>
      
         <v-data-table
            :headers="headers"
            :items="salesItems" 
            :items-per-page="5"
            sort-by
            hide-default-header
            :item-key="'date'"
            class="elevation-1 mySales_table"
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
                     <th class="text-left"
                        v-for="header in props.headers"
                        :key="header.value"
                        :class="['column',{'sortable':header.sortable}]"
                        @click="changeSort(header.value)">
                        <span class="text-left">{{ header.text }}
                           <v-icon v-if="header.sortable" v-html="sortedBy !== header.value?'sbf-arrow-down':'sbf-arrow-up'" />
                        </span>
                     </th>
                  </tr>
               </thead>
            </template>
            <!-- <template slot="headers" slot-scope="props">
               <tr>
                  <th class="text-xs-left"
                     v-for="header in props.headers"
                     :key="header.value"
                     :class="['column',{'sortable':header.sortable}]"
                     @click="changeSort(header.value)">
                     <span class="text-xs-left">{{ header.text }}
                        <v-icon v-if="header.sortable" v-html="sortedBy !== header.value?'sbf-arrow-down':'sbf-arrow-up'" />
                     </span>
                  </th>
               </tr>
            </template> -->
            <template v-slot:item="props">
               <tr class="mySales_table_tr">
                  <tablePreviewTd :item="props.item"/>
                  <tableInfoTd :item="props.item"/>

                  <td class="text-left" v-text="dictionary.types[props.item.type]"/>
                  <td class="text-left" v-text="formatItemStatus(props.item.paymentStatus)"/>
                  <td class="text-left">{{ $d(new Date(props.item.date)) }}</td>
                  <td class="text-left" v-text="formatPrice(props.item.price,props.item.type)"></td>
                  <td>
                     <v-btn 
                        color="#02C8BF"
                        class="white--text"
                        width="120"
                        depressed
                        rounded
                        v-if="pendingPayments && props.item.paymentStatus === 'PendingApproval' && props.item.type === 'TutoringSession'"
                        @click="$openDialog('teacherApproval', {item: props.item})">
                           {{$t('dashboardPage_btn_approve')}}
                     </v-btn>
                  </td>
               </tr>
            </template>

            <slot slot="no-data" name="tableEmptyState"/>
         </v-data-table>
   </div>
</template>

<script>
import { mapActions, mapGetters } from 'vuex';
import { LanguageService } from '../../../../services/language/languageService';

import tablePreviewTd from '../global/tablePreviewTd.vue';
import tableInfoTd from '../global/tableInfoTd.vue';
import buyPointsLayout from './buyPointsLayout/buyPointsLayout.vue'
import redeemPointsLayout from './redeemPointsLayout/redeemPointsLayout.vue'


export default {
   name:'mySales',
   components:{tablePreviewTd,tableInfoTd,buyPointsLayout,redeemPointsLayout},
   props:{
      dictionary:{
         type: Object,
         required: true
      },
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
            this.dictionary.headers['status'],
            this.dictionary.headers['date'],
            this.dictionary.headers['price'],
            '', // this is for empty th cell action approve button
         ],
         balancesHeaders:[
            this.dictionary.headers['preview'],
            this.dictionary.headers['points'],
            this.dictionary.headers['value'],
         ],
         wallet:{
            'Earned': this.$t(`wallet_earned`),
            'Spent': this.$t(`wallet_spent`),
            'Total': this.$t(`wallet_total`),
         }
      }
   },
   computed: {
      ...mapGetters(['getSalesItems','accountUser','getBalancesItems']),
      salesItems(){
         return this.getSalesItems;
      },
      balancesItems(){
         return this.getBalancesItems;
      },
      pendingPayments() {
         return this.$store.getters.getPendingPayment
      }
   },
   methods: {
      ...mapActions(['updateSalesItems','dashboard_sort']),
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
      formatBalancePts(pts){
         pts = Math.round(+pts).toLocaleString(`${global.lang}-${global.country}`);
         return `${pts} ${LanguageService.getValueByKey('dashboardPage_pts')}`
      },
      formatItemStatus(paymentStatus){
         if(paymentStatus === 'Pending'){
            return this.$t('dashboardPage_pending')
         }
         if(paymentStatus === 'Paid'){
            return this.$t('dashboardPage_paid')
         }
         if(paymentStatus === 'PendingApproval') {
            return this.$t('dashboardPage_pending_approve')
         }
      },
      changeSort(sortBy){
         if(sortBy === 'info') return;

         let sortObj = {
            listName: 'salesItems',
            sortBy,
            sortedBy: this.sortedBy
         }
         this.dashboard_sort(sortObj)
         this.paginationModel.page = 1;
         this.sortedBy = this.sortedBy === sortBy ? '' : sortBy;
      },
   },
   created() {
      this.updateSalesItems()
   },
}
</script>

<style lang="less">
@import '../../../../styles/mixin.less';
.mySales{
   .mySales_title{
      font-size: 22px;
      color: #43425d;
      font-weight: 600;
      padding: 0 0 10px 2px;
   }
   .mySales_wallet{
      .mySales_actions{
         display: flex;
         @media (max-width: @screen-md-plus) {
            flex-wrap: wrap;
            height: 100%;
            // justify-content: space-between;
            justify-content: space-evenly;
            align-content: space-between;
         }

      }
   }
   .mySales_table{
      max-width: 1334px;
      thead {
         tr{
            height: auto;
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
      .mySales_table_tr {
         td {
            font-size: 13px !important;
         }
      }
      .sbf-arrow-right-carousel, .sbf-arrow-left-carousel {
         transform: none /*rtl:rotate(180deg)*/;
         color: #43425d !important;
         height: inherit;
         font-size: 14px;
      }
      .v-data-footer {
         padding: 6px 0;
         .v-data-footer__pagination {
            font-size: 14px;
            color: #43425d;
         }
      }
   }

}
</style>
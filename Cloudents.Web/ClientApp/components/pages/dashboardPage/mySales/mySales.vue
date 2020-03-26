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
         calculate-widths
         :page.sync="paginationModel.page"
         :headers="headers"
         :items="salesItems"
         :items-per-page="20"
         sort-by
         :item-key="'date'"
         class="elevation-1 mySales_table-full"
         :footer-props="{
            showFirstLastPage: false,
            firstIcon: '',
            lastIcon: '',
            prevIcon: 'sbf-arrow-left-carousel',
            nextIcon: 'sbf-arrow-right-carousel',
            itemsPerPageOptions: [20]
         }">
         <template v-slot:item.preview="{item}">
            <router-link class="d-flex justify-center" v-if="item.preview" :to="item.url">
               <v-avatar size="40">
                  <img :src="item.preview">
               </v-avatar>
            </router-link>
            <router-link v-if="item.sessionId" :to="{name: 'profile',params: {id: item.id, name: item.name}}">
               <user-avatar :user-id="item.userId" 
               :user-image-url="item.image" 
               :size="'40'" 
               :user-name="item.name" >
               </user-avatar>
            </router-link>
         </template>
         <template v-slot:item.info="{item}">
            <tableInfoTd :item="item"/>
         </template>
         <template v-slot:item.type="{item}">
            {{dictionary.types[item.type]}}
         </template>
         <template v-slot:item.paymentStatus="{item}">
            {{formatItemStatus(item.paymentStatus)}}
         </template>
         <template v-slot:item.date="{item}">
            {{ $d(new Date(item.date)) }}
         </template>
         <template v-slot:item.price="{item}">
            {{formatPrice(item.price,item.type)}}
         </template>
         <template v-slot:item.action="{item}">
            <v-btn 
               color="#02C8BF"
               class="white--text"
               width="120"
               depressed
               rounded
               v-if="pendingPayments && item.paymentStatus === 'PendingApproval' && item.type === 'TutoringSession'"
               @click="$openDialog('teacherApproval', {item: item})">
                  {{$t('dashboardPage_btn_approve')}}
            </v-btn>
         </template>
            <slot slot="no-data" name="tableEmptyState"/>
         </v-data-table>
   </div>
</template>

<script>
import { mapActions, mapGetters } from 'vuex';
import { LanguageService } from '../../../../services/language/languageService';

import tableInfoTd from '../global/tableInfoTd.vue';
import buyPointsLayout from './buyPointsLayout/buyPointsLayout.vue'
import redeemPointsLayout from './redeemPointsLayout/redeemPointsLayout.vue'


export default {
   name:'mySales',
   components:{tableInfoTd,buyPointsLayout,redeemPointsLayout},
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
         headers:[
            this.dictionary.headers['preview'],
            this.dictionary.headers['info'],
            this.dictionary.headers['type'],
            this.dictionary.headers['status'],
            this.dictionary.headers['date'],
            this.dictionary.headers['price'],
            this.dictionary.headers['action'],
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
      ...mapActions(['updateSalesItems']),
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
      thead{
         tr{
            // height: auto;
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
   .mySales_table{
      max-width: 1334px; 
   }
   .mySales_table-full{
      max-width: 1334px;
      td:first-child {
         width:1%;
         white-space: nowrap;
      }
      tr:nth-of-type(2n) {
         td {
            background-color: #f5f5f5;
         }
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

}
</style>
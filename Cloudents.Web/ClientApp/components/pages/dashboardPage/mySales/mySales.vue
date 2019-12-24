<template>
   <div class="mySales">
      <div class="mySales_title" v-language:inner="'dashboardPage_my_sales_title'"/>
      <v-layout align-baseline row wrap class="mySales_wallet mb-5" v-if="!!accountUser && accountUser.id">
         <v-flex sm12 md6>
            <v-data-table
               :headers="balancesHeaders"
               :items="balancesItems"
               disable-initial-sort
               hide-actions
               :item-key="'date'"
               class="elevation-1 mySales_table"
               :prev-icon="'sbf-arrow-left-carousel'"
               :sort-icon="'sbf-arrow-down'"
               :next-icon="'sbf-arrow-right-carousel'">
               <template v-slot:items="props">
                  <td class="text-xs-left">
                     <span>{{dictionary.types[props.item.type]}}</span>
                  </td>
                  <td class="text-xs-center">
                     <span>{{formatBalancePts(props.item.points)}}</span>
                  </td> 
                  <td class="text-xs-center">
                     <span class="font-weight-bold">
                        {{ props.item.value | currencyFormat(props.item.symbol)}}
                     </span>
                  </td> 
               </template>
            </v-data-table>
         </v-flex>
         <v-flex sm12 md6 :class="[{'mt-3':$vuetify.breakpoint.smAndDown}]">
            <div class="mySales_cash-out-wrapper">
               <div class="mySales_text-wrap">
                     <!-- <div class="main-text" v-language:inner>wallet_more_SBL_more_valuable</div> -->
                     <div class="mySales_points-text">
                        <span>
                           <span v-language:inner>wallet_You_have</span>
                                 <bdi>
                           <span> {{Math.round(accountUser.balance).toLocaleString()}}
                                 <span v-language:inner="'cashoutcard_SBL'"/>
                                 </span>
                                       </bdi>
                           <span v-language:inner>wallet_you_have_redeemable_sbl</span>
                        </span>
                     </div>
               </div>
               
               <cash-out-card 
                  class="mySales_wallet_reedem" 
                  v-for="(cashOutOption,index) in cashOutOptions"
                  :key="index"
                  :points-for-dollar="cashOutOption.pointsForDollar"
                  :cost="cashOutOption.cost"
                  :image="cashOutOption.image"
                  :available="accountUser.balance >= cashOutOption.cost"
                  :updatePoint="recalculate">
               </cash-out-card>
            </div>




              
            
         </v-flex>
      </v-layout>
         <v-data-table
            :pagination.sync="paginationModel"
            :headers="headers"
            :items="salesItems"
            disable-initial-sort
            :item-key="'date'"
            :rows-per-page-items="['5']"
            class="elevation-1 mySales_table"
            :prev-icon="'sbf-arrow-left-carousel'"
            :sort-icon="'sbf-arrow-down'"
            :next-icon="'sbf-arrow-right-carousel'">
            <template slot="headers" slot-scope="props">
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
            </template>
            <template v-slot:items="props">
               <tablePreviewTd :globalFunctions="globalFunctions" :item="props.item"/>
               <tableInfoTd :globalFunctions="globalFunctions" :item="props.item"/>

               <td class="text-xs-left" v-html="dictionary.types[props.item.type]"/>
               <td class="text-xs-left" v-html="formatItemStatus(props.item.paymentStatus)"/>
               <td class="text-xs-left">{{ props.item.date | dateFromISO }}</td>
               <td class="text-xs-left" v-html="globalFunctions.formatPrice(props.item.price,props.item.type)"></td>
            </template>
            <slot slot="no-data" name="tableEmptyState"/>
            <slot slot="pageText" name="tableFooter"/>
         </v-data-table>
   </div>
</template>

<script>
import { mapActions, mapGetters } from 'vuex';
import { LanguageService } from '../../../../services/language/languageService';
import cashOutCard from '../../../wallet/cashOutCard/cashOutCard.vue';
import { cashOutCards } from '../../../wallet/consts.js';


import tablePreviewTd from '../global/tablePreviewTd.vue';
import tableInfoTd from '../global/tableInfoTd.vue';

export default {
   name:'mySales',
   components:{tablePreviewTd,tableInfoTd,cashOutCard},
   props:{
      globalFunctions: {
         type: Object,
      },
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
         cashOutOptions: cashOutCards,
         sortedBy:'',
         headers:[
            this.dictionary.headers['preview'],
            this.dictionary.headers['info'],
            this.dictionary.headers['type'],
            this.dictionary.headers['status'],
            this.dictionary.headers['date'],
            this.dictionary.headers['price'],
         ],
         balancesHeaders:[
            this.dictionary.headers['preview'],
            this.dictionary.headers['points'],
            this.dictionary.headers['value'],
         ]
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
   },
   methods: {
      ...mapActions(['updateSalesItems','dashboard_sort','updateBalancesItems']),
      formatBalancePts(pts){
         pts = Math.round(+pts).toLocaleString(`${global.lang}-${global.country}`);
         return `${pts} ${LanguageService.getValueByKey('dashboardPage_pts')}`
      },
      recalculate(){
         this.updateBalancesItems()
      },
      checkIsSession(prop){
         return prop === 'TutoringSession';
      },
      checkIsQuestion(prop){
         return prop === 'Question' && prop !== 'Answer';
      },
      checkIsItem(prop){
         return prop !== 'Question' && prop !== 'Answer' && prop !== 'TutoringSession';
      },
      formatItemStatus(paymentStatus){
         if(paymentStatus === 'Pending'){
            return LanguageService.getValueByKey('dashboardPage_pending')
         }
         if(paymentStatus === 'Paid'){
            return LanguageService.getValueByKey('dashboardPage_paid')
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
      calculatedEarnedPoints(){
         let typesDictionary = {};
            let earned = 0;
            this.balancesItems.forEach((item) => {
                      typesDictionary[item.type] = item.points;
                  });
            let reduce = typesDictionary["Stake"] + typesDictionary["Spent"];
            if(reduce < 0){
                earned = typesDictionary["Earned"] + reduce;
            }else{
                earned = typesDictionary["Earned"];
            }
            return earned;
        },
   },
   created() {
      this.updateSalesItems()
      this.updateBalancesItems()
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
        .mySales_cash-out-wrapper {
    text-align: center;
    padding: 0 8px;
   //  .responsive-property(margin-top, 22px, null, 16px);
   //  padding-bottom: 32px;
    .mySales_text-wrap {
      .responsive-property(margin-bottom, 32px, null, 16px);
      .responsive-property(font-size, 24px, null, 20px);
      color: grey;
      letter-spacing: -0.7px;
      text-align: center;
      .main-text {
        font-weight: bold;
      }
      .mySales_points-text span {
        font-weight: bold;
        color: @color-main-purple;
      }
    }
      .mySales_wallet_reedem{
         background: white;
      }

  }
      
   }
   .mySales_table{
      .v-datatable{
         tr{
            height: auto;
            th{
               color: #43425d !important;
               font-size: 14px;
               padding-top: 14px;
               padding-bottom: 14px;
            }
         }
         color: #43425d !important;
      }
      .sbf-arrow-right-carousel, .sbf-arrow-left-carousel {
         transform: none /*rtl:rotate(180deg)*/;
         color: #43425d !important;
         height: inherit;
         font-size: 14px;
      }

   }

}
</style>
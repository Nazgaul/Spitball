<template>
   <div class="mySales">
      <div class="mySales_title" v-language:inner="'dashboardPage_my_sales_title'"/>
      <v-layout ba row wrap class="mySales_wallet mb-5">
         <v-flex xs12 sm6>
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
                  <td>
                     <span>{{props.item.type}}</span>
                  </td>
                  <td>
                     <span>{{props.item.points}}</span>
                  </td> 
                  <td>
                     <span class="font-weight-bold">
                        {{ props.item.value | currencyFormat(props.item.symbol)}}
                     </span>
                  </td> 
               </template>
            </v-data-table>
         </v-flex>
         <v-flex xs12 sm6 >
              <cash-out-card class="mySales_wallet_reedem" v-for="(cashOutOption,index) in cashOutOptions"
                                       :key="index"
                                       :points-for-dollar="cashOutOption.pointsForDollar"
                                       :cost="cashOutOption.cost"
                                       :image="cashOutOption.image"
                                       :available="calculatedEarnedPoints >= cashOutOption.cost">
                        </cash-out-card>
            
         </v-flex>
      </v-layout>

         <v-data-table 
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
      }
   },
   data() {
      return {
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
.mySales{
   .mySales_title{
      font-size: 22px;
      color: #43425d;
      font-weight: 600;
      padding: 0 0 10px 2px;
   }
   .mySales_wallet{
      .mySales_wallet_reedem{
         background: white;
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
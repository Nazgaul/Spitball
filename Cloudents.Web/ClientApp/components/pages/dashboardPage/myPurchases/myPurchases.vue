<template>
   <div class="myPurchases">
      <div class="myPurchases_title" v-language:inner="'dashboardPage_my_purchases_title'"/>
      <v-data-table 
            :headers="headers"
            :items="purchasesItems"
            sort-by
            :item-key="'date'"
            :items-per-page="5"
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

            <template v-slot:item="props">
               <tr class="myPurchases_table_tr">
                  <tablePreviewTd :globalFunctions="globalFunctions" :item="props.item"/>
                  <tableInfoTd :globalFunctions="globalFunctions" :item="props.item"/>
                  <td class="text-left" v-html="dictionary.types[props.item.type]"/>
                  <td class="text-left" v-html="globalFunctions.formatPrice(props.item.price,props.item.type)"/>
                  <td class="text-left">{{ props.item.date | dateFromISO }}</td> 
                  
                  
                  <td class="text-center">
                     <button v-if="props.item.type !== 'TutoringSession' && props.item.type !== 'BuyPoints'" @click="dynamicAction(props.item)" class="myPurchases_action" v-language:inner="dynamicResx(props.item.type)"/>
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
      ...mapGetters(['getPurchasesItems']),
      purchasesItems(){
         return this.getPurchasesItems
      },
   },
   methods: {
      ...mapActions(['updatePurchasesItems','dashboard_sort']),
      dynamicResx(type){
         if(type === 'Document'){
            return 'dashboardPage_action_download'
         }else{
            return 'dashboardPage_action_watch'
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
   .myPurchases_title{
      font-size: 22px;
      color: #43425d;
      font-weight: 600;
      padding: 30px;
      line-height: 1.3px;
      background: #fff;
      box-shadow: 0 2px 1px -1px rgba(0,0,0,.2),0 1px 1px 0 rgba(0,0,0,.14),0 1px 3px 0 rgba(0,0,0,.12)!important;
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
         font-size: 13px !important;
         &:first-child {
            padding-right: 0;   
         }
         padding: 0 24px
      }
   }
   .myPurchases_action{
      outline: none;
      padding: 10px 0px;
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
         transform: none /*rtl:rotate(180deg)*/;
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
<template>
   <div class="myPurchases">
      <div class="myPurchases_title" v-language:inner="'dashboardPage_my_purchases_title'"/>
      <v-data-table 
            :pagination.sync="paginationModel"
            :headers="headers"
            :items="purchasesItems"
            disable-initial-sort
            :item-key="'date'"
            :rows-per-page-items="['5']"
            class="elevation-1 myPurchases_table"
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
               <td class="text-xs-left" v-html="globalFunctions.formatPrice(props.item.price,props.item.type)"/>
               <td class="text-xs-left">{{ props.item.date | dateFromISO }}</td> 
               <td class="text-xs-center">
                  <button v-if="props.item.type !== 'TutoringSession'" @click="dynamicAction(props.item)" class="myPurchases_action" v-language:inner="dynamicResx(props.item.type)"/>
               </td> 
            </template>
         <slot slot="no-data" name="tableEmptyState"/>
         <slot slot="pageText" name="tableFooter"/>
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
   .myPurchases_table{
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
      .sbf-arrow-right-carousel, .sbf-arrow-left-carousel {
         transform: none /*rtl:rotate(180deg)*/;
         color: #43425d !important;
         height: inherit;
         font-size: 14px;
      }

   }
}
</style>
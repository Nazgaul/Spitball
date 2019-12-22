<template>
 <div class="myStudyRooms">
      <div class="myStudyRooms_title" v-language:inner="'schoolBlock_my_study_rooms'"/>
      <v-data-table 
            :pagination.sync="paginationModel"
            :headers="headers"
            :items="studyRoomItems"
            disable-initial-sort
            :item-key="'date'"
            :rows-per-page-items="['5']"
            class="elevation-1 myStudyRooms_table"
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
               <td class="text-xs-left">{{ props.item.date | dateFromISO }}</td>
               <td class="text-xs-left">

               </td>
               <!-- 
               <td class="text-xs-left" v-html="dictionary.types[props.item.type]"/>
               <td class="text-xs-left" v-html="globalFunctions.formatPrice(props.item.price,props.item.type)"/>
               <td class="text-xs-center">
                  <button @click="dynamicAction(props.item)" class="myStudyRooms_action" v-language:inner="dynamicResx(props.item.type)"/>
               </td>  -->
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
   name:'myStudyRooms',
   components:{tablePreviewTd,tableInfoTd},
   data() {
      return {
         paginationModel:{
            page:1
         },
         sortedBy:'',
         headers:[
            this.dictionary.headers['preview'],
            this.dictionary.headers['student_tutor'],
            this.dictionary.headers['last_date'],
            this.dictionary.headers['action'],
         ],
      }
   },
   props:{
      globalFunctions: {
         type: Object,
      },
      dictionary:{
         type: Object,
         required: true
      }
   },
   computed: {
      ...mapGetters(['getStudyRoomItems']),
      studyRoomItems(){
         return this.getStudyRoomItems
      },
   },
   methods: {
      ...mapActions(['updateStudyRoomItems','dashboard_sort']),
      changeSort(sortBy){
         if(sortBy === 'info') return;

         let sortObj = {
            listName: 'studyRoomItems',
            sortBy,
            sortedBy: this.sortedBy
         }
         this.dashboard_sort(sortObj)
         this.paginationModel.page = 1;
         this.sortedBy = this.sortedBy === sortBy ? '' : sortBy;
      }
   },
   created() {
      this.updateStudyRoomItems()
   },
}
</script>

<style lang="less">
.myStudyRooms{
   .myStudyRooms_title{
      font-size: 22px;
      color: #43425d;
      font-weight: 600;
      padding: 0 0 10px 2px;
   }
   .myStudyRooms_table{
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
      .myStudyRooms_action{
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
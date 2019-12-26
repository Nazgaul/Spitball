<template>
   <div class="myContent">
      <div class="myContent_title" v-language:inner="'dashboardPage_my_content_title'"/>
      <v-data-table 
            :pagination.sync="paginationModel"
            :headers="headers"
            :items="contentItems"
            disable-initial-sort
            :item-key="'date'"
            :rows-per-page-items="['5']"
            class="elevation-1 myContent_table"
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
               <td class="text-xs-left">{{props.item.likes}}</td>
               <td class="text-xs-left">{{props.item.views}}</td>
               <td class="text-xs-left">{{props.item.downloads}}</td>
               <td class="text-xs-left">{{props.item.purchased}}</td>
               <td class="text-xs-left" v-html="globalFunctions.formatPrice(props.item.price,props.item.type)"/>
               <td class="text-xs-left">{{ props.item.date | dateFromISO }}</td>
               <td class="text-xs-center">
                  <v-menu lazy bottom left v-model="showMenu" v-if="!checkIsQuestion(props.item.type)" >
                     <v-icon @click="currentItemIndex = props.index" slot="activator" small icon>sbf-3-dot</v-icon>

                     <v-list v-if="props.index == currentItemIndex">
                        <v-list-tile style="cursor:pointer;" @click="globalFunctions.openDialog('rename',props.item)">{{rename}}</v-list-tile>
                        <v-list-tile style="cursor:pointer;" @click="globalFunctions.openDialog('changePrice',props.item)">{{changePrice}}</v-list-tile>
                     </v-list>
                  </v-menu>
               </td>
            </template>
         <slot slot="no-data" name="tableEmptyState"/>
         <slot slot="pageText" name="tableFooter"/>

      </v-data-table>
   </div>
</template>

<script>
import { mapActions, mapGetters } from 'vuex';
import { LanguageService } from '../../../../services/language/languageService';
import tablePreviewTd from '../global/tablePreviewTd.vue';
import tableInfoTd from '../global/tableInfoTd.vue';

export default {
   name:'myContent',
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
         currentItemIndex:'',
         showMenu:false,
         headers:[
            this.dictionary.headers['preview'],
            this.dictionary.headers['info'],
            this.dictionary.headers['type'],
            this.dictionary.headers['likes'],
            this.dictionary.headers['views'],
            this.dictionary.headers['downloads'],
            this.dictionary.headers['purchased'],
            this.dictionary.headers['price'],
            this.dictionary.headers['date'],
            this.dictionary.headers['action'],
         ],
         changePrice:LanguageService.getValueByKey("resultNote_change_price"),
         rename:LanguageService.getValueByKey("dashboardPage_rename"),
      }
   },
   computed: {
      ...mapGetters(['getContentItems']),
      contentItems(){
         return this.getContentItems
      },
   },
   methods: {
      ...mapActions(['updateContentItems','dashboard_sort']),
      checkIsQuestion(prop){
         return prop === 'Question' || prop === 'Answer';
      },
      changeSort(sortBy){
         if(sortBy === 'info') return;

         let sortObj = {
            listName: 'contentItems',
            sortBy,
            sortedBy: this.sortedBy
         }
         this.dashboard_sort(sortObj)
         this.paginationModel.page = 1;
         this.sortedBy = this.sortedBy === sortBy ? '' : sortBy;
      }
   },
   created() {
      this.updateContentItems()
   },
}
</script>

<style lang="less">
.myContent{
   .myContent_title{
      font-size: 22px;
      color: #43425d;
      font-weight: 600;
      padding: 30px;
      background-color: #fff;
      line-height: 1.3px;
   }
   .myContent_table{
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
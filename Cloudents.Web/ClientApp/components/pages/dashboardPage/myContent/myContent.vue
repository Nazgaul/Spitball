<template>
   <div class="myContent">
      <div class="myContent_title" v-language:inner="'dashboardPage_my_content_title'"/>
      <v-data-table 
            :headers="headers"
            :items="contentItems"
            :items-per-page="5"
            hide-default-header
            sort-by
            :item-key="'date'"
            class="elevation-1 myContent_table"
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
            </thead>
         </template>
            <template v-slot:item="props">
               <tr class="myContent_table_tr">
                  <tablePreviewTd :item="props.item"/>
                  <tableInfoTd :item="props.item"/>

                  <td class="text-xs-left" v-text="dictionary.types[props.item.type]"/>
                  <td class="text-xs-left">{{props.item.likes}}</td>
                  <td class="text-xs-left">{{props.item.views}}</td>
                  <td class="text-xs-left">{{props.item.downloads}}</td>
                  <td class="text-xs-left">{{props.item.purchased}}</td>
                  <td class="text-xs-left" v-text="formatPrice(props.item.price,props.item.type)"/>
                  <td class="text-xs-left">{{ $d(new Date(props.item.date)) }}</td>
                  <td class="text-xs-center">
                     <v-menu bottom left v-model="showMenu" v-if="!checkIsQuestion(props.item.type)">
                        <template v-slot:activator="{ on }">
                           <v-icon @click="currentItemIndex = props.index" v-on="on" slot="activator" small icon>sbf-3-dot</v-icon>
                        </template>
                     
                        <v-list v-if="props.index == currentItemIndex">
                           <v-list-item style="cursor:pointer;" @click="openChangeNameDialog(props.item)">{{rename}}</v-list-item>
                           <v-list-item style="cursor:pointer;" @click="openChangePriceDialog(props.item)">{{changePrice}}</v-list-item>
                        </v-list>
                     </v-menu>
                  </td>
               </tr>
            </template>

            <slot slot="no-data" name="tableEmptyState"/>
      </v-data-table>
      <sb-dialog 
         :showDialog="isChangeNameDialog"
         :isPersistent="true"
         :popUpType="'dashboardDialog'"
         :onclosefn="closeDialog"
         :activateOverlay="true"
         :max-width="'fit-content'"
         :content-class="'pop-dashboard-container'">
            <changeNameDialog :dialogData="currentItem" @closeDialog="closeDialog"/>
      </sb-dialog>
      <sb-dialog 
         :showDialog="isChangePriceDialog"
         :isPersistent="true"
         :popUpType="'dashboardDialog'"
         :onclosefn="closeDialog"
         :activateOverlay="true"
         :max-width="'fit-content'"
         :content-class="'pop-dashboard-container'">
            <changePriceDialog :dialogData="currentItem" @closeDialog="closeDialog"/>
      </sb-dialog>
   </div>
</template>

<script>
import { mapActions, mapGetters } from 'vuex';
import { LanguageService } from '../../../../services/language/languageService';
import tablePreviewTd from '../global/tablePreviewTd.vue';
import tableInfoTd from '../global/tableInfoTd.vue';
import sbDialog from '../../../wrappers/sb-dialog/sb-dialog.vue';
import changeNameDialog from '../dashboardDialog/changeNameDialog.vue';
import changePriceDialog from '../dashboardDialog/changePriceDialog.vue';

export default {
   name:'myContent',
   components:{tablePreviewTd,tableInfoTd,sbDialog,changeNameDialog,changePriceDialog},
   props:{
      dictionary:{
         type: Object,
         required: true
      }
   },
   data() {
      return {
         currentItem:'',
         isChangeNameDialog:false,
         isChangePriceDialog:false,
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
      ...mapGetters(['getContentItems','accountUser']),
      contentItems(){
         return this.getContentItems
      },
   },
   methods: {
      ...mapActions(['updateContentItems','dashboard_sort']),
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
      openChangeNameDialog(item){
         this.currentItem = item;
         this.isChangeNameDialog = true;
      },
      openChangePriceDialog(item){
         this.currentItem = item;
         this.isChangePriceDialog = true;
      },
      closeDialog(){
         this.isChangeNameDialog = false;
         this.isChangePriceDialog = false;
         this.currentItem = '';
      },
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
@import "../../../../styles/mixin.less";
.pop-dashboard-container {
   background: #fff;
}
.myContent{
   max-width: 1366px;
   .myContent_title{
      font-size: 22px;
      color: #43425d;
      font-weight: 600;
      padding: 30px;
      background-color: #fff;
      line-height: 1.3px;
      box-shadow: 0 2px 1px -1px rgba(0,0,0,.2),0 1px 1px 0 rgba(0,0,0,.14),0 1px 3px 0 rgba(0,0,0,.12)!important;
   }
   .myContent_table{
      thead{
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
      // .myContent_table_tr {
      //    @media (max-width: @screen-xs) {
      //       display: flex;
      //       flex-direction: column;
      //       border-bottom: 1px solid rgba(0, 0, 0, 0.12);
      //       padding-bottom: 10px;  
      //    }
         
      //    td {
      //       font-size: 13px !important;
      //       @media (max-width: @screen-xs) {
      //          height: unset;
      //          border-bottom: none !important;    
                        
      //       }
      //    }
      // }
      .sbf-arrow-right-carousel, .sbf-arrow-left-carousel {
         transform: none /*rtl:rotate(180deg)*/;
         color: #43425d !important;
         height: inherit;
         font-size: 14px;
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
<template>
   <div class="myContent">
      <v-data-table 
         calculate-widths
         :page.sync="paginationModel.page"
         :headers="headers"
         :items="contentItems"
         :items-per-page="20"
         sort-by
         :item-key="'date'"
         class="elevation-1"
         :footer-props="{
            showFirstLastPage: false,
            firstIcon: '',
            lastIcon: '',
            prevIcon: 'sbf-arrow-left-carousel',
            nextIcon: 'sbf-arrow-right-carousel',
            itemsPerPageOptions: [20]
         }">
         <template v-slot:top >
            <div class="myContent_title">
                  {{$t('dashboardPage_my_content_title')}}
                  </div>
         </template>
         <template v-slot:item.preview="{item}">
            <router-link class="d-flex justify-center" :to="dynamicRouter(item)">
               <v-avatar>
                  <img :src="item.preview? item.preview : require('../global/images/qs.png')">
               </v-avatar>
            </router-link>
         </template>
         <template v-slot:item.info="{item}">
            <tableInfoTd :item="item"/>
         </template>
         <template v-slot:item.type="{item}">
            {{dictionary.types[item.type]}}
         </template>
         <template v-slot:item.price="{item}">
            {{ formatPrice(item.price,item.type) }}
         </template>
         <template v-slot:item.date="{item}">
            {{ $d(new Date(item.date)) }}
         </template>
         <template v-slot:item.action="{item}">
            <v-menu bottom left v-model="showMenu" v-if="!checkIsQuestion(item.type)">
               <template v-slot:activator="{ on }">
                  <v-icon @click="currentItemIndex = item" v-on="on" slot="activator" small icon>sbf-3-dot</v-icon>
               </template>
               <v-list v-if="item == currentItemIndex">
                  <v-list-item style="cursor:pointer;" @click="openChangeNameDialog(item)">{{rename}}</v-list-item>
                  <v-list-item style="cursor:pointer;" @click="openChangePriceDialog(item)">{{changePrice}}</v-list-item>
               </v-list>
            </v-menu>
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
import tableInfoTd from '../global/tableInfoTd.vue';
import sbDialog from '../../../wrappers/sb-dialog/sb-dialog.vue';
import changeNameDialog from '../dashboardDialog/changeNameDialog.vue';
import changePriceDialog from '../dashboardDialog/changePriceDialog.vue';

export default {
   name:'myContent',
   components:{tableInfoTd,sbDialog,changeNameDialog,changePriceDialog},
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
      ...mapActions(['updateContentItems']),
      dynamicRouter(item){
         if(item.url){
            return item.url;
         }
         if(item.type === 'Question' || item.type === 'Answer'){
            return {path:'/question/'+item.id}
         }
      },
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
      // box-shadow: 0 2px 1px -1px rgba(0,0,0,.2),0 1px 1px 0 rgba(0,0,0,.14),0 1px 3px 0 rgba(0,0,0,.12)!important;
   }
   td:first-child {
      width:1%;
      white-space: nowrap;
   }
   tr:nth-of-type(2n) {
      td {
         background-color: #f5f5f5;
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
</style>
<template>
   <div class="dashboardPage">
      <component :dictionary="dictionary" :globalFunctions="globalFunctions" :is="component">
         <template slot="tableFooter">
            <tableFooter/>
         </template>
         <template slot="tableEmptyState">
            <tableEmptyState/>
         </template>
      </component>
      <sb-dialog 
         :showDialog="isDialog"
         :isPersistent="true"
         :popUpType="'dashboardDialog'"
         :onclosefn="closeDialog"
         :activateOverlay="true"
         :max-width="'438px'"
         :content-class="'pop-dashboard-container'">
            <changeNameDialog v-if="currentDialog === 'rename'" :dialogData="dialogData" @closeDialog="closeDialog"/>
            <changePriceDialog v-if="currentDialog === 'changePrice'" :dialogData="dialogData" @closeDialog="closeDialog"/>
      </sb-dialog>
   </div>
</template>

<script>
import mySales from './mySales/mySales.vue';
import myContent from './myContent/myContent.vue';
import myPurchases from './myPurchases/myPurchases.vue';
import myStudyRooms from './myStudyRooms/myStudyRooms.vue';

import tableEmptyState from './global/tableEmptyState.vue';
import tableFooter from './global/tableFooter.vue';

import sbDialog from '../../wrappers/sb-dialog/sb-dialog.vue';
import changeNameDialog from './dashboardDialog/changeNameDialog.vue';
import changePriceDialog from './dashboardDialog/changePriceDialog.vue';
import { LanguageService } from '../../../services/language/languageService';
import { mapGetters } from 'vuex';

export default {
   name:'dashboardPage',
   props:['component'],
   data() {
      return {
         currentDialog:'',
         isDialog:false,
         dialogData:'',
         dictionary:{
            types:{
               'Question': LanguageService.getValueByKey('dashboardPage_qa'),
               'Answer': LanguageService.getValueByKey('dashboardPage_qa'),
               'Document': LanguageService.getValueByKey('dashboardPage_document'),
               'Video': LanguageService.getValueByKey('dashboardPage_video'),
               'TutoringSession': LanguageService.getValueByKey('dashboardPage_tutor_session'),
               'Earned': LanguageService.getValueByKey('wallet_earned'),
               'Spent': LanguageService.getValueByKey('wallet_spent'),
               'Total': LanguageService.getValueByKey('wallet_total'),
            },
            headers:{
               'preview': {text: '', align:'left', sortable: false, value:'preview'},
               'info': {text: LanguageService.getValueByKey('dashboardPage_info'), align:'left', sortable: false, value:'info'},
               'type': {text: LanguageService.getValueByKey('dashboardPage_type'), align:'left', sortable: true, value:'type'},
               'likes': {text:LanguageService.getValueByKey('dashboardPage_likes'), align:'left', sortable: true, value:'likes'},
               'views': {text:LanguageService.getValueByKey('dashboardPage_views'), align:'left', sortable: true, value:'views'},
               'downloads': {text:LanguageService.getValueByKey('dashboardPage_downloads'), align:'left', sortable: true, value:'downloads'},
               'purchased': {text:LanguageService.getValueByKey('dashboardPage_purchased'), align:'left', sortable: true, value:'purchased'},
               'price': {text:LanguageService.getValueByKey('dashboardPage_price'), align:'left', sortable: true, value:'price'},
               'date': {text: LanguageService.getValueByKey('dashboardPage_date'), align:'left', sortable: true, value:'date'},
               'action': {text: '', align:'center', sortable: false, value:'action'},
               'status': {text: LanguageService.getValueByKey('dashboardPage_status'), align:'left', sortable: true, value:'paymentStatus'},
               'points': {text: LanguageService.getValueByKey('wallet_Tokens'), align:'center', sortable: true, value:'points'},
               'value': {text: LanguageService.getValueByKey('wallet_Value'), align:'center', sortable: true, value:'value'},
               'student_tutor': {text: LanguageService.getValueByKey('dashboardPage_student_tutor'), align:'left', sortable: true, value:'name'},
               'created': {text: LanguageService.getValueByKey('studyRoom_created'), align:'left', sortable: true, value:'date'},
               'last_date': {text: LanguageService.getValueByKey('dashboardPage_last_date'), align:'left', sortable: true, value:'lastSession'},
            }
         },
         globalFunctions:{
            openDialog: this.openDialog,
            formatImg: this.formatImg,
            formatPrice: this.formatPrice,
            router: this.dynamicRouter,
            '$Ph': this.$Ph,
         }
      }
   },
   components:{
      mySales,
      myContent,
      myPurchases,
      myStudyRooms,

      changeNameDialog,
      changePriceDialog,
      sbDialog,
      tableEmptyState,
      tableFooter
   },
   computed: {
      ...mapGetters(['accountUser'])
   },
   methods: {
      closeDialog() {
         this.currentDialog = '';
         this.dialogData = '';
         this.isDialog = false;
      },
      openDialog(dialogName,itemData){
         this.currentDialog = dialogName;
         this.dialogData = itemData;
         this.isDialog = true;
      },
      dynamicRouter(item){
         if(item.url){
            return item.url;
         }
         if(item.type === 'Question' || item.type === 'Answer'){
            return {path:'/question/'+item.id}
         }
         if(item.type === 'TutoringSession'){
            return {name: 'profile',params: {id: item.id, name: item.name}}
         }
         if(item.conversationId){
            return {name: 'profile',params: {id: item.userId, name: item.name}}
         }
      },
      formatImg(item){
         if(item.preview){
            return this.$proccessImageUrl(item.preview,80,80)
         }
         if(item.image){
            return this.$proccessImageUrl(item.image,80,80)
         }
         if(item.type === 'Question' || item.type === 'Answer'){
            return require(`./images/qs.png`) 
         }
      },      
      formatPrice(price,type){
         if(price < 0){
            price = Math.abs(price)
         }
         price = Math.round(+price).toLocaleString();
         if(type === 'Document' || type === 'Video'){
            return `${price} ${LanguageService.getValueByKey('dashboardPage_pts')}`
         }
         if(type === 'TutoringSession'){
            return `${price} ${this.accountUser.currencySymbol}`
         }
      },
   }

}
</script>

<style lang="less">
@import '../../../styles/mixin.less';
.dashboardPage{
   padding: 30px;
	@media (max-width: @screen-xs) {
      padding-left: 6px;
      padding-right: 6px;
      width: 100%;
      height: 100%;
   }
}
.pop-dashboard-container {
   background: #fff;
}

</style>

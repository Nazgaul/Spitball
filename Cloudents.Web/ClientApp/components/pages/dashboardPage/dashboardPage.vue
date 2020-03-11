<template>
   <div class="dashboardPage">
      <component v-if="!!accountUser" :dictionary="dictionary" :is="component">
         <template slot="tableEmptyState">
            <tableEmptyState/>
         </template>
      </component>
   </div>
</template>

<script>
import mySales from './mySales/mySales.vue';
import myContent from './myContent/myContent.vue';
import myPurchases from './myPurchases/myPurchases.vue';
import myStudyRooms from './myStudyRooms/myStudyRooms.vue';
import myCalendar from './myCalendar/myCalendar.vue';
import myFollowers from './myFollowers/myFollowers.vue';

import tableEmptyState from './global/tableEmptyState.vue';

import { LanguageService } from '../../../services/language/languageService';
import { mapGetters } from 'vuex';

export default {
   name:'dashboardPage',
   props:['component'],
   data() {
      return {
         dictionary:{
            types:{
               'Question': LanguageService.getValueByKey('dashboardPage_qa'),
               'Answer': LanguageService.getValueByKey('dashboardPage_qa'),
               'Document': LanguageService.getValueByKey('dashboardPage_document'),
               'Video': LanguageService.getValueByKey('dashboardPage_video'),
               'TutoringSession': LanguageService.getValueByKey('dashboardPage_tutor_session'),
               'BuyPoints': LanguageService.getValueByKey('dashboardPage_type_buy_points')
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
               'joined': {text: LanguageService.getValueByKey('dashboardPage_joined'), align:'left', sortable: true, value:'date'},
               'name': {text: LanguageService.getValueByKey('dashboardPage_name'), align:'left', sortable: true, value:'name'},
            }
         },
      }
   },
   components:{
      mySales,
      myContent,
      myPurchases,
      myStudyRooms,
      myCalendar,
      myFollowers,
      tableEmptyState,
   },
   computed: {
      ...mapGetters(['accountUser'])
      
   },
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
   .v-snack__content{
      justify-content: center;
   }
}

</style>

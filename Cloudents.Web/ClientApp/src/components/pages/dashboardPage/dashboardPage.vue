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
import { mapGetters } from 'vuex';

const mySales = () => import('./mySales/mySales.vue');
const myCourses = () => import('./myContent/myContent.vue');
const myPurchases = () => import('./myPurchases/myPurchases.vue');
const myStudyRooms = () => import('./myStudyRooms/myStudyRooms.vue');
const myStudyRoomsBroadcast = () => import('./myStudyRooms/myStudyRooms.vue');
const myCalendar = () => import('./myCalendar/myCalendar.vue');
const myFollowers = () => import('./myFollowers/myFollowers.vue');
const tableEmptyState = () => import('./global/tableEmptyState.vue');

export default {
   name:'dashboardPage',
   props:['component'],
   data() {
      return {
         dictionary:{
            types:{
               //'Question': this.$t('dashboardPage_qa'),
               //'Answer': this.$t('dashboardPage_qa'),
               'Document': this.$t('dashboardPage_document'),
               'Video': this.$t('dashboardPage_video'),
               'TutoringSession': this.$t('dashboardPage_tutor_session'),
               'BuyPoints': this.$t('dashboardPage_type_buy_points')
            },
            headers:{
               'preview': {text: '', align:'', sortable: false, value:'preview'},
               'info': {text: this.$t('dashboardPage_info'), align:'', sortable: false, value:'info'},
               'type': {text: this.$t('dashboardPage_type'), align:'', sortable: true, value:'type'},
               'likes': {text:this.$t('dashboardPage_likes'), align:'', sortable: true, value:'likes'},
               'views': {text:this.$t('dashboardPage_views'), align:'', sortable: true, value:'views'},
               'downloads': {text:this.$t('dashboardPage_downloads'), align:'', sortable: true, value:'downloads'},
               'purchased': {text:this.$t('dashboardPage_purchased'), align:'', sortable: true, value:'purchased'},
               'price': {text:this.$t('dashboardPage_price'), align:'', sortable: true, value:'price'},
               'date': {text: this.$t('dashboardPage_date'), align:'', sortable: true, value:'date'},
               'action': {text: '', align:'center', sortable: false, value:'action'},
               'status': {text: this.$t('dashboardPage_status'), align:'', sortable: true, value:'paymentStatus'},
               'points': {text: this.$t('wallet_Tokens'), align:'center', sortable: true, value:'points'},
               'value': {text: this.$t('wallet_Value'), align:'center', sortable: true, value:'value'},
               'student_tutor': {text: this.$t('dashboardPage_student_tutor'), align:'', sortable: true, value:'name'},
               'created': {text: this.$t('studyRoom_created'), align:'', sortable: true, value:'date'},
               'last_date': {text: this.$t('dashboardPage_last_date'), align:'', sortable: true, value:'lastSession'},
               'joined': {text: this.$t('dashboardPage_joined'), align:'', sortable: true, value:'date'},
               'name': {text: this.$t('dashboardPage_name'), align:'', sortable: true, value:'name'},
               'students': {text: this.$t('dashboardPage_students'), align:'', sortable: true, value:'students'},
               'scheduled': {text: this.$t('dashboardPage_scheduled'), align:'', sortable: true, value:'scheduled'},
            }
         },
      }
   },
   components:{
      mySales,
      myCourses,
      myPurchases,
      myStudyRooms,
      myStudyRoomsBroadcast,
      myCalendar,
      myFollowers,
      tableEmptyState,
   },
   computed: {
      ...mapGetters(['accountUser'])
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
   .v-snack__content{
      justify-content: center;
   }
}

</style>

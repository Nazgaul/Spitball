<template>
   <div class="mySales">
      <div class="pa-4">
         <div class="mySales_title" v-t="'financial_status'"></div>
         <!-- <v-layout wrap class="mySales_wallet mb-2" v-if="!!accountUser && accountUser.id"> -->
            <!-- <v-flex sm12 md6 :class="[{'mt-1':$vuetify.breakpoint.xsOnly},{'mt-3':$vuetify.breakpoint.smAndDown && !$vuetify.breakpoint.xsOnly}]"> -->
               <!-- <div class="mySales_actions"> -->
                  <!-- <redeemPointsLayout class="my-2 my-md-0 me-lg-2 "/> -->
                  <!-- <billOfflineLayout class="my-2 my-md-0"/> -->
               <!-- </div> -->
            <!-- </v-flex> -->
         <!-- </v-layout> -->
      </div>

      <v-data-table 
         :loading="!isReady"
         calculate-widths
         :page.sync="paginationModel.page"
         :headers="headers"
         :mobile-breakpoint="0"
         :items="salesItems"
         :items-per-page="20"
         sort-by
         :item-key="'sessionId'"
         class="mySales_table-full"
         :footer-props="{
            showFirstLastPage: false,
            firstIcon: '',
            lastIcon: '',
            prevIcon: 'sbf-arrow-left-carousel',
            nextIcon: 'sbf-arrow-right-carousel',
            itemsPerPageOptions: [5]
         }">
            <template v-slot:item.preview="{item}">
               <div class="d-flex justify-center" v-if="item.preview">
                  <v-avatar size="68" :class="{'cursor-pointer':item.type == 'Course'}"  @click="item.type == 'Course'? goToCourse(item):''">
                     <img :src="$proccessImageUrl(item.preview, {width:68, height:68}) ">
                  </v-avatar>
               </div>
               <div v-if="item.sessionId">
                  <userAvatarNew
                     class="mySalesUserAvatar"
                     :userImageUrl="item.image"
                     :user-name="item.name"
                     :width="68"
                     :height="68"
                     :fontSize="14"
                  />
               </div>
            </template>
            <template v-slot:item.info="{item}">
               <tableInfoTd :item="item"/>
            </template>
            <template v-slot:item.type="{item}">
               {{dictionary.types[item.type]}}
            </template>
            <template v-slot:item.paymentStatus="{item}">
               {{formatItemStatus(item.paymentStatus)}}
            </template>
            <template v-slot:item.date="{item}">
               {{$moment(item.date).format('MMM D')}}
            </template>
            <template v-slot:item.price="{item}">
               {{formatPrice(item.price,item.type)}}
            </template>
            <template v-slot:item.action="{item}">
               <v-btn 
                  color="#02C8BF"
                  class="white--text"
                  width="100"
                  depressed
                  rounded
                  v-if="item.paymentStatus === 'PendingTutor' && item.type === 'TutoringSession' && item.price > 0"
                  @click="$openDialog('teacherApproval', {item: item})">
                     {{$t('dashboardPage_btn_approve')}}
               </v-btn>
            </template>
            <slot slot="no-data" name="tableEmptyState"/>
         </v-data-table>
   </div>
</template>

<script>
import { mapActions, mapGetters } from 'vuex';
import * as routeNames from '../../../../routes/routeNames.js';
import tableInfoTd from '../global/tableInfoTd.vue';
// import billOfflineLayout from './buyPointsLayout/billOfflineLayout.vue'
// import redeemPointsLayout from './redeemPointsLayout/redeemPointsLayout.vue'

export default {
   name:'mySales',
   components:{tableInfoTd},
   props:{
      dictionary:{
         type: Object,
         required: true
      },
   },
   data() {
      return {
         isReady:false,
         paginationModel:{
            page:1
         },
         headers:[
            this.dictionary.headers['preview'],
            this.dictionary.headers['info'],
            this.dictionary.headers['type'],
            this.dictionary.headers['status'],
            this.dictionary.headers['date'],
            this.dictionary.headers['price'],
            this.dictionary.headers['action'],
         ]
      }
   },
   computed: {
      ...mapGetters(['getSalesItems','accountUser']),
      salesItems(){
         return this.getSalesItems;
      },
      // pendingPayments() {
      //    return this.$store.getters.getPendingPayment
      // }
   },
   methods: {
      ...mapActions(['updateSalesItems']),
      goToCourse(item){
         this.$router.push({
            name: routeNames.CoursePage,
            params: {
               id:item.id,
               name:item.name
            }
         })
      },
      formatPrice(price,type){
         if(isNaN(price)) return;
         if(price < 0){
            price = Math.abs(price)
         }
         if(type === 'Document' || type === 'Video' ){
            price = Math.round(+price).toLocaleString();
            return `${price} ${this.$t('dashboardPage_pts')}`
         }
         if(type === 'TutoringSession' || type === 'BuyPoints' || type === 'Course'){
            // TODO: Currency Change
            return this.$n(price, {'style':'currency','currency': this.accountUser.currencySymbol, minimumFractionDigits: 0, maximumFractionDigits: 0})
         }
      },
      formatItemStatus(paymentStatus){
         if(paymentStatus === 'Approved'){
            return this.$t('dashboardPage_paid')
         }
         if(paymentStatus === 'PendingSystem'){
            return this.$t('dashboardPage_pending_approve')
         }
         if(paymentStatus === 'PendingTutor') {
            return this.$t('dashboardPage_pending')
         }
      },
   },
   created() {
      let self = this;
      this.updateSalesItems().finally(()=>{
         self.isReady = true
      })
   }
}
</script>

<style lang="less">
@import '../../../../styles/mixin.less';
.mySales{
   background: #fff;
   box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.15);
   // max-width: 1334px; 
   .mySales_title{
      font-size: 22px;
      color: #43425d;
      font-weight: 600;
      padding: 0 0 10px 2px;
   }
   .mySales_wallet{
      .mySales_actions{
         display: flex;
         @media (max-width: @screen-md-plus) {
            flex-wrap: wrap;
            height: 100%;
            // justify-content: space-between;
            justify-content: space-evenly;
            align-content: space-between;
         }

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
               border-top: thin solid rgba(0, 0, 0, 0.12);
            }
            
         }
         color: #43425d !important;
      } 
   // .mySales_table{
   //    max-width: 1334px; 
   // }
   .mySales_table-full{
   tr{
      height:54px;
   }
   td{
      border: none !important;
   }
      // max-width: 1334px;
      td:first-child {
         width:1%;
         white-space: nowrap;
      }
      tr:nth-of-type(2n) {
         td {
            background-color: #f5f5f5;
         }
      }
      .sbf-arrow-right-carousel, .sbf-arrow-left-carousel {
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
   .mySalesUserAvatar {
      .user-avatar-image-wrap {
         margin: 0 auto;

         .v-lazy {
            display: flex;
         }
      }
   }

}
</style>
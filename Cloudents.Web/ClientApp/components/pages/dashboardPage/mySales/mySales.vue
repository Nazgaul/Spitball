<template>
   <div class="mySales">
      <div class="mySales_title" v-language:inner="'dashboardPage_my_sales_title'"/>
         <v-data-table v-if="salesItems.length"
            :headers="headers"
            :items="salesItems"
            :items-per-page="5"
            :footer-props="{
               showFirstLastPage: false,
               firstIcon: '',
               lastIcon: '',
               prevIcon: 'sbf-arrow-left-carousel',
               nextIcon: 'sbf-arrow-right-carousel'
            }"
            sort-by
            :item-key="'date'"
            class="elevation-1 mySales_table">
            <template v-slot:body="props">
               <tbody>
                  <tr v-for="(item, index) in props.items" :key="index">
                     <td class="mySales_td mySales_td_img d-block d-sm-table-cell">
                        <router-link :to="dynamicRouter(item)" class="mySales_td_img_img">
                           <img width="80" height="80" :src="formatItemImg(item)" :class="{'imgPreview_sales':item.preview}">
                        </router-link>
                     </td>
                     <td class="mySales_td text-xs-left mySales_td_course d-block d-sm-table-cell">
                        <router-link :to="dynamicRouter(item)">
                           <template v-if="checkIsSession(item.type)">
                              <span v-html="$Ph('dashboardPage_session',item.studentName)"/>
                              <p><span v-language:inner="'dashboardPage_duration'"/> {{item.duration | sessionDuration}}</p>
                           </template>

                           <template v-if="checkIsQuestuin(item.type)">
                              <div class="text-truncate">
                                 <span v-language:inner="'dashboardPage_questuin'"/>
                                 <span class="text-truncate">{{item.text}}</span>
                              </div>
                              <div class="text-truncate">
                                 <span v-language:inner="'dashboardPage_answer'"/>
                                 <span>{{item.answerText}}</span>
                              </div>
                              <div>
                                 <span v-language:inner="'dashboardPage_course'"></span>
                                 <span>{{item.course}}</span>
                              </div>
                           </template>

                           <template v-if="checkIsItem(item.type)">
                              <span>{{item.name}}</span>
                              <div>
                                 <span v-language:inner="'dashboardPage_course'"></span>
                                 <span>{{item.course}}</span>
                              </div>
                           </template>
                        </router-link>
                     </td>
                     <td class="mySales_td text-xs-left d-block d-sm-table-cell">{{formatItemType(item.type)}}</td>
                     <td class="mySales_td text-xs-left d-block d-sm-table-cell" v-html="formatItemStatus(item.paymentStatus)"/>
                     <td class="mySales_td text-xs-left d-block d-sm-table-cell">{{ item.date | dateFromISO }}</td>
                     <td class="mySales_td text-xs-left d-block d-sm-table-cell">{{ formatItemPrice(item.price,item.type) }}</td>
                     <!-- <td class="text-xs-left"><v-icon @click="openDialog" small>sbf-3-dot</v-icon></td> -->
                  </tr>
               </tbody>
            </template>
            <template slot="pageText" slot-scope="item">
               <span class="mySales_footer">
               {{item.pageStart}} <span v-language:inner="'dashboardPage_of'"/> {{item.itemsLength}}
               </span>
            </template>
         </v-data-table>
   </div>
</template>

<script>
import { mapActions, mapGetters } from 'vuex';

import { LanguageService } from '../../../../services/language/languageService';

export default {
   name:'mySales',
   data() {
      return {
         headers:[
            {text: '', align:'left', sortable: false, value:'preview'},
            {text: LanguageService.getValueByKey('dashboardPage_info'), align:'left', sortable: false, value:'info'},
            {text: LanguageService.getValueByKey('dashboardPage_type'), align:'left', sortable: true, value:'type'},
            {text: LanguageService.getValueByKey('dashboardPage_status'), align:'left', sortable: true, value:'status'},
            {text: LanguageService.getValueByKey('dashboardPage_date'), align:'left', sortable: true, value:'date'},
            {text: LanguageService.getValueByKey('dashboardPage_price'), align:'left', sortable: true, value:'price'},
         ],
      }
   },
   computed: {
      ...mapGetters(['getSalesItems','accountUser']),
      salesItems(){
         return this.getSalesItems;
      },
   },
   methods: {
      ...mapActions(['updateSalesItems']),

      checkIsSession(prop){
         return prop === 'TutoringSession';
      },
      checkIsQuestuin(prop){
         return prop === 'Question';
      },
      checkIsItem(prop){
         return prop !== 'Question' && prop !== 'TutoringSession';
      },
      formatItemPrice(price,type){
         if(type === 'TutoringSession'){
            return `${Math.round(+price)} ${this.accountUser.currencySymbol}`
         }else{
            return `${Math.round(+price)} ${LanguageService.getValueByKey('dashboardPage_pts')}`
         }
      },
      formatItemStatus(paymentStatus){
         if(paymentStatus === 'Pending'){
            return LanguageService.getValueByKey('dashboardPage_pending')
         }
         if(paymentStatus === 'Paid'){
            return LanguageService.getValueByKey('dashboardPage_paid')
         }
      },
      dynamicRouter(item){
         if(item.url){
            return item.url;
         }
         if(item.studentId){
            return {name: 'profile',params: {id: item.studentId, name: item.studentName}}
         }
         if(item.type === 'Question'){
            return {path:'/question/'+item.id}
         }
      },
      formatItemType(type){
         if(type === 'Question'){
            return LanguageService.getValueByKey('dashboardPage_qa')
         }
         if(type === 'Document'){
            return LanguageService.getValueByKey('dashboardPage_document')
         }
         if(type === 'Video'){
            return LanguageService.getValueByKey('dashboardPage_video')
         }
         if(type === 'TutoringSession'){
            return LanguageService.getValueByKey('dashboardPage_tutor_session')
         }
      },
      formatItemImg(item){
         if(item.preview){
            return this.$proccessImageUrl(item.preview,140,140,"crop&anchorPosition=top")
         }
         if(item.studentImage){
            return this.$proccessImageUrl(item.studentImage,80,80)
         }
         if(item.type === 'Question'){
            return require(`../images/qs.png`) 
         }
      }
   },
   created() {
      this.updateSalesItems()
   },
}
</script>

<style lang="less">
@import '../../../../styles/mixin';

.mySales{
   .mySales_title{
      font-size: 22px;
      color: #43425d;
      font-weight: 600;
      padding: 0 0 10px 2px;
   }
   .mySales_table{
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
      .mySales_footer{
         font-size: 14px;
         color: #43425d !important;
      }
      .mySales_td {
         height: auto;
      }
      .mySales_td_img{
         line-height: 0;
         padding-right: 0 !important;
         .mySales_td_img_img{
            img{
               margin: 10px 0;
               &.imgPreview_sales{
                  object-fit: none;
                  object-position: top;
               }
            }

         }
      }
      .mySales_td_course {
         a{
            color: #43425d !important;
         }
         width: 450px;
         max-width: 450px;
         min-width: 300px;
         @media (max-width: @screen-xs) {
            width: auto;
         }
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
<template>
   <div class="mySales">
      <div class="mySales_title" v-language:inner="'dashboardPage_my_sales_title'"/>
         <v-data-table v-if="salesItems.length"
            :headers="headers"
            :items="salesItems"
            disable-initial-sort
            :item-key="'date'"
            :rows-per-page-items="['5']"
            class="elevation-1 mySales_table"
            :prev-icon="'sbf-arrow-left-carousel'"
            :sort-icon="'sbf-arrow-down'"
            :next-icon="'sbf-arrow-right-carousel'">
            <template v-slot:items="props">
               <td class="mySales_td_img">
                  <router-link :to="globalFunctions.router(props.item)" class="mySales_td_img_img">
                     <img width="80" height="80" :src="globalFunctions.formatImg(props.item)" :class="{'imgPreview_sales':props.item.preview}">
                  </router-link>
               </td>
               <td class="text-xs-left mySales_td_course">
                  <router-link :to="globalFunctions.router(props.item)">
                     <template v-if="checkIsSession(props.item.type)">
                        <span v-html="$Ph('dashboardPage_session',props.item.studentName)"/>
                        <p><span v-language:inner="'dashboardPage_duration'"/> {{props.item.duration | sessionDuration}}</p>
                     </template>

                     <template v-if="checkIsQuestion(props.item.type)">
                        <div class="text-truncate">
                           <span v-language:inner="'dashboardPage_question'"/>
                           <span class="text-truncate">{{props.item.text}}</span>
                        </div>
                        <div class="text-truncate">
                           <span v-language:inner="'dashboardPage_answer'"/>
                           <span>{{props.item.answerText}}</span>
                        </div>
                        <div v-if="props.item.course">
                           <span v-language:inner="'dashboardPage_course'"></span>
                           <span>{{props.item.course}}</span>
                        </div>
                     </template>

                     <template v-if="checkIsItem(props.item.type)">
                        <span>{{props.item.name}}</span>
                        <div v-if="props.item.course">
                           <span v-language:inner="'dashboardPage_course'"></span>
                           <span>{{props.item.course}}</span>
                        </div>
                     </template>
                  </router-link>
               </td>
               <td class="text-xs-left" v-html="dictionary.types[props.item.type]"/>
               <td class="text-xs-left" v-html="formatItemStatus(props.item.paymentStatus)"/>
               <td class="text-xs-left">{{ props.item.date | dateFromISO }}</td>
               <td class="text-xs-left">{{ formatItemPrice(props.item.price,props.item.type) }}</td>
               <!-- <td class="text-xs-left"><v-icon @click="openDialog" small>sbf-3-dot</v-icon></td> -->
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
         headers:[
            this.dictionary.headers['preview'],
            this.dictionary.headers['info'],
            this.dictionary.headers['type'],
            this.dictionary.headers['status'],
            this.dictionary.headers['date'],
            this.dictionary.headers['price'],
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
      checkIsQuestion(prop){
         return prop === 'Question' && prop !== 'Answer';
      },
      checkIsItem(prop){
         return prop !== 'Question' && prop !== 'TutoringSession' && prop !== 'Answer';
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
   },
   created() {
      this.updateSalesItems()
   },
}
</script>

<style lang="less">
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
<template>
      <div class="mySales">
            <v-data-table v-if="salesItems.length"
               border="1"
               :headers="headers"
               :items="salesItems"
               disable-initial-sort
               :item-key="'date'"
               :rows-per-page-items="['5']"
               class="elevation-1"
               :prev-icon="'sbf-arrow-left-carousel'"
               :sort-icon="'sbf-arrow-down'"
               :next-icon="'sbf-arrow-right-carousel'">
               <template v-slot:items="props">
                  <td class="mySales_td_img">
                     <img v-if="props.item.preview || props.item.studentImage" :src="$proccessImageUrl(props.item.preview? props.item.preview: props.item.studentImage,80,80)">
                     <v-icon v-else>sbf-user</v-icon>
                  </td>
                  <td class="text-xs-left mySales_td_course">
                     <template v-if="checkIsSession(props.item.type)">
                        <span v-language:inner="'dashboardPage_session'"></span>
                        <span>{{props.item.studentName}}</span>
                        <span>{{props.item.duration | sessionDuration}}</span>
                     </template>

                     <template v-if="checkIsQuestuin(props.item.type)">
                        <div class="content_txt text-truncate">
                           <span v-language:inner="'dashboardPage_questuin'"/>
                           <span class="text-truncate">{{props.item.text}}</span>
                        </div>
                        <div class="content_txt text-truncate">
                           <span v-language:inner="'dashboardPage_answer'"/>
                           <span>{{props.item.answerText}}</span>
                        </div>
                        <div>
                           <span v-language:inner="'dashboardPage_course'"></span>
                           <span>{{props.item.course}}</span>
                        </div>
                     </template>

                     <template v-if="checkIsItem(props.item.type)">
                        <span>{{props.item.name}}</span>
                        <div>
                           <span v-language:inner="'dashboardPage_course'"></span>
                           <span>{{props.item.course}}</span>
                        </div>
                     </template>
                  </td>
                  <td class="text-xs-left">{{ props.item.type }}</td>
                  <td class="text-xs-left" v-language:inner="`dashboardPage_${props.item.status.toLowerCase()}`"></td>
                  <td class="text-xs-left">{{ props.item.date | dateFromISO }}</td>
                  <td class="text-xs-left">{{ props.item.price}}</td>
                  <!-- <td class="text-xs-left"><v-icon @click="openDialog" small>sbf-3-dot</v-icon></td> -->
               </template>
               <template slot="pageText" slot-scope="item">
                  {{item.pageStart}} <span v-language:inner="'dashboardPage_of'"/> {{item.itemsLength}}
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
            {text: LanguageService.getValueByKey('dashboardPage_preview'), align:'left', sortable: false, value:'preview'},
            {text: LanguageService.getValueByKey('dashboardPage_info'), align:'left', sortable: false, value:'info'},
            {text: LanguageService.getValueByKey('dashboardPage_type'), align:'left', sortable: true, value:'type'},
            // {text:LanguageService.getValueByKey('dashboardPage_likes'), align:'left', sortable: true, value:'likes'},
            // {text:LanguageService.getValueByKey('dashboardPage_views'), align:'left', sortable: true, value:'views'},
            // {text:LanguageService.getValueByKey('dashboardPage_downloads'), align:'left', sortable: true, value:'downloads'},
            // {text:LanguageService.getValueByKey('dashboardPage_purchased'), align:'left', sortable: true, value:'purchased'},
            // {text:LanguageService.getValueByKey('dashboardPage_price'), align:'left', sortable: true, value:'price'},
            {text: LanguageService.getValueByKey('dashboardPage_status'), align:'left', sortable: true, value:'status'},
            {text: LanguageService.getValueByKey('dashboardPage_date'), align:'left', sortable: true, value:'date'},
            {text: LanguageService.getValueByKey('dashboardPage_price'), align:'left', sortable: true, value:'price'},
            // {text: LanguageService.getValueByKey('dashboardPage_action'), align:'left', sortable: false, value:'action'},
         ],
      }
   },
   computed: {
      ...mapGetters(['getSalesItems']),
      salesItems(){
         return this.getSalesItems;
      },
   },
   methods: {
      ...mapActions(['updateSalesItems', 'openDashboardDialog']),

      openDialog() {
         this.openDashboardDialog(true);
      },
      checkIsSession(prop){
         return prop === 'TutoringSession';
      },
      checkIsQuestuin(prop){
         return prop === 'Question';
      },
      checkIsItem(prop){
         return prop !== 'Question' && prop !== 'TutoringSession';
      }
   },
   created() {
      this.updateSalesItems()
   },
}
</script>

<style lang="less">
.mySales{
   .v-table {
      thead {
         th{
            // border: 1px solid black;
            // border-bottom: 0;
         }
      }
      tbody {
         td{
            // padding: 8px 24px !important; //vuetify
            // border: 1px solid black;
         }
      }
   }

   .mySales_td_img{
		padding-right: 0 !important;
		width: 100px;
		img{
			height: 80px;
      }
      i {
         // Temporary
         font-size: 70px;
      }
   }

   .mySales_td_course {
      div {
         // padding: 10px 0;
      }
      .content_txt{
         max-width: 300px;
      }
   }
   .sbf-arrow-right-carousel, .sbf-arrow-left-carousel {
      transform: none /*rtl:rotate(180deg)*/;
      height: inherit;
   }
}
</style>
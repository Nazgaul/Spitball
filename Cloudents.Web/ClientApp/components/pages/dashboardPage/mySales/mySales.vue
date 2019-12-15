<template>
      <div class="mySales">
            <v-data-table v-if="salesItems.length"
               border="1"
               :headers="headers"
               :items="salesItems"
               disable-initial-sort
               :rows-per-page-items="['5']"
               class="elevation-1"
               :prev-icon="'sbf-arrow-left-carousel'"
               :sort-icon="'sbf-arrow-down'"
               :next-icon="'sbf-arrow-right-carousel'">
               <template v-slot:items="props">
                  <td class="mySales_td_img">
                     <img :src="props.item.preview" :alt="props.item.info" v-if="props.item.preview">
                     <v-icon v-else>sbf-user</v-icon>
                  </td>
                  <td class="text-xs-left mySales_td_course">
                     <div v-if="props.item.type !== 'TutoringSession'">
                        <span>{{props.item.name}}</span>
                        <span>{{ props.item.info }}</span>
                     </div>
                     <div v-else>
                        <span v-language:inner="'dashboardPage_session'"></span>
                        <span>{{props.item.studentName}} {{props.item.duration | currentHourAndMin}}</span>
                     </div>
                     <div>
                        <span v-language:inner="'dashboardPage_course'"></span>
                        <span>{{props.item.course}}</span>
                     </div>
                  </td>
                  <td class="text-xs-left">{{ props.item.type }}</td>
                  <td class="text-xs-left" v-language:inner="`dashboardPage_${props.item.status.toLowerCase()}`"></td>
                  <td class="text-xs-left">{{ props.item.date | dateFromISO }}</td>
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
         padding: 10px 0;
      }
   }
   .sbf-arrow-right-carousel, .sbf-arrow-left-carousel {
      transform: none /*rtl:rotate(180deg)*/ 
   }
}
</style>
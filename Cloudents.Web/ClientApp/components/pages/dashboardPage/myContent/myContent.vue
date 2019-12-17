<template>
      <div class="myContent">
           
      </div>
</template>

<script>
import { mapActions, mapGetters } from 'vuex';

import { LanguageService } from '../../../../services/language/languageService';

export default {
   name:'myContent',
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
      ...mapGetters(['getContentItems']),
      // contentItems(){
      //    return this.getContentItems;
      // },
   },
   methods: {
      ...mapActions(['updateContentItems', 'openDashboardDialog']),

      // openDialog() {
      //    this.openDashboardDialog(true);
      // }
   },
   created() {
      // this.updateContentItems()
   },
}
</script>

<style lang="less">
.mySales{
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
<template>
<div class="myFollowers">
<div class="myFollowers_title" v-language:inner="'dashboardPage_my_followers_title'"/>
   <v-data-table 
         :page.sync="paginationModel.page"
         :headers="headers"
         :items="followersItems"
         :items-per-page="5"
         hide-default-header
         sort-by
         :item-key="'date'"
         class="elevation-1 myFollowers_table"
         :footer-props="{
            showFirstLastPage: false,
            firstIcon: '',
            lastIcon: '',
            prevIcon: 'sbf-arrow-left-carousel',
            nextIcon: 'sbf-arrow-right-carousel',
            itemsPerPageOptions: [5]
         }">
      <template v-slot:header="{props}">
         <thead>
            <tr>
               <th class="text-xs-left"
                  v-for="header in props.headers"
                  :key="header.value"
                  :class="['column',{'sortable':header.sortable}]"
                  @click="changeSort(header.value)">
                  <span class="text-xs-left">{{ header.text }}
                     <v-icon v-if="header.sortable" v-html="sortedBy !== header.value?'sbf-arrow-down':'sbf-arrow-up'" />
                  </span>
               </th>
            </tr>
         </thead>
      </template>
         <template v-slot:item="props">
            <tr class="myFollowers_table_tr">
               <tablePreviewTd :item="props.item"/>
               <td class="text-xs-left">{{props.item.name}}</td>
               <td class="text-xs-left">{{ props.item.date | dateFromISO }}</td>
               <td class="text-xs-left actions">
                  <v-btn icon @click="sendWhatsapp(props.item)" depressed rounded color="#4caf50">
                     <v-icon v-text="'sbf-whatsup-share'"/>
                     <!-- <span v-language:inner="'dashboardPage_send_whatsapp'"/> -->
                  </v-btn>
                  <v-btn link icon :href="`mailto:${props.item.email}`" depressed rounded  color="#69687d">
                     <v-icon size="18" v-text="'sbf-email'"/>
                     <!-- <span v-language:inner="'dashboardPage_send_email'"/> -->
                  </v-btn>
               </td>
            </tr>
         </template>

         <slot slot="no-data" name="tableEmptyState"/>
   </v-data-table>
</div>
</template>

<script>
import { mapActions, mapGetters } from 'vuex';
import tablePreviewTd from '../global/tablePreviewTd.vue';
import { LanguageService } from '../../../../services/language/languageService';

export default {
   name:'myFollowers',
   components:{tablePreviewTd},
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
         paginationModel:{
            page:1
         },
         sortedBy:'',
         headers:[
            this.dictionary.headers['preview'],
            this.dictionary.headers['name'],
            this.dictionary.headers['joined'],
            this.dictionary.headers['action'],
         ],
      }
   },
   computed: {
      ...mapGetters(['getFollowersItems']),
      followersItems(){
         return this.getFollowersItems
      },
   },
   methods: {
      ...mapActions(['updateFollowersItems','dashboard_sort']),
      changeSort(sortBy){
         if(sortBy === 'info') return;

         let sortObj = {
            listName: 'followersItems',
            sortBy,
            sortedBy: this.sortedBy
         }
         this.dashboard_sort(sortObj)
         this.paginationModel.page = 1;
         this.sortedBy = this.sortedBy === sortBy ? '' : sortBy;
      },
      sendWhatsapp(user) {
         let defaultMessage = LanguageService.getValueByKey("dashboardPage_default_message")
         window.open(`https://api.whatsapp.com/send?phone=${user.phoneNumber}&text=%20${defaultMessage}`);
         this.tutorRequestDialogClose();
      },
   },
   created() {
      this.updateFollowersItems()
   },

}
</script>

<style lang="less">
@import "../../../../styles/mixin.less";
.myFollowers{
   max-width: 1334px;
   .myFollowers_title{
      font-size: 22px;
      color: #43425d;
      font-weight: 600;
      padding: 30px;
      background-color: #fff;
      line-height: 1.3px;
      box-shadow: 0 2px 1px -1px rgba(0,0,0,.2),0 1px 1px 0 rgba(0,0,0,.14),0 1px 3px 0 rgba(0,0,0,.12)!important;
   }
   .myFollowers_table{
      thead{
         tr{
            height: auto;
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
      .actions{
         .v-btn{
            text-transform: none;
         }
      }
      .sbf-arrow-right-carousel, .sbf-arrow-left-carousel {
         transform: none /*rtl:rotate(180deg)*/;
         color: #43425d !important;
         height: inherit;
         font-size: 14px;
      }
      .v-data-footer {
         padding: 6px 0;
         .v-data-footer__pagination {
            font-size: 14px;
            color: #43425d;
         }
      }
   }
}
</style>

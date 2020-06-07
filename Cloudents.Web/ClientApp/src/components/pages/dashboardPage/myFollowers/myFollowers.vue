<template>
<div class="myFollowers">
   <!-- <div class="myFollowers_title">{{$t('dashboardPage_my_followers_title')}}</div> -->
   <v-data-table 
   calculate-widths
         :page.sync="paginationModel.page"
         :headers="headers"
         :items="followersItems"
         :items-per-page="20"
         sort-by
         :item-key="'date'"
         class="elevation-1"
         :footer-props="{
            showFirstLastPage: false,
            firstIcon: '',
            lastIcon: '',
            prevIcon: 'sbf-arrow-left-carousel',
            nextIcon: 'sbf-arrow-right-carousel',
            itemsPerPageOptions: [20]
         }">
      <!-- <template v-slot:header="{props}">
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
      </template> -->
       <!-- <tablePreviewTd :item="props.item"/> -->
                 <!-- <v-avatar :tile="true" tag="v-avatar" :class="'tablePreview_img tablePreview_no_image userColor' + strToACII(props.item.name)" 
                 :style="{width: `80px`, height: `80px`, fontSize: `22px`}">
                     <span class="white--text">{{item.name.slice(0,2).toUpperCase()}}</span>
               </v-avatar> -->
         <template v-slot:top >
            <div class="myFollowers_title">
                  {{$t('dashboardPage_my_followers_title')}}
                  </div>
         </template>
      <template v-slot:item.preview="{item}">
           <user-avatar :user-id="item.userId" 
               :user-image-url="item.image" 
               :size="'40'" 
               :user-name="item.name" >
               </user-avatar>
          
           
      </template>
      <!-- <template v-slot:item.name="{item}">
           <user-avatar :user-id="item.userId" 
               :user-image-url="item.image" 
               :size="'40'" 
               :user-name="item.name" >
               </user-avatar>
                <span>{{item.name}}</span>
      </template>" -->
      <template v-slot:item.date="{item}">
           {{ $d(new Date(item.date)) }}
           
      </template>
        <template v-slot:item.action="{item}">
         <v-btn class="mr-1" icon @click="sendWhatsapp(item)" depressed rounded color="#4caf50" x-small>
            <v-icon v-text="'sbf-whatsup-share'"/>
         </v-btn>
         <v-btn link icon :href="`mailto:${item.email}`" depressed rounded  color="#69687d" x-small>
            <v-icon v-text="'sbf-email'"/>
         </v-btn>
      </template>
         <!-- <template v-slot:item="props">
            <tr>
               <user-avatar :user-id="props.item.userId" 
               :user-image-url="props.item.image" 
               :size="'80'" 
               :user-name="props.item.name" >
               </user-avatar>
              
               <td class="text-xs-left">{{props.item.name}}</td>
               <td class="text-xs-left">{{ $d(new Date(props.item.date)) }}</td>
               <td class="text-xs-left actions">
                  <v-btn icon @click="sendWhatsapp(props.item)" depressed rounded color="#4caf50">
                     <v-icon v-text="'sbf-whatsup-share'"/>
                  </v-btn>
                  <v-btn link icon :href="`mailto:${props.item.email}`" depressed rounded  color="#69687d">
                     <v-icon size="18" v-text="'sbf-email'"/>
                  </v-btn>
               </td>
            </tr>
         </template> -->

         <slot slot="no-data" name="tableEmptyState"/>
   </v-data-table>
</div>
</template>

<script>
import { mapActions, mapGetters } from 'vuex';
import { LanguageService } from '../../../../services/language/languageService';

export default {
   name:'myFollowers',
   props:{
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
   .myFollowers_title {
      font-size: 22px;
      color: #43425d;
      font-weight: 600;
      padding: 30px;
      background-color: #fff;
      line-height: 1.3px;
    //  box-shadow: 0 2px 1px -1px rgba(0,0,0,.2),0 1px 1px 0 rgba(0,0,0,.14),0 1px 3px 0 rgba(0,0,0,.12)!important;
   }
   td:first-child {
      width:1%;
      white-space: nowrap;
   }
   tr:nth-of-type(2n) {
      td {
         background-color: #f5f5f5;
      }
   }
  // .myFollowers_table{
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
      font-size: 14px !important;
   }
   .v-data-footer {
      padding: 6px 0;
      .v-data-footer__pagination {
         font-size: 14px;
         color: #43425d;
      }
   }
  // }
}
</style>

<template>
<div class="myFollowers">
   <v-data-table 
   v-model="selected"
   calculate-widths
         :page.sync="paginationModel.page"
         :headers="headers"
         :items="followersItems"
         :items-per-page="20"
         sort-by
          :search="search"
         :item-key="'date'"
         class="elevation-1"
         show-select
         :footer-props="{
            showFirstLastPage: false,
            firstIcon: '',
            lastIcon: '',
            itemsPerPageOptions: [20]
         }">
         <template v-slot:top >
            <div class="d-flex flex-wrap pa-2">
            <div class="myFollowers_title">
                  {{$t('dashboardPage_my_followers_title')}}
                  </div>
            <v-spacer></v-spacer>
             <v-text-field
               v-model="search"
               :label="$t('search_search_btn')"
               outlined 
               dense
               ></v-text-field>
               
               <v-btn class="mx-1 white--text"
                
                        depressed
                        rounded
                        :block="$vuetify.breakpoint.xsOnly"
                        color="#5360FC"
                v-if="selected.length > 0" @click="SendEmail()">{{$t('send-email')}}</v-btn>
            </div>
         </template>
      <template v-slot:item.preview="{item}">
            <userAvatarNew
               class="followersUserAvatar"
               :user-image-url="item.image"
               :user-name="item.name"
               :width="40"
               :height="40"
               :fontSize="14"
            />
           
      </template>
      <template v-slot:item.date="{item}">
           {{ $d(new Date(item.date)) }}
           
      </template>
        <template v-slot:item.action="{item}">
         <v-btn link icon :href="`mailto:${item.email}`" depressed rounded  color="#69687d" x-small>
            <v-icon>sbf-email</v-icon>
         </v-btn>
      </template>
      <slot slot="no-data" name="tableEmptyState"/>
   </v-data-table>
</div>
</template>

<script>
import { mapActions, mapGetters } from 'vuex';

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
         search: '',
         selected: [],
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
      ...mapGetters(['getFollowersItems','getAccountEmail']),
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
      SendEmail() {
         let emails = this.selected.map(x=>x.email);
         let myEmail = this.getAccountEmail;
         window.open(`mailto:?to=${myEmail}&bcc=${emails.join(';')}`)
      }
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
      line-height: 1.3px;
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
   thead{
         th{
            color: #43425d !important;
            font-size: 14px;
         //   padding-top: 14px;
            //padding-bottom: 14px;
            font-weight: normal; //for title
            min-width: 100px;
         }
         
     
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
   .followersUserAvatar {
      .user-avatar-image-wrap {
         margin: 0 auto;

         .v-lazy {
            display: flex;
         }
      }
   }
}
</style>

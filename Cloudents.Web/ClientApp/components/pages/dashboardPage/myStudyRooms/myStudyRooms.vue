<template>
 <div class="myStudyRooms">
       <v-data-table 
         calculate-widths
         :page.sync="paginationModel.page"
         :headers="headers"
         :items="studyRoomItems"
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
         <template v-slot:top>
            <div class="tableTop d-flex flex-sm-row flex-column align-sm-center justify-space-between">
               <div class="myStudyRooms_title pb-3 pb-sm-0">{{$t('schoolBlock_my_study_rooms')}}</div>
               <div>
                  <v-btn v-if="isTutor" v-openDialog="createStudyRoomDialog" class="link white--text mr-0 mr-sm-4" depressed color="#5360FC">{{$t('dashboardPage_my_studyrooms_create_room')}}</v-btn>
                  <v-btn v-if="!$vuetify.breakpoint.xsOnly" class="link white--text" :to="{name: routeNames.StudyRoom}" depressed color="#5360FC">{{$t('dashboardPage_link_studyroom')}}</v-btn>
               </div>
            </div>
         </template>
         <!-- <template v-slot:item.preview="{item}">
            <user-avatar :user-id="item.userId"
                  :user-image-url="item.image" 
                  :size="'40'" 
                  :user-name="item.name" >
               </user-avatar>
         </template>  -->
         <template v-slot:item.name="{item}">
            {{item.name}}
         </template>
         <template v-slot:item.date="{item}">
            {{ $d(new Date(item.date)) }}
         </template>
         <template v-slot:item.lastSession="{item}">
            <template v-if="item.lastSession">
               {{ $d(new Date(item.lastSession)) }}
            </template>
         </template> 
         <template v-slot:item.action="{item}">
            <v-btn  icon    x-small @click="sendMessage(item)" :title="$t('schoolBlock_SendMessageTooltip')">
               <iconChat/>
            </v-btn>
            <v-btn icon  x-small @click="enterRoom(item.id)" :title="$t('schoolBlock_EnterStudyRoomTooltip')">
               <enterRoom/>
            </v-btn>
         </template>
         <slot slot="no-data" name="tableEmptyState"/>
      </v-data-table>
   </div>
</template>

<script>
import { mapActions, mapGetters } from 'vuex';
//import tablePreviewTd from '../global/tablePreviewTd.vue';
import iconChat from './images/icon-chat.svg';
import enterRoom from './images/enterRoom.svg';
import * as routeNames from '../../../../routes/routeNames'
import * as dialogNames from '../../global/dialogInjection/dialogNames.js'

export default {
   name: 'myStudyRooms',
   components:{
      iconChat,
      enterRoom
   },
   data() {
      return {
         createStudyRoomDialog: dialogNames.CreateStudyRoom,
         routeNames,
         sortedBy:'',
         paginationModel:{
            page:1
         },
         headers:[
          //  this.dictionary.headers['preview'],
            this.dictionary.headers['name'],
            this.dictionary.headers['created'],
            this.dictionary.headers['last_date'],
            this.dictionary.headers['action'],
         ],
      }
   },
   props:{
      dictionary:{
         type: Object,
         required: true
      }
   },
   computed: {
      ...mapGetters(['getStudyRoomItems']),
      isTutor(){
         return this.$store.getters.accountUser.isTutor;
      },
      studyRoomItems(){
         return this.getStudyRoomItems
      },
   },
   methods: {
      ...mapActions(['updateStudyRoomItems','dashboard_sort','openChatInterface','setActiveConversationObj']),

      // showFirstName(name) {
      //    let maxChar = 4;
      //    name = name.split(' ')[0];
      //    if(name.length > maxChar) {
      //    return this.$t('resultTutor_message_me');
      //    }
      //    return name;
      // },
      sendMessage(item){
         let currentConversationObj = {
            userId: item.userId,
            conversationId: item.conversationId,
            name: item.name,
            image: item.image || null,
         }
         this.setActiveConversationObj(currentConversationObj);
         this.openChatInterface();
      },
      enterRoom(id){
         let routeData = this.$router.resolve({
            name: 'roomSettings',
            params: {id}
            });
         global.open(routeData.href, '_self');
      },
      // changeSort(sortBy){
      //    if(sortBy === 'info') return;

      //    let sortObj = {
      //       listName: 'studyRoomItems',
      //       sortBy,
      //       sortedBy: this.sortedBy
      //    }
      //    this.dashboard_sort(sortObj)
      //    this.paginationModel.page = 1;
      //    this.sortedBy = this.sortedBy === sortBy ? '' : sortBy;
      // }
   },
   created() {
      this.updateStudyRoomItems()
   },
}
</script>

<style lang="less">
@import '../../../../styles/mixin.less';
@import '../../../../styles/colors.less';

.myStudyRooms{
   max-width: 1334px;
   .tableTop {
      padding: 30px;
      color: @global-purple !important;
      .myStudyRooms_title{
         font-size: 22px;
         font-weight: 600;
         line-height: 1.3px;
         @media (max-width: @screen-xs) {
            line-height: initial;
         }
         background: #fff;
      }
      .link {
         color: inherit;
      }
   }
   tr{
      height:54px;
   }
   td{
      border: none !important;
   }
   td:first-child {
      // text-align: center !important;
      // width:1%;
      white-space: nowrap;
   }
   tr:nth-of-type(2n) {
      td {
         background-color: #f5f5f5;
      }
   }
   thead{
      tr{
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
}
</style>
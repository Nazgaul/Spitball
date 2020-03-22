<template>
 <div class="myStudyRooms">
      <div class="myStudyRooms_title" v-language:inner="'schoolBlock_my_study_rooms'"/>
      <v-data-table 
            :headers="headers"
            :items="studyRoomItems"
            :items-per-page="5"
            sort-by
            hide-default-header
            :item-key="'date'"
            class="elevation-1 myStudyRooms_table"
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
            <tr class="myStudyRooms_table_tr">
               <tablePreviewTd :item="props.item"/>
               <tableInfoTd :item="props.item"/>
               <td class="text-xs-left">{{ $d(new Date(props.item.date)) }}</td>
               <td class="text-xs-left">{{ $d(new Date(props.item.lastSession)) }}</td>
               <td>
                  <v-btn class="myStudyRooms_btns white--text" depressed rounded color="#4452fc" @click="sendMessage(props.item)">
                     <iconChat class="myStudyRooms_btn_icon"/>
                     <div class="myStudyRooms_btn_txt" v-html="$Ph('resultTutor_send_button', showFirstName(props.item.name))"></div>
                  </v-btn>
                  <v-btn class="myStudyRooms_btns myStudyRooms_btns_enterRoom" depressed rounded color="white" @click="enterRoom(props.item.id)">
                     <enterRoom class="myStudyRooms_btn_icon"/>
                     <span class="myStudyRooms_btn_txt" v-language:inner="'dashboardPage_enter_room'"/>
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
import tableInfoTd from '../global/tableInfoTd.vue';
import iconChat from './images/icon-chat.svg';
import enterRoom from './images/enterRoom.svg';
import { LanguageService } from '../../../../services/language/languageService';


export default {
   name:'myStudyRooms',
   components:{tablePreviewTd,tableInfoTd,iconChat,enterRoom},
   data() {
      return {
         paginationModel:{
            page:1
         },
         sortedBy:'',
         headers:[
            this.dictionary.headers['preview'],
            this.dictionary.headers['student_tutor'],
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
      studyRoomItems(){
         return this.getStudyRoomItems
      },
   },
   methods: {
      ...mapActions(['updateStudyRoomItems','dashboard_sort','openChatInterface','setActiveConversationObj']),
      showFirstName(name) {
         let maxChar = 4;
         name = name.split(' ')[0];
         if(name.length > maxChar) {
         return LanguageService.getValueByKey('resultTutor_message_me');
         }
         return name;
      },
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
      changeSort(sortBy){
         if(sortBy === 'info') return;

         let sortObj = {
            listName: 'studyRoomItems',
            sortBy,
            sortedBy: this.sortedBy
         }
         this.dashboard_sort(sortObj)
         this.paginationModel.page = 1;
         this.sortedBy = this.sortedBy === sortBy ? '' : sortBy;
      }
   },
   created() {
      this.updateStudyRoomItems()
   },
}
</script>

<style lang="less">
.myStudyRooms{
   max-width: 1334px;
   .myStudyRooms_title{
      font-size: 22px;
      color: #43425d;
      font-weight: 600;
      padding: 30px;
      line-height: 1.3px;
      background: #fff;
      box-shadow: 0 2px 1px -1px rgba(0,0,0,.2),0 1px 1px 0 rgba(0,0,0,.14),0 1px 3px 0 rgba(0,0,0,.12)!important;
   }
   .myStudyRooms_table{
      thead{
         tr{
            height: auto;
            th{
               color: #43425d !important;
               font-size: 14px;
               padding-top: 14px;
               padding-bottom: 14px;
               font-weight: normal;
               min-width: 130px;
            }
         }
         color: #43425d !important;
      }
      .myStudyRooms_action{
         outline: none;
         padding: 10px 0px;
         width: 100%;
         max-width: 140px;
         border: 1px solid black;
         border-radius: 26px;
         text-transform: capitalize;
         font-weight: 600;
         font-size: 14px;
      }
      .myStudyRooms_btns{
         max-width: 180px;
         min-width: 180px;
         height: 38px;
         font-size: 12px;
         font-weight: 600;
         text-transform: initial;
         margin: 6px 8px;
         .myStudyRooms_btn_icon {
            text-transform: inherit;
            position: absolute;
            left: 0;
         }
         .myStudyRooms_btn_txt{
            margin-left: 20px;
         }
         &.myStudyRooms_btns_enterRoom{
            // max-width: 170px;
            font-weight: bold;
            color: #43425d;
            border: solid 1px #43425d !important;
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
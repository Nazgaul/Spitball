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
      class="myFollowersTable"
      mobile-breakpoint="0"
      :search="search"
      :item-key="'date'"
      show-select
      :footer-props="{
         showFirstLastPage: false,
         firstIcon: '',
         lastIcon: '',
         itemsPerPageOptions: [20]
      }">
         <template v-slot:top >
            <div class="pa-2">
            <div class="myFollowers_title">{{$t('dashboardPage_my_followers_title')}}</div>
               <div class="d-flex">
                  <v-spacer></v-spacer>
                  <v-text-field
                     v-model="search"
                     :label="$t('search_search_btn')"
                     outlined 
                     dense
                     rounded
                  ></v-text-field>
                  
                  <v-btn class="mx-1 white--text"
                     depressed
                     rounded
                     :block="$vuetify.breakpoint.xsOnly"
                     color="#5360FC"
                     v-if="selected.length > 0" @click="SendEmail()">
                        {{$t('send-email')}}
                  </v-btn>
               </div>
            </div>
         </template>
         <!-- <template v-slot:header.data-table-select="props">
            <v-checkbox
               class="ma-0 pa-0"
               v-model="headerCheckbox"
               :label="$t('select_all')"
               hide-details
            ></v-checkbox>
         </template>
         <template v-slot:item.data-table-select="props">
            <v-checkbox
               class="ma-0 pa-0"
               @change="selectItem(props.item)"
               hide-details
            ></v-checkbox>
         </template> -->
         <template v-slot:item.preview="{item}">
            <userAvatarNew
               class="followersUserAvatar"
               :user-image-url="item.image"
               :user-name="item.name"
               :width="68"
               :height="68"
               :fontSize="14"
            />
           
         </template>
         <template v-slot:item.date="{item}">
            {{ $d(new Date(item.date)) }}
         </template>
         <template v-slot:item.action="{item}">
            <div class="itemsAction d-flex align-center justify-center">
               <div class="d-flex align-center flex-column me-9">
                  <v-btn style="margin-top: 2px;" icon @click="openChatById(item)" depressed rounded  color="#69687d" x-small >
                     <chatSvg />
                  </v-btn>
                  <div class="iconTitle" v-t="'message_me'"></div>
               </div>
               <div class="d-flex align-center flex-column">
                  <v-btn link icon :href="`mailto:${item.email}`" depressed rounded  color="#69687d" x-small>
                     <emailSvg />
                  </v-btn>
                  <div class="iconTitle" v-t="'email_me'"></div>
               </div>
            </div>
         </template>
         <slot slot="no-data" name="tableEmptyState"/>
      </v-data-table>
   </div>
</template>

<script>
import { mapActions, mapGetters } from 'vuex';
import chatService from '../../../../services/chatService.js';
import { MessageCenter } from '../../../../routes/routeNames.js';
import chatSvg from './chat-24-px-copy-24.svg';
import emailSvg from './fill-1.svg';

export default {
   name:'myFollowers',
   components: {
      chatSvg,
      emailSvg
   },
   props:{
      dictionary:{
         type: Object,
         required: true
      }
   },
   data() {
      return {
         headerCheckbox: false,
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
   watch: {
      headerCheckbox(val) {
         console.log(val);
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
      selectItem(item) {
         if(!item.hasOwnProperty('selected')) {
            item.selected = true
            return
         }
         item.selected = !item.selected
         console.log(item);
      },
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
         window.location.href = `mailto:?to=${myEmail}&bcc=${emails.join(';')}`;
      },
      openChatById(user){
         let currentUser = this.$store.getters.accountUser;
         let conversationObj = {
            userId: user.userId,
            image: user.image,
            name: user.name,
            conversationId: chatService.createConversationId([user.userId, currentUser?.id]),
         }
         let isNewConversation = !(this.$store.getters.getIsActiveConversationTutor(conversationObj.conversationId))
         if(isNewConversation){
            let tutorInfo = {
               id: currentUser?.id,
               name: `${currentUser.firstName} ${currentUser.lastName}`,
               image: currentUser?.image,
            }
            this.$store.commit('ACTIVE_CONVERSATION_TUTOR', { tutorInfo, conversationId:conversationObj.conversationId })
         }

         let currentConversationObj = chatService.createActiveConversationObj(conversationObj)
         this.$store.dispatch('setActiveConversationObj', currentConversationObj);
         this.$router.push({name: MessageCenter, params: { id:currentConversationObj.conversationId }})
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
   .myFollowersTable {
      @media (max-width: @screen-xs) {
         border-radius: 0;
      }
   }
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
   thead{
         th{
            color: #43425d !important;
            font-size: 14px;
         //   padding-top: 14px;
            //padding-bottom: 14px;
            font-weight: normal; //for title
            min-width: 100px;
            border-top: thin solid rgba(0, 0, 0, 0.12);
         }
         
     
   }
   tbody {
      tr {
         height: 98px;
         &:nth-of-type(2n) {
            td {
               background-color: #f5f6fa;
            }  
         }
         td {
            border-bottom: none !important;
         }
      }
   }

   .actions{
      .v-btn{
         text-transform: none;
      }
   }
   .sbf-arrow-right-carousel, .sbf-arrow-left-carousel {
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
         // margin: 0 auto;
         .v-lazy {
            display: flex;
         }
      }
   }
   .itemsAction {
      line-height: 2;
      .iconTitle {
         font-size: 12px;
         color: #69687d;
      }
   }
}
</style>

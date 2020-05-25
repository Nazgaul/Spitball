<template>
   <v-dialog v-model="dialogState" persistent max-width="640px" :fullscreen="$vuetify.breakpoint.xsOnly">
      <div class="createGroupChatDialog pa-4 d-sm-block d-flex flex-column justify-space-between">

         <v-form class="justify-space-between input-room-name mb-3" ref="createChatValidation">
            <v-icon class="close-dialog" v-text="'sbf-close'" @click="$emit('updateCreateGroupDialogState',false)" />
            <div class="createGroupChatDialog-title text-center" v-t="'chat_create_title'"></div>
            <div class="createGroupChatDialog-list">
               <div class="listTitle mb-3" v-t="'chat_invite'"></div>
               <v-list flat class="list-followers">
                  <v-list-item-group>
                     <v-list-item 
                           v-for="(item, index) in myFollowers"
                           :key="index"
                           @click="addSelectedUser(item)"
                           :class="[{'dark-line': index % 2}]">
                           <template v-slot:default="{}">
                              <v-list-item-avatar>
                                 <UserAvatar :size="'40'" :user-name="item.name" :user-id="item.id" :userImageUrl="item.image"/> 
                              </v-list-item-avatar>
                              <v-list-item-content>
                                 <v-list-item-title>{{item.name}}</v-list-item-title>
                              </v-list-item-content>
                              <v-list-item-action>
                                 <v-checkbox @click.prevent multiple v-model="selected" :value="item" color="#4c59ff" off-icon="sbf-check-box-un" on-icon="sbf-check-box-done"></v-checkbox>
                              </v-list-item-action>
                           </template>
                     </v-list-item>
                  </v-list-item-group>
               </v-list>
            </div>

         </v-form>
         <div class="d-flex flex-column align-center pt-4">
            <div class="mb-4">
               <span v-if="currentError" class="error--text" v-t="currentError"></span>
            </div>
            <v-btn @click="createChat" width="160" depressed height="40" color="#4452fc" class="white--text" rounded >{{$t('chat_create_btn_txt')}}</v-btn>
         </div>
      </div>
   </v-dialog>
</template>

<script>
import chatService from '../../../../services/chatService.js';
export default {
   props:{
      dialogState:{
         type:Boolean,
         required:true
      }
   },
   data() {
      return {
         selected:[],
         myFollowers:[],
         currentError: '',
         errorsResource:{
            showErrorEmpty: 'dashboardPage_create_room_empty_error',
            showErrorMaxUsers: 'dashboardPage_create_room_max_error',
         },
         errors: {
            showErrorEmpty: false,
         },
         MAX_PARTICIPANT: 49,
      }
   },
   computed: {
      isErrors() {
         return this.errors.showErrorEmpty || this.errors.showErrorMaxUsers;
      }
   },
   watch: {
      selected(){
         this.resetErrors()
      }
   },
   methods: {
      resetErrors(){
         this.errors.showErrorEmpty = false;
         this.errors.showErrorMaxUsers = false;
         this.currentError = '';
      },
      createChat(){
         if(this.isErrors) return;
         
         if(!this.selected.length) {
            this.errors.showErrorEmpty = true
            this.currentError = this.errorsResource.showErrorEmpty;
            return;
         }
         let currentUserId = this.$store.getters?.accountUser?.id;
         let conversationId = Array.from(this.selected.map(user=> user.userId));
         conversationId.push(currentUserId);
         conversationId = conversationId.sort((a,b)=> a - b).join('_');
         let conversation = this.$store.getters.getConversations.find(c=>c.conversationId == conversationId)
         
         if(!conversation){
            conversation = {
               conversationId: conversationId,
               name: this.selected.map(u=>u.name).join(" ,"),
               image: this.selected[0].image,
            }
         }
         let currentConversationObj = chatService.createActiveConversationObj(conversation);
         this.$store.dispatch('setActiveConversationObj',currentConversationObj);
         this.$router.push({...this.$route,params:{id:currentConversationObj.conversationId}}).catch(()=>{})
         this.$emit('updateCreateGroupDialogState',false);
      },
      addSelectedUser(user){
         let idx;
         let isInList = this.selected.some((u,i)=>{
               idx = i;
               return u.userId === user.userId;
         })
         if(isInList){
               this.selected.splice(idx,1);
         } else {
               if(this.selected.length < this.MAX_PARTICIPANT){
                  this.resetErrors()
                  this.selected.push(user)
               } else {
                  this.currentError = this.errorsResource.showErrorMaxUsers
               }
         }
      },
   },
   created() {
      this.$store.dispatch('updateFollowersItems').then(()=>{
         this.myFollowers = this.$store.getters.getFollowersItems
      })
   }

}
</script>

<style lang="less">
@import '../../../../styles/mixin.less';
@import '../../../../styles/colors.less';
.createGroupChatDialog{
   background: white;
   position: relative;
   height: 100%;
   .close-dialog {
      position: absolute;
      right: 12px;
      font-size: 12px;
   }
   .createGroupChatDialog-title {
      color: @global-purple;
      font-size: 20px;
      font-weight: 600;
      padding-bottom: 34px;
   }

   .input-room-name{
      width: 100%;
      .createGroupChatDialog-list{
         ::-webkit-scrollbar-track {
            background: #f5f5f5; 
         }
         ::-webkit-scrollbar {
            width: 6px;
         }
         ::-webkit-scrollbar-thumb {
            background: #bdc0d1 !important;
            border-radius: 4px !important;
         }
         min-height: 320px;
         width: 100%;
         .listTitle {
            font-size: 18px;
            font-weight: 600;
            color: @global-purple;
         }
         .list-followers {
            overflow-y: auto;
            max-height: 270px;
            @media (max-width: @screen-xs) {
               max-height: 350px;
            }
            .v-item-group {
               padding-right: 6px;
            }
            .dark-line{
               background: #f5f5f5;
            }
         }
      }
   }
}
</style>
<template>
   <v-btn @click="sendMessage"
      :width="isMobile? '50px' : '200px'" 
      :height="isMobile? '50px' :'46px'" 
      depressed :fab="isMobile" color="#317ca0"
      class="profileFloatingBtn">
         <v-icon size="24" color="white" class="me-0 me-sm-4" v-text="'sbf-message-icon'"/>
         <span class="btnCon" v-if="!isMobile">{{$t('message_me')}}</span>
   </v-btn> 
</template>

<script>
import chatService from '../../../services/chatService.js';
import { MessageCenter } from '../../../routes/routeNames.js';

export default {
   computed: {
      isMobile(){
         return this.$vuetify.breakpoint.xsOnly;
      },
   },
   methods: {
      sendMessage() {
         if(this.$store.getters.getIsMyProfile) return;
         let currentProfile = this.$store.getters.getProfile;
         if(!this.$store.getters.getUserLoggedInStatus) {
            this.$ga.event('Tutor_Engagement', 'contact_BTN_profile_page', `userId:GUEST`);
            this.$store.dispatch('updateCurrTutor', currentProfile.user)
            this.$store.dispatch('setTutorRequestAnalyticsOpenedFrom', {
               component: 'profileContactBtn',
               path: this.$route.path
            });
            this.$store.dispatch('updateRequestDialog', true);
         } else {
            this.$ga.event('Request Tutor Submit', 'Send_Chat_Message', `${this.$route.path}`);
            let conversationObj = {
               userId: currentProfile.user.id,
               image: currentProfile.user.image,
               name: currentProfile.user.name,
               conversationId: chatService.createConversationId([currentProfile.user.id, this.$store.getters.getAccountId]),
            }
            let isNewConversation = !(this.$store.getters.getIsActiveConversationTutor(conversationObj.conversationId))
            if(isNewConversation){
               let tutorInfo = {
                  id: currentProfile.user.id,
                  name: currentProfile.user.name,
                  image: currentProfile.user.image,
                  calendar: currentProfile.user.calendarShared,
               }
               this.$store.commit('ACTIVE_CONVERSATION_TUTOR', { tutorInfo, conversationId:conversationObj.conversationId })
            }
            let currentConversationObj = chatService.createActiveConversationObj(conversationObj)
            this.$store.dispatch('setActiveConversationObj', currentConversationObj);
            this.$router.push({name: MessageCenter, params: { id:currentConversationObj.conversationId }})
         }
      }
   },
}
</script>

<style lang="less">
@import "../../../styles/mixin.less";
   .profileFloatingBtn{
      position: sticky;
      .btnCon{
         color: #ffffff;
         text-align: right;
         font-size: 16px;
         font-weight: 600;
      }
      margin: 0 6px 0px auto;
      bottom: 0px;
      
      @media (max-width: @screen-xs) {
         bottom: 6px;
      }
   }
</style>
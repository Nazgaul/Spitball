<template>
  <div class="messageCenter d-flex">
     <div class="messageCenter2 d-flex">
         <conversationsContainer style="z-index:2" v-if="showConversationsList"/>
         <currentConversationContainer @toggleTeacherInfo="isTeacherInfoOpen = !isTeacherInfoOpen" style="z-index:2" v-if="showCurrentConversation"/>
         <teacherInfoContainer @toggleTeacherInfo="isTeacherInfoOpen = false" style="z-index:2" v-if="showTeacherInfo"/>
     </div>
  </div>
</template>

<script>
import { mapGetters } from 'vuex';
import chatService from '../../../services/chatService.js'
const conversationsContainer = () => import('./components/conversationsContainer.vue');
const currentConversationContainer = () => import('./components/currentConversationContainer.vue');
const teacherInfoContainer = () => import('./components/teacherInfoContainer.vue');
export default {
   data() {
      return {
         isTeacherInfoOpen:false,
      }
   },
   components:{
      conversationsContainer,
      currentConversationContainer,
      teacherInfoContainer
   },
   computed: {
      ...mapGetters(['getConversations']),
      isMobile(){
         return this.$vuetify.breakpoint.xsOnly;
      },
      showConversationsList(){
         return !this.isMobile || !this.$store.getters.getActiveConversationObj?.conversationId;
      },
      showCurrentConversation(){
         return this.$vuetify.breakpoint.mdAndUp || this.$store.getters.getActiveConversationObj?.conversationId && !this.isTeacherInfoOpen;
      },
      showTeacherInfo(){
         return this.$vuetify.breakpoint.mdAndUp || this.isTeacherInfoOpen;
      }
   },
   watch: {
      "$route.params.id": function(){
         this.isTeacherInfoOpen = false;
      },
      getConversations:{
         immediate:true,
         handler(newVal){
            if(newVal.length){
               let currentActiveChatId = this.$store.getters.getActiveConversationObj?.conversationId;
               let idFromRouteParam = this.$route.params.id;
               if(idFromRouteParam != currentActiveChatId || !idFromRouteParam){
                  let conversation = newVal.find(c => c.conversationId == idFromRouteParam)
                  if(conversation){
                     let currentConversationObj = chatService.createActiveConversationObj(conversation)
                     this.$store.dispatch('setActiveConversationObj',currentConversationObj).catch(()=>{});
                  }else{
                     let firstConversation = {};
                     if(!this.isMobile){
                        firstConversation = newVal[0];
                     }
                     let currentConversationObj = chatService.createActiveConversationObj(firstConversation)
                     this.$store.dispatch('setActiveConversationObj',currentConversationObj);
                     this.$router.push({...this.$route,params:{id:firstConversation?.conversationId}}).catch(()=>{});
                  }
               }
            }
         }
      },
   },
   beforeCreate() {
      let conversationsList = this.$store.getters.getConversations
      if(conversationsList.length){
        return
      }else{
        this.$store.dispatch("getAllConversations")
      }
   },
   beforeDestroy(){
      this.$store.dispatch('setActiveConversationObj',chatService.createActiveConversationObj({}));
   }
}
</script>

<style lang="less">
   @import '../../../styles/mixin.less';
   .messageCenter{
      height: 100%;
      padding: 24px 32px 16px 32px;
      max-height: calc(~"100vh - 70px"); //global header height;
      background-color: #dddddd;
      @media(max-width: @screen-xs) {
         padding: 0;
         max-height: calc(~"100vh - 62px");
      }
      &::before{
         content: '';
         position: absolute;
         background: #4c59ff;
         width: 100%;
         top: 0;
         left: 0;
         right: 0;
         height: 40vh;
         bottom: 0;
      }
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
      .messageCenter2{
         width: 100%;
         max-width: 1302px;
         height: 100%;
         border-radius: 6px;
         overflow: hidden;
         box-shadow: 0 0 9px 0 rgba(0, 0, 0, 0.14);
         margin: 0 auto;
         @media(max-width: @screen-xs) {
            margin: initial;
            box-shadow: none;
            overflow:initial;
            border-radius: 0;
         }
      }
   }
</style>
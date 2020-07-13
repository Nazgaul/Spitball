<template>
   <v-flex xs12 sm6 md5 lg6 class="currentConversationContainer">
      <div class="cMessagesHeader d-flex flex-grow-0 flex-shrink-0 align-center ">
         <v-icon @click="$emit('toggleTeacherInfo')" :style="isRtl?'transform: scaleX(-1)':''" class="openTeacherInfoIcon ml-4 d-flex d-md-none" size="24" :color="isMobile? '#ffffff' : '#69687d'">sbf-read-more</v-icon>
         <v-icon @click="backToChatList" class="ml-4 d-flex d-sm-none" size="16" color="#ffffff">{{isRtl?'sbf-arrow-right-carousel':'sbf-arrow-left-carousel'}}</v-icon>
         <user-avatar class="ml-4" :size="'40'" :userImageUrl="currentAvatar" :user-name="currentConversationObj.name"/>
         <div class="pl-3 pr-12 pr-md-7 text-truncate">
            <div class="chatName text-truncate pr-6 pr-sm-3 pr-md-0">{{currentName}}</div>
         </div>
      </div>
      <v-sheet class="currentMessages d-flex flex-grow-1">
         <messages/>
      </v-sheet>
   </v-flex>
</template>

<script>
const messages = () => import('../../../chat/components/messages.vue');

export default {
   data() {
      return {
         isRtl: global.isRtl,
      }
   },
   components:{messages},
   computed: {
      currentConversationObj(){
         return this.$store.getters.getActiveConversationObj
      },
      currentAvatar(){
         return this.currentConversationObj?.image;
      },
      currentName(){
         return this.currentConversationObj?.name;
      },
      isMobile(){
         return this.$vuetify.breakpoint.xsOnly;
      }
   },
   methods: {
      backToChatList(){
         this.$store.dispatch('setActiveConversationObj',{});
         this.$router.push({...this.$route,params:{id:undefined}})
      }
   },
}
</script>

<style lang="less">
   @import '../../../../styles/mixin.less';
   .currentConversationContainer{
      @headerHeight:62px;
      @media(max-width: @screen-xs) {
         @headerHeight: 60px;
         height: 100vh;
      }
      height: 100%;
      .cMessagesHeader{
         @media(max-width: @screen-xs) {
            background-color: #4c59ff;
            color: #ffffff;
         }
         max-width: 100%;
         height: @headerHeight;
         background-color: #efefef;
         color: #43425d;
         position: relative;
         .openTeacherInfoIcon{
            position: absolute;
            right: 16px;
            top: 20px;
         }
         .chatName{
            font-size: 16px;
            font-weight: 600;
         }
      }
      .currentMessages{
         height: calc(~"100% - 62px");
         border-radius: 0;
         background-image: url('../group-10.png');
         background-repeat: repeat;
         background-color: #a6bcd8;
         @media(max-width: @screen-xs) {
         //   height: calc(~"100% - 112px");
         }
         .messages-input {
            top: initial !important;
         }
      }
   }
</style>
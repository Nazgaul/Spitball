<template>
   <div class="currentConversationContainer flex-grow-1 text-truncate">
      <div class="cMessagesHeader d-flex flex-grow-0 flex-shrink-0 align-center ">
         <v-icon @click="backToChatList" class="ml-4 d-flex d-sm-none" size="16" color="#ffffff">sbf-arrow-left-carousel</v-icon>
         <user-avatar class="ml-4" :size="'40'" :userImageUrl="currentAvatar" :user-name="currentConversationObj.name"/>
         <div class="ml-3 text-truncate">
            <div class="chatName">Group name</div>
            <div class="chatUsers text-truncate">{{currentName}}</div>
         </div>
      </div>
      <v-sheet class="currentMessages d-flex flex-grow-1">
         <messages/>
      </v-sheet>
   </div>
</template>

<script>
const messages = () => import('../../../chat/components/messages.vue');

export default {
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
      }
   },
   methods: {
      backToChatList(){
         this.$store.dispatch('setActiveConversationObj',{})
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
      }
      width: 100%;
      height: 100%;
      .cMessagesHeader{
         @media(max-width: @screen-xs) {
            background-color: #4c59ff;
            color: #ffffff;
            border: none;
         }
         max-width: 100%;
         height: @headerHeight;
         background-color: #efefef;
         border-right: 1px solid #e4e4e4;
         border-bottom: 1px solid #e4e4e4;
         color: #43425d;
         .chatName{
            @media(max-width: @screen-xs) {
               font-size: 14px;
            }
            font-size: 16px;
            font-weight: 600;
         }
         .chatUsers{
            @media(max-width: @screen-xs) {
               font-size: 12px;
            }
            font-size: 14px;
            font-weight: 600;
         }
      }
      .currentMessages{
         height: calc(~"100% - 62px");
         .messages-input {
            top: initial !important;
         }
      }
      .messages-header{
         display: none;
      }
   }
</style>
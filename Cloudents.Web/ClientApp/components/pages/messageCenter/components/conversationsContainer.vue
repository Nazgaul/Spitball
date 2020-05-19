<template>
   <div class="conversationsContainer flex-grow-0 flex-shrink-0">
      <div class="conversationsHeader d-flex align-center flex-grow-0 flex-shrink-0">
         <span class="cHeaderTitle">
            <v-icon class="mr-2" size="20">sbf-message-icon</v-icon>
            {{$t("chat_messages")}}
         </span>
      </div>
      <v-sheet class="conversationsList d-flex flex-grow-1">
         <conversations></conversations>
      </v-sheet>
   </div>
</template>

<script>
const conversations = () => import('../../../chat/components/conversations.vue');
export default {
   components:{
      conversations
   }
}
</script>

<style lang="less">
   @import '../../../../styles/mixin.less';
   @headerHeight: 62px;
   @media(max-width: @screen-xs) {
      @headerHeight: 60px;
   }
   .conversationsContainer{
      height: 100%;
      width: 100%;
      max-width: 408px;
      @media(max-width: @screen-xs) {
         max-width: initial;
      }
      background: #ffffff;
      .conversationsHeader{
         width: 100%;
         height: @headerHeight;
         @media(max-width: @screen-xs) {
            background-color: #4c59ff;
         }
         background-color: #efefef;
         border-right: 1px solid #e4e4e4;
         border-bottom: 1px solid #e4e4e4;
         .cHeaderTitle{
            font-size: 16px;
            font-weight: 600;
            color: #43425d;
            padding-left: 18px;
            @media(max-width: @screen-xs) {
               color: #ffffff;
               .v-icon{
                  color: #ffffff;
               }
            }
         }
      }
      .conversationsList{
         height: calc(~"100% - 62px");
      }
   }
</style>
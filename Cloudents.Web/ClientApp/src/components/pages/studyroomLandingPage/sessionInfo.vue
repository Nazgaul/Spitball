<template>
   <div class="sessionInfo px-4 px-lg-0">
      <div class="sessionTitle pt-0 pt-sm-6" v-t="'what_learn'"/>
      <div class="divider"/>
      <div class="sessionText px-sm-4 px-md-0" v-text="sessionDescription"/>
   </div>
</template>

<script>
export default {
   computed: {
      sessionDescription(){
         return this.$store.getters.getRoomDetails?.description;
      }
   },
}
</script>

<style lang="less">
   @import '../../../styles/mixin.less';
   .sessionInfo{
      width: 100%;
      background-color: #ffffff;
      margin-top: 24px;
      text-align: center;
      color: #43425d;
      @media(max-width: @screen-xs) {
         margin-top: 20px;
      }
      .sessionTitle{
         font-size: 28px;
         font-weight: 600;
         @media(max-width: @screen-xs) {
            font-size: 22px;
         }
      }
      .divider{
         margin: 12px auto 22px;
         @media(max-width: @screen-xs) {
            margin: 10px auto 16px;
         }
         width: 64px;
         background: #41c4bc;
         height: 3px;
      }
      .sessionText{
         @media(max-width: @screen-xs) {
            font-size: 18px;
         }
         font-size: 20px;
         line-height: 1.7;
         max-width: 855px;
         margin: 0 auto;
         @media(max-width: @screen-xs) {
            text-align: initial;
         }
      }
   }
</style>
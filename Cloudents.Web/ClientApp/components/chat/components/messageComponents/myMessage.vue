<template>
   <v-card color="#deedff" tile class="myMessageContainer pa-3">
      <v-card-title class="messageTitle pa-0">
         {{userName}}:
      </v-card-title>
      <v-card-text tag="p" dir="auto" class="messageText px-0 pb-0 pt-1">
            {{message.text}}
      </v-card-text>
   </v-card>
</template>

<script>
export default {
   props:{
      message:{
         type:Object,
         required:true
      }
   },
   computed: {
      userName(){
         console.log(this.message.text)
         return this.$store.getters.accountUser?.name
      }
   },

}
</script>

<style lang="less">
.myMessageContainer{
   border-radius: 8px 8px 0px 8px;
   max-width: 70%;
   width: fit-content;
   box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.25);
   margin-right: unset;
   margin-left: auto;
   .messageTitle{
      font-size: 14px;
      font-weight: 600;
      color: black;
      line-height: 1;
   }
   .messageText{
      font-size: 14px;
      color: black !important;
      // line-height: normal;
      // white-space: pre-wrap;
      // white-space: pre-line;
      // word-break: break-word;
   }
}
</style>
<template>
   <div class="sessionInfo">
      <div class="sessionTitle pt-6" v-t="'what_learn'"/>
      <div class="divider"/>
      <div class="sessionText" v-text="sessionDescription"/>
   </div>
</template>
<script>
export default {
   data() {
      return {
         sessionDescription: `Learn how to effectively search for a quality online teaching job! Many companies are looking for teachers. Find hidden job boards and awesome online teaching opportunities. We'll also talk about how to network on Linked In to find work and how to search for online teaching jobs using keywords.`
      }
   },
}
</script>

<style lang="less">
   .sessionInfo{
      width: 100%;
      background-color: #ffffff;
      margin-top: 22px;
      text-align: center;
      color: #43425d;
      .sessionTitle{
         font-size: 28px;
         font-weight: 600;
      }
      .divider{
         margin: 12px auto 22px;
         width: 64px;
         background: #41c4bc;
         height: 3px;
      }
      .sessionText{
         font-size: 20px;
         line-height: 1.7;
         max-width: 855px;
         margin: 0 auto;
      }
   }
</style>
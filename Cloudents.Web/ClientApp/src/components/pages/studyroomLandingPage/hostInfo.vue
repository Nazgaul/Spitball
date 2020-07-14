<template>
   <div class="hostInfo">
      <div class="hostTitle" v-t="'about_host'"/>
      <div class="divider"/>
      <div class="hostBox d-flex">
         <div class="hostImg">
            <img src="./hi.png" alt="">
         </div>
         <div class="hostText">
            <div class="hostName" v-text="hostName"/>
            <div class="hostBio" v-text="hostBio"/>
         </div>
      </div>
   </div>
</template>

<script>
export default {
   data() {
      return {
         hostName: "Ariel Feinsilberg",
         hostBio: `Has extensive experience with students from all universities and colleges, in preparation for matriculation and IMAT exams
If you are looking to learn all the material from the ground up, or just a quick review of the material, I will give you the tools and help you while preparing a clear and concise summary.
Also runs marathons for EvanLine teams !!
Teaches data extraction methods, and provides simple tools.`
      }
   },

}
</script>

<style lang="less">
   .hostInfo{
      width: 100%;
      background-color: #f5f5f5;
      height: 498px;
      margin-top: 54px;
      margin-bottom: 80px;
      color: #43425d;
      padding-top: 34px;
      .hostTitle{
         text-align: center;
         font-size: 28px;
         font-weight: 600;
      }
      .divider{
         margin: 12px auto 36px;
         width: 64px;
         background: #41c4bc;
         height: 3px;
      }
      .hostBox{
         margin: 0 auto;
         max-width: 904px;
         height: 329px;
         padding: 16px;
         background: white;
         .hostText{
            padding-left: 24px;
            display: flex;
            flex-direction: column;
            justify-content: center;
            .hostName{
               font-size: 22px;
               font-weight: 600;
               color: #41c4bc;
               padding-bottom: 8px;
            }
            .hostBio{
               font-size: 18px;
               line-height: 1.67;
               color: #43425d;
            }
         }
      }
   }
</style>
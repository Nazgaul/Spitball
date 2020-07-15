<template>
   <div class="hostInfo">
      <div class="hostTitle" v-t="'about_host'"/>
      <div class="divider"/>
      <div class="hostBox d-flex flex-wrap flex-sm-nowrap">
         <div class="hostImg">
            <UserAvatarNew
            :fontSize="16" 
            :tile="true"
            :width="242"
            :height="298"
            :user-name="tutorName" 
            :user-id="tutorId" 
            :userImageUrl="tutorImage"/> 
         </div>
         <div class="hostText">
            <div class="hostName" v-text="tutorName"/>
            <div class="hostBio" v-text="tutorBio"/>
         </div>
      </div>
   </div>
</template>

<script>
export default {
   computed: {
      tutorName(){
         return this.$store.getters.getRoomTutor?.tutorName;
      },
      tutorId(){
         return this.$store.getters.getRoomTutor?.tutorId;
      },
      tutorImage(){
         return this.$store.getters.getRoomTutor?.tutorImage;
      },
      tutorBio(){
         return `Has extensive experience with students from all universities and colleges, in preparation for matriculation and IMAT exams
If you are looking to learn all the material from the ground up, or just a quick review of the material, I will give you the tools and help you while preparing a clear and concise summary.
Also runs marathons for EvanLine teams !!
Teaches data extraction methods, and provides simple tools.`
      }
   },

}
</script>

<style lang="less">
   @import '../../../styles/mixin.less';

   .hostInfo{
      width: 100%;
      padding: 34px 24px 48px;
      @media(max-width: @screen-xs) {
         background: none;
         padding: 0;
         margin-bottom: 40px;
      }

      background-color: #f5f5f5;
      margin-top: 54px;
      margin-bottom: 80px;
      color: #43425d;
      .hostTitle{
         text-align: center;
         font-size: 28px;
         font-weight: 600;
         @media(max-width: @screen-xs) {
            font-size: 22px;
         }
      }
      .divider{
         margin: 12px auto 36px;
         width: 64px;
         background: #41c4bc;
         height: 3px;
         @media(max-width: @screen-xs) {
            margin: 10px auto 26px;
         }
      }
      .hostBox{
         margin: 0 auto;
         max-width: 904px;
         padding: 16px;
         background: white;
         @media(max-width: @screen-xs) {
            padding: 0;
            justify-content: center;
         }
         .hostText{
            padding-left: 24px;
            @media(max-width: @screen-xs) {
               padding: 0 16px;
               padding-top: 24px;
            }
            display: flex;
            flex-direction: column;
            justify-content: center;
            .hostName{
               font-size: 22px;
               @media(max-width: @screen-xs) {
                  font-size: 20px;
                  padding-bottom: 4px;
               }

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
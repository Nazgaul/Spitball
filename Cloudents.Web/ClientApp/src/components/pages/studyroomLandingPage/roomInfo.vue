<template>
   <div class="roomInfoContainer d-flex flex-column">
      <div class="roomInfoTop d-flex">
         <div class="rightSide">
            <div class="classTitle">{{$t('live_with',[tutorName])}}</div>
            <div class="classSubject" v-text="classSubject"/>
            <div>
               <div>10 live sessions - every  Tuesday and Thursday</div>
               <div>{{$t('starts_on',[date])}}</div>
            </div>
            <div>Only $39.99 for the entire class</div>
            <img class="triangle" src="./images/triangle.png">
         </div>
         <div class="leftSide">
            <img src="../../new_profile/components/profileLiveClasses/live-banner-desktop.png">
         </div>
      </div>
      <div class="roomInfoBottom d-flex">
         <div class="bottomRight text-center">
            <v-btn class="saveBtn" v-t="'save_spot'" depressed height="82px" color="#1b2441"/>
            <div class="couponText" v-t="'apply_coupon_code'"/>
         </div>
         <div class="bottomLeft"></div>
      </div>
   </div>
</template>

<script>
export default {
   data() {
      return {
         tutorName:'Ariel Feinsilberg',
         classSubject: 'Just Say Hello-Find an Online Teaching Job',
         date: `July 19th, 7:00 PM`
         
      }
   },
}
</script>

<style lang="less">
   .roomInfoContainer{
      width: 100%;
      .roomInfoTop{
         width: 100%;
         height: 357px;
         .rightSide{
            color: white;
            font-size: 20px;
            width: 50%;
            background-color: #1b2441;
            text-align: center;
            position: relative;
            display: flex;
            flex-direction: column;
            justify-content: space-evenly;
            .classTitle{
               font-size: 22px;
               color: #41c4bc;
            }
            .classSubject{
               font-size: 42px;
            }
            .triangle{
               position: absolute;
               bottom: -22px;
               left: calc(~"50% - 32px");
            }
         }
         .leftSide{
            width: 50%;
            img{
               width: 100%;
            }
         }
      }
      .roomInfoBottom{
         width: 100%;
         height: 170px;
         background-color: #41c4bc;
         .bottomRight{
            width: 50%;
            .saveBtn{
               width: 100%;
               border-radius: 8px;
               max-width: 380px;
               margin-top: 34px;
               font-size: 22px;
               font-weight: 600;
               color: white;
            }
            .couponText{
               font-size: 20px;
               color: #1b2441;
               text-decoration: underline;
               padding-top: 10px;
            }
         }
         .bottomLeft{
            width: 50%;
         }
      }
   }
</style>
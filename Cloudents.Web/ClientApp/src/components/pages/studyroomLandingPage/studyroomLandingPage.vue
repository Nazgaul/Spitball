<template>
   <div class="studyroomLandingPage d-flex">
      <div class="pageWrapper">
         <logo class="pageLogo"/>
         <roomInfo/>
         <sessionInfo/>
         <hostInfo/>
      </div>
   </div>
</template>

<script>
import roomInfo from './roomInfo.vue';
import sessionInfo from './sessionInfo.vue';
import hostInfo from './hostInfo.vue';
import logo from '../../app/logo/logo.vue'
export default {
   components:{
      roomInfo,
      sessionInfo,
      hostInfo,
      logo
   }
}
</script>

<style lang="less">
   .studyroomLandingPage{
      width: 100%;
      height: 100%;
      background: white;  
      position: relative;
      &::before{
         content: '';
         position: absolute;
         background-repeat: no-repeat;
         background-size: cover;
         background-position: center;
         background-image: url('./images/Landing-page_small.jpg');
         width: 100%;
         max-height: 714px;
         top: 0;
         left: 0;
         right: 0;
         bottom: 0;
         z-index: 1;
      }
      .pageWrapper{
         z-index: 2;
         margin: 0 auto;
         max-width: 1100px;
         .pageLogo{
            margin-top: 16px;
            margin-bottom: 16px;
            .logo {
               fill: #fff !important;
               width: 120px;
            }
         }
      }

   }
</style>
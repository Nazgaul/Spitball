<template>
<div class="authenticationPage">
   <!-- <router-view></router-view> -->
   <!-- <authenticationImage/> -->
   <div class="leftDynamicImgs text-center">
      <logo class="logo"/>
      <div class="bottom-text">
         Lorem Ipsum is simply dummy text of the printing and typesetting
      </div>
   </div>
</div> 
</template>

<script>
const logo = () => import('../../app/logo/logo.vue');
export default {
   name:'authenticationPage',
   components:{logo}
}
</script>

<style lang="less">
.authenticationPage{
   .leftDynamicImgs{
      height: 100vh;
      width: 508px;
      background-image: url('./images/sideImage.jpg');
      background-repeat: no-repeat;
      background-size: contain;
      display: flex;
      flex-direction: column;
      justify-content: space-between;
      align-items: center;
      padding: 22px 24px 18px;
      .logo{
         width: 175px;
         height: 40px;
      }
      .bottom-text{
         font-size: 26px;
         font-weight: 600;
         color: #ffffff;
      }
   }
}
</style>

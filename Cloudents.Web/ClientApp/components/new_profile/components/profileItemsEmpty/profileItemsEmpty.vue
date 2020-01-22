<template>
   <div class="profileItemsEmpty">
      <div class="profileItemsEmpty_wrapper">
         <img class="mb-2 profileItemsEmpty_img" width="76" height="80" src="./empty.png" alt="">
         <div class="profileItemsEmpty_title" v-language:inner="'profile_empty_title'"/>
      </div>
   </div>
</template>

<script>
export default {
   name:'profileItemsEmpty',
}
</script>

<style lang="less">
@import '../../../../styles/mixin.less';
.profileItemsEmpty{
   width: 100%;
   border-radius: 8px;
   box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.15);
   background-color: #ffffff;
   text-align: center;
   padding: 28px;
   @media (max-width: @screen-xs) {
      padding: 20px;
      border-radius: 0px;
      box-shadow: none;
   }

   .profileItemsEmpty_wrapper{
      max-width: 490px;
      margin: 0 auto;
      .profileItemsEmpty_title{
         font-size: 16px;
         font-weight: 600;
         color: #bdc0d1;
      }
      .profileItemsEmpty_img{
         width: 76px;
         object-fit: contain;
      }
   }
}
</style>
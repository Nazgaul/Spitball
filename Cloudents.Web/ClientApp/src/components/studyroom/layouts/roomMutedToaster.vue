<template>
   <v-fade-transition>
      <div class="roomMutedToaster">
         <v-icon v-text="'sbf-mic-ignore'" class="icon" size="36" color="white"></v-icon>
         <p class="text">{{$t('roomMuted')}}</p>
      </div>
   </v-fade-transition>
</template>

<style lang="less">
.roomMutedToaster{
   position: absolute;
   top: calc(~"50% - 45px");
   left: calc(~"50% - 60px");
   width: 121px;
   height: 91px;
   border-radius: 6px;
   background-color: rgba(0, 0, 0, 0.5);
   text-align: center;
   padding: 12px;
   display: flex;
   flex-direction: column;
   justify-content: space-between;
   .text{
      opacity: 0.6;
      font-size: 13px;
      font-weight: 600;
      color: #ffffff;
      margin: 0;
      padding: 0;
   }
   .icon{
      opacity: 0.6;
   }
}
</style>
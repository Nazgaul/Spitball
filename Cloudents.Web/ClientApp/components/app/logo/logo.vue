<template>
    <div>
        <app-frymo v-if="isFrymo" class="logo frymo-logo"></app-frymo>
        <app-logo v-else class="logo"></app-logo>
    </div>
</template>

<script>
import AppLogo from "./logo-spitball.svg";
// import AppFrymo from "../../../../wwwroot/Images/frymo.svg";\
import AppFrymo from "./frymo-logo.svg";
export default {
    data() {
        return {
            isFrymo: global.siteName === 'frymo',
        }
    },
    components:{
        AppLogo,
        AppFrymo
    }
}
</script>

<style lang="less">
@import '../../../styles/mixin.less';
    .logo {
        width: 120px;
        height: 30px;
        fill: #43425D;
        vertical-align: bottom;
        @media (max-width: @screen-xs) {
            width: 94px;
            height: 22px;    
        }  
    }
     .frymo-logo {
        //width: 114px;
        //height: 48px;
        fill:#429DDB;
      @media (max-width: @screen-xs) {
          //height: 32px;
      }
    }
</style>
<template>
    <div>

        <app-frymo v-if="isFrymo" class="logo frymo-logo"></app-frymo>
        <app-logo v-else class="logo"></app-logo>
    
    
    </div>
</template>

<script>
import {mapGetters} from 'vuex';
import AppLogo from "../../../../wwwroot/Images/logo-spitball.svg";
// import AppFrymo from "../../../../wwwroot/Images/frymo.svg";\
import AppFrymo from "../../../../wwwroot/Images/frymo-logo.svg";
export default {
    components:{
        AppLogo,
        AppFrymo
    },
    computed:{
        ...mapGetters(['isFrymo']),
    }
}
</script>

<style lang="less">
@import '../../../styles/mixin.less';
    .frymo-logo {
        width: 114px;
        height: 48px;
        fill:#429DDB;
      @media (max-width: @screen-xs) {
          height: 32px;
      }
    }
</style>
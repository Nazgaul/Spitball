<template>
    <div>
        <app-frymo v-if="isFrymo" class="logo frymo-logo"></app-frymo>
        <component v-else :is="spLogo" class="logo"></component>
        <!-- <app-logo ></app-logo> -->
    </div>
</template>

<script>
import {mapGetters} from 'vuex';
import AppLogo from "./logo-spitball.svg";
import AppMobileLogo from './logo-m-spitball.svg'
import AppFrymo from "./frymo-logo.svg";
export default {
    props: {
        menuList: {
            type: Boolean,
            required: false
        }
    },
    components:{
        AppLogo,
        AppMobileLogo,
        AppFrymo
    },
    computed:{
        ...mapGetters(['isFrymo']),
        spLogo() {
            return this.$vuetify.breakpoint.xsOnly && !this.menuList
            ? 'AppMobileLogo'
            : 'AppLogo'
        }
    }
}
</script>

<style lang="less">
@import '../../../styles/mixin.less';
    .logo {
        // width: 120px;
        height: 30px;
        fill: #43425D !important; // vuetify upgrade to 2.3.2 add svg:not([fill]) to svg element
        vertical-align: bottom;
        @media (max-width: @screen-xs) {
            // width: 94px;
            height: 22px;
            // margin-top: 8px;
            margin-left: 10px;
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
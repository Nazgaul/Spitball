<template>
    <div>
        <app-frymo v-if="isFrymo" class="logo"></app-frymo>
        <app-logo v-else class="logo"></app-logo>

    </div>
</template>

<script>
import {mapGetters} from 'vuex';
import AppLogo from "../../../../wwwroot/Images/logo-spitball.svg";
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

</style>
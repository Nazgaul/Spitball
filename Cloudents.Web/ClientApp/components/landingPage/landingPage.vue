<template>
    <div class="crappyLandingPage">
        <top-nav></top-nav>
        <router-view/>
        <Footer></Footer>
    </div>
</template>

<script>   
import topNav from '../landingPageTools/TopNav.vue'
import Footer from '../landingPageTools/Footer.vue'
import {mapGetters} from 'vuex';
    export default {
        components:{
            topNav,
            Footer
        },
        computed:{
            ...mapGetters(['accountUser']),
        },
        created(){
        if(!!this.accountUser){
            const defaultSubmitRoute = {path: '/ask'};
            defaultSubmitRoute.query = this.$route.query;
            this.$router.push(defaultSubmitRoute);
        }
    }
}
</script>

<style src="./LandingPage.less" lang="less"></style>

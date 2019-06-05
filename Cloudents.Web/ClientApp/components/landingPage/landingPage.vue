<template>
    <div class="crappyLandingPage">
        <TopNav></TopNav>
        <router-view/>
        <Footer></Footer>
    </div>
</template>

<script>   
    import TopNav from './components/TopNav.vue';
    import Footer from './components/Footer.vue';

    export default {
        components: {
            TopNav,
            Footer,
        },
    }
</script>

<style src="./LandingPage.less" lang="less"></style>

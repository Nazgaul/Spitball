<template>
    <div>
        <top-nav></top-nav>
        <router-view/>
        <Footer></Footer>
    </div>
</template>

<script>   
import topNav from '../landingPageTools/TopNav.vue';
import Footer from '../landingPageTools/Footer.vue';
export default {
    components:{topNav,Footer}
}
</script>
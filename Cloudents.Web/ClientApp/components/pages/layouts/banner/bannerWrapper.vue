<template>
    <div>
        <div v-if="getBannerStatus">Hi Ram </div>
        <banner v-if="getBannerStatus"> </banner> 
    
    </div> 
</template>

<script>
const banner = () => import('./banner.vue');
import { mapGetters } from 'vuex';
export default {
    components: {
        banner
    },
    computed: {
        ...mapGetters(['getBannerStatus']),
    }
}
</script>
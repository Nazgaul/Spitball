<template>
   <v-flex xs12 class="signup-banner" :class="{'has-smart-banner': showSmartAppBanner}">
        <v-flex xs12>
            <button class="back-button">
                <v-icon right>sbf-close</v-icon>
            </button>
        </v-flex>
        <v-flex xs12 class="text-block">
            <p class="medium">Don’t miss the</p>
            <p class="large">Revolution</p>
            <p class="medium text-right">of the student economy</p>
            <p class="small">Join a community where you pay less and earn more for everything school-related</p>

            <div class="buttons">
                <router-link class="sign-up" to="register">Sign up</router-link>
                <router-link class="learn-more" to="">Learn More</router-link>
            </div>
            
        </v-flex>
    </v-flex>
</template>


<script>
    import {mapGetters} from 'vuex'
    export default {
        computed: {
            ...mapGetters(['showSmartAppBanner']),
        }

    }
</script>

<style src="./signup-banner.less" lang="less"></style>
<template>
    <v-flex xs12 class="signup-banner">
        <v-flex xs12>
            <button class="back-button" @click="hideRegistrationBanner()">
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
                <router-link class="learn-more" to="">Learn more</router-link>
            </div>

        </v-flex>
    </v-flex>
</template>


<script>
    import {mapActions} from 'vuex'

    export default {
        methods: {
            ...mapActions(['hideRegistrationBanner']),
        }

    }
</script>

<style src="./signup-banner.less" lang="less"></style>
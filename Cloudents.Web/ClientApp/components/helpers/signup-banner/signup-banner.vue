<template>
    <v-flex xs12 class="banner-wrap">
        <div class="overlay-bubble">
            <div class="overlay-bubble-1"></div>
            <div class="overlay-bubble-2"></div>
            <div class="overlay-bubble-3"></div>
            <div class="overlay-bubble-4"></div>
            <div class="overlay-bubble-5"></div>
            <div class="overlay-bubble-6"></div>
        </div>
        <vue-particles
                :class="'signup-banner color-'+$route.path.slice(1)"
                 color="#dedede"
                :particleOpacity="0.7"
                :particlesNumber="80"
                shapeType="star"
                :particleSize="4"
                linesColor="#dedede"
                :linesWidth="1"
                :lineLinked="true"
                :lineOpacity="0.4"
                :linesDistance="150"
                :moveSpeed="3"
                :hoverEffect="true"
                hoverMode="grab"
                :clickEffect="true"
                clickMode="push"
        >
        </vue-particles>

        <!--<button class="back-button" @click="hideRegistrationBanner()">-->
            <!--<v-icon>sbf-close</v-icon>-->
        <!--</button>-->
        <!--<v-flex xs12 class="text-block">-->
            <!--<div class="inner-text-block">-->
                <!--<div class="text-wrap">-->
                    <!--<p class="medium">Don’t miss the</p>-->
                    <!--<p class="large">Revolution</p>-->
                    <!--<p class="medium text-right">of the student economy</p>-->
                    <!--<p class="small">Join a community where you pay less and earn more for everything-->
                        <!--school-related.</p>-->
                    <!--<div class="buttons">-->
                        <!--<router-link class="sign-up" to="register">Sign up</router-link>-->
                        <!--<router-link class="learn-more" :to="{name:'about'}">Learn more</router-link>-->
                    <!--</div>-->
                <!--</div>-->
            <!--</div>-->
        <!--</v-flex>-->
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
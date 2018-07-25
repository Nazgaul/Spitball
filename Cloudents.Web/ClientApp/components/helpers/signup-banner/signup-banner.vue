<template>
    <v-flex xs12 :class="'banner-wrap color-'+$route.path.slice(1)">
        <button class="back-button" @click="hideRegistrationBanner()">
            <v-icon>sbf-close</v-icon>
        </button>
        <div class="overlay-bubble">
            <div class="overlay-bubble-1"></div>
            <div class="overlay-bubble-2"></div>
            <div class="overlay-bubble-3"></div>
            <div class="overlay-bubble-4"></div>
            <div class="overlay-bubble-5"></div>
            <div class="overlay-bubble-6"></div>
        </div>
        <vue-particles style="z-index:100" :key="bannerData.lineColor"
                       class="signup-banner"
                       :color="bannerData.lineColor"
                       :particleOpacity="0.7"
                       :particlesNumber="80"
                       shapeType="circle"
                       :particleSize="4"
                       :linesColor="bannerData.lineColor"
                       :linesWidth="1"
                       :lineLinked="true"
                       :lineOpacity="0.4"
                       :linesDistance="150"
                       :moveSpeed="3"
                       :hoverEffect="true"
                       hoverMode="grab"
                       :clickEffect="true"
                       clickMode="push"
        > </vue-particles>

        <v-flex  class="text-block">
        <div class="title-wrap">
        <h2 class="large">{{bannerData.title}}</h2>
        </div>
            <div class="text-wrap">
        <p class="medium">{{ bannerData.textMain }}</p>
                <!--<p class="medium">{{ bannerData.textSub }}</p>-->
        </div>
        </v-flex>
        <div class="round-free">
            <p class="round-free-text">Spitball is a completely<br/>
               <span>FREE</span><br/> service.</p>
        </div>
    </v-flex>
</template>

<style src="./signup-banner.less" lang="less"></style>
<script src="./signup-banner.js"></script>

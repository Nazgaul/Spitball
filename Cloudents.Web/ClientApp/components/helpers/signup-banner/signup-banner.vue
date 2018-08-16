<template>
    <v-flex xs12 :class="'banner-wrap campaign-'+ bannerData.campaign +' color-'+$route.path.slice(1) ">
        <button class="back-button" @click="hideRegistrationBanner()">
            <v-icon>sbf-close</v-icon>
        </button>
        <div class="overlay-bubble" v-if="bannerData.showOverlay">
            <div class="overlay-bubble-1"></div>
            <div class="overlay-bubble-2"></div>
            <div class="overlay-bubble-3"></div>
            <div class="overlay-bubble-4"></div>
            <div class="overlay-bubble-5"></div>
            <div class="overlay-bubble-6"></div>
        </div>
        <particles
                :linesColor="bannerData.lineColor"
                :key="bannerData.lineColor"
                class="signup-banner particles-js">
         </particles>

        <v-flex  class="text-block">
        <div class="title-wrap">
        <h2 class="large">{{bannerData.title}}</h2>
        </div>
            <div class="text-wrap">
        <p class="medium" v-html="$options.filters.bolder(bannerData.textMain, bannerData.boldText)"></p>
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

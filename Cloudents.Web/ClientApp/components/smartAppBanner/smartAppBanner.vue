<template>
    <transition name="slide-fade">
        <div class="smart-app-banner elevation-1">
            <button class="close-btn" @click="close()">
                <v-icon right>sbf-close</v-icon>
            </button>
            <div class="content">
                <div class="data">
                    <logo></logo>
                    <div class="app-info">
                        <div class="app-name">Spitball: Simplify School</div>
                        <div class="app-rating"></div>
                        <div class="app-price">{{appData.priceText}}</div>
                    </div>
                </div>
                <a class="app-btn" target="_blank" :href="appData.appUrl">View</a>
            </div>
        </div>
    </transition>
</template>

<script>
    import logo from './svg/logo.svg';
    import {mapActions} from 'vuex'

    const appsData = {
        ios: {
            priceText: 'FREE - On the App Store',
            appUrl: 'https://itunes.apple.com/us/app/spitball-simplify-school/id990911114?mt=8'
        },
        android: {
            priceText: 'FREE - in google play',
            appUrl: 'https://play.google.com/store/apps/details?id=com.cloudents.spitball'
        }
    }
    export default {
        components: {
            logo
        },
        data() {
            return {appData: {}}
        },
        methods: {
            ...mapActions(['hideSmartAppBanner']),
            close() {
                this.hideSmartAppBanner();
            }
        },
        computed: {},
        created() {
            if (/iPhone|iPad|iPod/i.test(window.navigator.userAgent)) {
                this.appData = appsData.ios;
            } else if (/Android/i.test(window.navigator.userAgent)) {
                this.appData = appsData.android;
            }
        }

    }
</script>

<style lang="less" src="./smartAppBanner.less"></style>
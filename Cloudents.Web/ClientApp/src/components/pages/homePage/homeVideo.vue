<template>
    <div class="homeVideo">
        <video
            ref="video"
            poster="https://az32006.vo.msecnd.net/spitball-user/home-page/poster.png"
            class="video"
            loop
            autoplay
            muted
            playsinline 
        >
            <source src="https://az32006.vo.msecnd.net/spitball-user/home-page/websitemovie_Trim.mp4" type="video/mp4">
        </video>

        <div class="actions">
            <h1 class="text" v-t="'homePage_video_title'"></h1>
            <homeButtons :action="startLearn" :country="country" v-if="!$vuetify.breakpoint.xsOnly" />
        </div>
        <div class="videoLinear"></div>
    </div>
</template>

<script>
import homeMixin from './homeMixin'

const homeButtons = () => import('./homeButtons.vue')

export default {
    mixins: [homeMixin],
    components:{
        homeButtons
    },
    computed: {
        country() {
            if(global.country === 'IL') {
                return 'lang=he'
            }
            return ''
        }
    },
    mounted() {
        let playPromise = this.$refs.video.play()
        let self = this
        
        if (playPromise !== undefined) {
            playPromise.then(() => {}).catch(error => {
                self.$appInsights.trackException(error)
            });
        }
    }
}
</script>

<style lang="less">
    @import "../../../styles/mixin.less";
  .homeVideo {
    position: relative;
    height: 730px;
    @media (max-width: @screen-xs) {
      height: 354px;
    }
    .video {
      width: 100%;
      object-fit: cover;
      height: inherit;
    }
    .actions {
      color: #fff;
      position: absolute;
      bottom: 50px;
      right: 0;
      left: 0;
      text-align: center;
      z-index: 1;
      @media (max-width: @screen-xs) {
        bottom: 20px;
      }
      .text {
        font-size: 34px;
        width: 600px;
        font-weight: normal;
        line-height: 1.5;
        margin: 0 auto 50px;
        @media (max-width: @screen-xs) {
          width: 100%;
          max-width: 320px;
          margin: 0 auto;
          padding: 0 10px;
          font-size: 22px;
        }
      }
    }
    .videoLinear {
      position: absolute;
      bottom: 0;
      right: 0;
      left: 0;
      height: 100%;
      background-image: linear-gradient(to bottom, rgba(0, 0, 0, 0), rgba(0, 0, 0, 0.06), rgba(0, 0, 0, 0.26), rgba(0, 0, 0, 0.89));
    }
  }
</style>
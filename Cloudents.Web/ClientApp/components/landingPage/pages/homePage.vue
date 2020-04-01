<template>
    <div class="homePage">

        <div class="homeVideo">
            <video
              ref="video"
              poster="../images/Spitball_original_homepage_poster.jpg"
              class="video"
              loop
              autoplay
              muted
            >
                <source src="https://az32006.vo.msecnd.net/spitball-user/Spitball-HP-video.mp4" type="video/mp4">
            </video>

            <div class="actions">
                <div class="text" v-t="'homePage_video_title'"></div>
                <homeButtons :action="actions" v-if="!$vuetify.breakpoint.xsOnly" />
            </div>
        </div>

        <div class="middle d-sm-flex d-block text-center mb-6 mb-sm-12 pa-4 pt-0 pa-sm-0">
            <homeButtons :action="actions" v-if="$vuetify.breakpoint.xsOnly" />
            <div class="boxStudent">
                <div class="boxHeader" v-t="'homePage_student'"></div>
                <div class="boxDesc mt-3 mb-6" v-t="'homePage_student_text'"></div>
                <button class="boxBtns btnsLearn mb-4" @click="studentRegister" v-t="'homePage_btn_register'"></button>
                <div class="boxNoCredit" v-t="'homePage_credit'"></div>
            </div>
            <div>
                <div class="boxHeader" v-t="'homePage_teacher'"></div>
                <div class="boxDesc mt-3 mb-6" v-t="'homePage_teacher_text'"></div>
                <button class="boxBtns btnsTeach mb-4" @click="teacherRegister" v-t="'homePage_btn_register'"></button>
                <div class="boxNoCredit" v-t="'homePage_credit'"></div>
            </div>
        </div>

    </div>
</template>

<script>
const homeButtons = () => import('./homeButtons.vue')

export default {
      components:{
        homeButtons
    },
    data() {
      return {
        actions: {
          startLearn: () => this.startLearn,
          startTeach: () => this.startTeach
        }
      }
    },
    methods: {
      studentRegister() {
        this.$store.commit('setComponent', 'register')
      },
      teacherRegister() {
        this.$store.commit('setComponent', 'registerTeacher')
      },
      startLearn() {
        //
      },
      startTeach() {
        //
      }
    },
    mounted() {
      let playPromise = this.$refs.video.play();
      if (playPromise !== undefined) {
        playPromise.then(() => {}).catch(error => {
          console.log(error);
          self.$appInsights.trackException({exception: new Error(error)});
        });
      }
    }
};
</script>

<style lang="less">
@import "../../../styles/mixin.less";
.homePage {
  background: #fff;
  .homeVideo {
    position: relative;
    height: 636px;
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
      @media (max-width: @screen-xs) {
        bottom: 20px;
      }
      .text {
        font-size: 34px;
        width: 600px;
        line-height: 1.5;
        margin: 0 auto 60px;
        @media (max-width: @screen-xs) {
          margin: 0;
          width: 100%;
          font-size: 22px;
        }
      }
 
    }
  }

  .middle {
    .responsive-property(margin-top, 45px, null, 28px);
    color: @global-purple;
    display: flex;
    justify-content: space-evenly;
    .boxStudent {
      @media (max-width: @screen-xs) {
        border-top: 1px solid #ddd;
        margin: 30px 0;
        padding-top: 30px;
      }
    }
    .boxHeader {
      font-size: 32px;
    }
    .boxDesc {
      line-height: 1.6;
      .responsive-property(max-width, 380px, null, 100%);
      .responsive-property(font-size, 20px, null, 16px);
    }
    .boxBtns {
      border-radius: 28px;
      border: solid 1px #4c59ff;
      width: 230px;
      height: 44px;
      font-size: 16px;
      &.btnsLearn {
        color: #4c59ff;
        font-weight: 600;
        border: solid 1px #4c59ff;
      }
      &.btnsTeach {
        color: #41c4bc;
        font-weight: 600;
        border: solid 1px #41c4bc;
      }
    }
    .boxNoCredit {
      color: #a8a8ac;
    }
  }
}
</style>
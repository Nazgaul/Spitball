<template>
  <div class="landingPageHP">
      <!-- <homePageVideo></homePageVideo> -->
      <!-- <headlineSection></headlineSection>
      <categoriesSection></categoriesSection>
      <tutorsSection></tutorsSection>
      <itemsSection></itemsSection>
      <testimonialsSection></testimonialsSection>
      <earnSection></earnSection> -->


      <div class="homeVideo">
        <video ref="video" class="video" height="636" data-object-position="50% 10%" autoplay loop data-object-fit="cover">
          <!-- <source src="https://az32006.vo.msecnd.net/spitball-user/Spitball-HP-video.mp4" type="video/mp4"> -->
        </video>

        <div class="actions">
          <div class="text">The Platform That Connects Teachers With Learners</div>
          <div class="buttons">
            <button class="btn btnLearn">Start Learning</button>
            <button class="btn btnTeach">Start Teaching</button>
          </div>
        </div>
      </div>

      <div class="middle d-flex text-center mb-12">
        <div class="studentAction">
          <div class="studentText header">Student</div>
          <div class="desc mt-3 mb-6">Access course specific study materials, talk to expert teachers or watch educational videos.</div>
          <button class="btns btnsLearn mb-4">Register for Free</button>
          <div class="noCredit">No credit card required</div>
        </div>
        <div class="teacherAction">
          <div class="teacherText header">Teacher</div>
          <div class="desc mt-3 mb-6">Your teaching skills are unique. We provide you with a beautiful site to recruit and teach students</div>
          <button class="btns btnsTeach mb-4">Register for Free</button>
          <div class="noCredit">No credit card required</div>
        </div>
      </div>

  </div>
</template>

<script>
// import headlineSection from '../components/headlineSection.vue';
// import categoriesSection from '../components/categoriesSection.vue';
// import tutorsSection from '../components/tutorsSection.vue';
// import itemsSection from '../components/itemsSection.vue';
// import testimonialsSection from '../components/testimonialsSection.vue';
// import earnSection from '../components/earnSection.vue'

// import homePageVideo from 

export default {
      components:{
        // headlineSection,
        // categoriesSection,
        // tutorsSection,
        // itemsSection,
        // testimonialsSection,
        // earnSection
    },
    mounted() {
      var playPromise = this.$refs.video.play();
      if (playPromise !== undefined) {
        playPromise.then(_ => {
          // Automatic playback started!
          // Show playing UI.
        })
        .catch(error => {
          // Auto-play was prevented
          // Show paused UI.
        });
      }
    }
};
</script>

<style lang="less">
@import "../../../styles/mixin.less";
.landingPageHP {
  background: #fff;
  .homeVideo {
    position: relative;
    height: 636px;
    background-position: 50% 10%;
    object-fit: cover;
    background-image: url('../images/Spitball_original_homepage_poster.jpg');
    .video {
      width: 100%;
      object-fit: cover;
      flex: 1;
      min-height: 100%;
      min-width: 100%;

    }
    .actions {
      color: #fff;
      position: absolute;
      bottom: 50px;
      right: 0;
      left: 0;
      text-align: center;

      .text {
        font-size: 34px;
        padding: 0 33%;
        margin-bottom: 60px;
      }
      .buttons {
        display: flex;
        justify-content: space-evenly;
        .btn {
          outline: none;
          width: 230px;
          height: 44px;
          border-radius: 28px;
          font-size: 16px;
          &.btnLearn {
            background-color: #4c59ff;
          }
          &.btnTeach {
            margin-left: 100px;
            background-color: #41c4bc;
          }
          
        }
      }
    }
  }
  .middle {
    margin-top: 45px;
    color: @global-purple;
    display: flex;
    justify-content: space-evenly;
    .header {
      font-size: 32px;
    }
    .desc {
      line-height: 1.6;
      font-size: 20px;
      max-width: 380px;
    }
    .btns {
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
    .noCredit {
      color: #a8a8ac;
    }
  }
}
</style>
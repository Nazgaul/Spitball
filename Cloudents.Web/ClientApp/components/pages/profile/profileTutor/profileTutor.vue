<template>
    <div id="profileTutor">

        <profileTabs />

        <div class="bgImage"></div>

        <profileTutorBox />
        <profileBookSession />
        <profileVideo />
        <profileCarousel />
        <profileQuestions />
        <profileTestimonial />

    </div>
</template>

<script>
const profileTabs = () => import('./profileTabs.vue');
const profileTutorBox = () => import('./profileTutorBox.vue');
const profileBookSession = () => import('./profileBookSession.vue');
const profileVideo = () => import('./profileVideo.vue');
const profileCarousel = () => import('./profileCarousel.vue');
const profileQuestions = () => import('./profileQuestions.vue');
const profileTestimonial = () => import('./profileTestimonial.vue');

export default {
    components: {
        profileTabs,
        profileTutorBox,
        profileBookSession,
        profileVideo,
        profileCarousel,
        profileQuestions,
        profileTestimonial,
    }
}
</script>

<style lang="less">
   #profileTutor {


       .bgImage {
            background-image: url('../../../landingPage/images/earn.png');
            background-size: cover;
            background-position: center;
            height: 400px;
       }

   }
</style>
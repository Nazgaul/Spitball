<template>
    <div class="homeQuote pa-2 text-center">
        <homeButtons :action="startLearn" v-if="$vuetify.breakpoint.xsOnly" />
        <blockquote class="quote mb-3">
            Spitball helps students learn and teachers teach. Just enter the site and start a real-time lesson, no need for apps or leave your house
        </blockquote>
        <div class="teacher d-flex">
            <router-link class="userLink px-2 text-truncate" to="/">idan kallmann</router-link>
            <span class="userTeach px-2 text-truncate">Statistics teachers</span>
        </div>
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
}
</script>

<style lang="less">
@import "../../../styles/mixin.less";
@import "../../../styles/colors.less";

.homeQuote {
    max-width: 700px;
    width: 100%;
    margin: 80px auto;
    @media (max-width: @screen-xs) {
        margin: 20px auto;
    }
    .quote {
        .responsive-property(font-size, 24px, null, 20px);
        position: relative;
        color: @global-purple;
        line-height: 2;
        &:before {
            content: '\201C';
            width: 100%;
            height: 0;
            font-family: serif;
            font-style: normal;
            font-weight: 700;
            position: absolute;
            font-size: 175px;
            top: -140px;
            right: 48%; // for responsive quote
            color: rgba(200,200,200,.4);

            @media (max-width: @screen-xs) {
                left: 0;
            }
        }
    }
    .teacher {
        .userLink {
            border-right: 1px solid #8c8b9b;
            color: @global-auth-text;
            font-weight: 600;
            max-width: 200px;
            margin-left: auto;
        }
        .userTeach {
            color: #a8a8ac;
            margin-right: auto;
            max-width: 200px;
        }
    }
    .homeBtns {
        margin-bottom: 60px;
    }
}
</style>
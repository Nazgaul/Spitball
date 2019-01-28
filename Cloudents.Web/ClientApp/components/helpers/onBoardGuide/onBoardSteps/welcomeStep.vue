<template>
    <div class="onboard-step-wrap">
        <div class="title-row"  v-show="$vuetify.breakpoint.smAndUp">
          <span class="step-title"  v-language:inner>onboard_welcome_title</span>
            <span class="step-subtitle"  v-language:inner>onboard_welcome_subtitle</span>
        </div>
        <div class="title-row" v-show="$vuetify.breakpoint.xsOnly">
            <span class="step-title" v-language:inner>onboard_welcome_title</span>
        </div>
        <div class="images-row" >
            <div class="left-block" v-if="!$vuetify.breakpoint.xsOnly">
                <div class="img-wrap">
                    <bell-image  class="block-image"></bell-image>
                </div>
                <div class="text-wrap">
                    <span class="bottom-title" v-language:inner>onboard_welcome_bottom_title_learn</span>
                    <span  class="bottom-subtitle" v-language:inner>onboard_welcome_bottom_subtitle_learn</span>
                </div>
            </div>
            <div class="right-block">
                <div class="img-wrap">
                    <list-image  class="block-image"></list-image>
                </div>
                <div class="text-wrap" v-show="$vuetify.breakpoint.smAndUp">
                    <span class="bottom-title" v-language:inner>onboard_welcome_bottom_title_earn</span>
                    <span class="bottom-subtitle" v-language:inner>onboard_welcome_bottom_subtitle_earn</span>
                </div>
                <div class="text-wrap" v-show="$vuetify.breakpoint.xsOnly">
                    <span class="bottom-subtitle" v-language:inner>onboard_welcome_subtitle</span>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
    import listImage from '../images/list.svg';
    import bellImage from '../images/earn_1st_step.svg'
    export default {
        components: {listImage, bellImage},
        name: "welcomeStep"
    }
</script>

<style lang="less">
@import "./onBoardSteps.less";


</style>
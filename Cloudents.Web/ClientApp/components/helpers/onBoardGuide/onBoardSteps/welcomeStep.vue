<template>
    <div class="onboard-step-wrap">
        <div class="title-row">
          <span class="step-title"  v-language:inner>onboard_welcome_title</span>
        </div>
        <div class="images-row align-center justify-center" >
            <div class="right-block">
                <div class="img-wrap">
                    <welcome-svg  class="block-image"></welcome-svg>
                </div>
                <div class="text-wrap">
                    <span class="bottom-text" v-language:inner>onboard_welcome_text</span>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
    import welcomeSvg from '../images/SVG_step_1.svg';
    export default {
        components: {welcomeSvg},
        name: "welcomeStep"
    }
</script>

<style lang="less">
@import "./onBoardSteps.less";


</style>
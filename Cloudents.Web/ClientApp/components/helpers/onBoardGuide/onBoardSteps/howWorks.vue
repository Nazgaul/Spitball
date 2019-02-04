<template>
    <div class="onboard-step-wrap">
        <div class="title-row">
            <span class="step-title" v-language:inner>onboard_how_title</span>
        </div>

        <div class="images-row align-center justify-center" >
            <div class="right-block ">
                <div class="img-wrap">
                    <howSvg  class="block-image"></howSvg>
                </div>
                <div class="text-wrap">
                    <span class="bottom-text" v-language:inner>onboard_how_text</span>
                </div>
            </div>
        </div>
    </div>
</template>


<script>

    import howSvg from '../images/SVG_step_2.svg'
    export default {
        components: {howSvg},
        name: "howWorks"
    }
</script>

<style lang="less">
    @import "./onBoardSteps.less";

</style>
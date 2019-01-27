<template>
    <div class="onboard-step-wrap">
        <div class="title-row"  v-show="$vuetify.breakpoint.smAndUp">
            <span class="step-title" v-language:inner>onboard_earn_title</span>
            <span class="step-subtitle" v-language:inner>onboard_earn_subtitle</span>
        </div>
        <div class="title-row" v-show="$vuetify.breakpoint.xsOnly">
            <span class="step-title" v-language:inner>onboard_earn_title</span>
            <span class="step-subtitle" v-language:inner>onboard_earn_subtitle</span>
        </div>
        <div class="images-row" >
            <div class="left-block" v-if="!$vuetify.breakpoint.xsOnly">
                <div class="img-wrap">
                    <earn-a class="block-image"></earn-a>
                </div>
                <div class="text-wrap">
                    <span class="bottom-title" v-language:inner>onboard_earn_bottom_title_one</span>
                    <span  class="bottom-subtitle" v-language:inner>onboard_earn_bottom_subtitle_one</span>
                </div>
            </div>
            <div class="right-block">
                <div class="img-wrap">
                    <earn-b class="block-image"></earn-b >
                </div>
                <div class="text-wrap" v-show="$vuetify.breakpoint.smAndUp">
                    <span class="bottom-title" v-language:inner>onboard_earn_bottom_title_two</span>
                    <span class="bottom-subtitle" v-language:inner>onboard_earn_bottom_subtitle_two</span>
                </div>
                <div class="text-wrap" v-show="$vuetify.breakpoint.xsOnly">
                    <span class="bottom-subtitle" v-language:inner>onboard_earn_bottom_title_mobile</span>
                </div>
            </div>
        </div>
    </div>
</template>


<script>
    import earnA from '../images/Earn_3rd_step_a.svg';
    import earnB from '../images/Earn_3rd_step_b.svg'
    export default {
        components: { earnA, earnB},
        name: "earnStep"
    }
</script>

<style lang="less">
    @import "./onBoardSteps.less";

</style>
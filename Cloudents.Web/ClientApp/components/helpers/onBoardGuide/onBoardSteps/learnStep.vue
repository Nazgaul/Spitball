<template>
    <div class="onboard-step-wrap">
        <div class="title-row"  v-show="$vuetify.breakpoint.smAndUp">
            <span class="step-title" v-language:inner>onboard_learn_title</span>
            <span class="step-subtitle" v-language:inner>onboard_learn_subtitle</span>
        </div>
        <div class="title-row" v-show="$vuetify.breakpoint.xsOnly">
            <span class="step-title"  v-language:inner>onboard_learn_title</span>
            <span class="step-subtitle" v-language:inner>onboard_learn_subtitle</span>
        </div>
        <div class="images-row" >
            <div class="left-block" v-if="!$vuetify.breakpoint.xsOnly">
                <div class="img-wrap">
                    <learn-symbols  class="block-image"></learn-symbols>
                </div>
                <div class="text-wrap">
                    <span class="bottom-title" v-language:inner>onboard_learn_bottom_title_one</span>
                    <span  class="bottom-subtitle"  v-language:inner>onboard_learn_bottom_subtitle_one</span>
                </div>
            </div>
            <div class="right-block">
                <div class="img-wrap">
                    <learn-clock  class="block-image"></learn-clock>
                </div>
                <div class="text-wrap" v-show="$vuetify.breakpoint.smAndUp">
                    <span class="bottom-title" v-language:inner>onboard_learn_bottom_title_two</span>
                    <span class="bottom-subtitle" v-language:inner>onboard_learn_bottom_subtitle_two</span>
                </div>
                <div class="text-wrap" v-show="$vuetify.breakpoint.xsOnly">
                    <span class="bottom-subtitle" v-language:inner>onboard_learn_bottom_subtitle_mobile</span>
                </div>
            </div>
        </div>
    </div>
</template>


<script>
    import learnSymbols from '../images/symbols.svg';
    import learnClock from '../images/Learn_2st_step_b.svg'
    export default {
        components: {learnSymbols, learnClock},
        name: "learnStep"
    }
</script>

<style lang="less">
    @import "./onBoardSteps.less";

</style>
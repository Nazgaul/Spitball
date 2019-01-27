<template>
    <div class="onboard-step-wrap">
        <div class="title-row"  v-show="$vuetify.breakpoint.smAndUp">
            <span class="step-title">Welcome to Spitball</span>
            <span class="step-subtitle">The place for students to learn and earn</span>
        </div>
        <div class="title-row" v-show="$vuetify.breakpoint.xsOnly">
            <span class="step-title">Welcome to Spitball</span>
        </div>
        <div class="images-row" >
            <div class="left-block" v-if="!$vuetify.breakpoint.xsOnly">
                <div class="img-wrap">
                    <list-image  class="block-image"></list-image>
                </div>
                <div class="text-wrap">
                    <span class="bottom-title">Learn</span>
                    <span  class="bottom-subtitle">Improve your grades, with access to study resources and homework help</span>
                </div>
            </div>
            <div class="right-block">
                <div class="img-wrap">
                    <list-image  class="block-image"></list-image>
                </div>
                <div class="text-wrap" v-show="$vuetify.breakpoint.smAndUp">
                    <span class="bottom-title">Earn</span>
                    <span class="bottom-subtitle">Earn money by sharing your class notes and answering questions</span>
                </div>
                <div class="text-wrap" v-show="$vuetify.breakpoint.xsOnly">
                    <span class="bottom-subtitle">The place for students to learn & earn</span>
                </div>
            </div>
        </div>
    </div>
</template>


<script>
    export default {
        name: "earnStep"
    }
</script>

<style lang="less">
    @import "./onBoardSteps.less";

</style>
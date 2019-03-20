<template>
    <v-layout class="bio-text-container" column>
        <v-flex>
            <h4 class="intro-name">Hey there, I’m Ben!</h4>
        </v-flex>
        <v-flex class="pt-3">
            <span class="bio-about-intro">
                Some message i want to pass on to students, so they would identify with me. it might take up to two text lines.
            </span>
        </v-flex>
    </v-layout>
</template>

<script>
    export default {
        name: "userAboutMessage"
    }
</script>

<style lang="less">
    @import '../../../../../styles/mixin.less';
    .bio-text-container{
        .intro-name{
            font-family: @fontOpenSans;
            font-size: 30px;
            font-weight: 600;
            line-height: 0.57;
            color: @profileTextColor;
        }
        .bio-about-intro{
            font-family: @fontOpenSans;
            font-size: 22px;
            font-style: italic;
            line-height: 1.36;
            letter-spacing: normal;
            color: @profileTextColor;
        }
    }

</style>
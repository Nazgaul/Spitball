<template>
    <v-layout v-if="!isNotFound" column align-center justify-center class="error-permisson-wrap">
        <v-flex class="mb-2" xs12 sm12 md12>
            <p>
                <span v-language:inner>tutor_quality_allowed_title</span>
                <span class="font-weight-bold" v-language:inner>tutor_quality_allowed_press</span>
            </p>
        </v-flex>
        <v-flex xs6 sm6 md6 class="mb-5">
            <img class="enable-explanation" src="../../../images/chrome-camera-reset.gif" alt="">
        </v-flex>
    </v-layout>

    <v-layout v-else column align-center justify-center class="error-permisson-wrap">
        <!-- <v-flex class="mb-4">
            <inputCableImage class="input-icon"></inputCableImage>
        </v-flex> -->
        <v-flex class="mb-5">
            <!--<h3>Unable to access device.</h3>-->
            <p class="font-weight-bold" style="font-size: 16px;" v-language:inner>tutor_quality_allowed_looks_like</p>
            <div v-language:inner>tutor_quality_allowed_tip_one</div>
            <p v-language:inner style="font-size: 12px;margin-top:10px;max-width: 80%;">tutor_quality_allowed_tip_two</p>
            <!-- <ol class="numbered pl-3">
                <li v-language:inner></li>
                <li v-language:inner></li>
            </ol> -->

        </v-flex>
    </v-layout>
</template>

<script>
    // import inputCableImage from '../../../images/input.svg';

    export default {
        name: "notAllowed",
        // components: {inputCableImage},
        props: {
            isNotFound: {
                type: Boolean,
                default: false,
                required: true
            },
        },
    };
</script>

<style  lang="less">
    .error-permisson-wrap {
        .enable-explanation {
            width: 100%;
            max-width: 368px;
            border-radius: 4px;
            box-shadow: 0 3px 8px 0 rgba(0, 0, 0, 0.16);
            border: solid 6px #ffffff;
        }
        .input-icon {
            height: 40px;
            width: 40px;
            fill: #a8a8a8;
        }
        img {
            width: 100%;
            height: auto;
        }
    }

</style>
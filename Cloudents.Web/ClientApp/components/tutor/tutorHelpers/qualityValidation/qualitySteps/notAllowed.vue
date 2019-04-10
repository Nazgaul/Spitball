<template>
    <v-layout v-if="!isNotFound" column align-center justify-center class="error-permisson-wrap">
        <v-flex class="mb-2" xs12 sm12 md12>
            <p>
                To communicate with the other participant, allow your web browser to access the camera and microphone.
                <span class="font-weight-bold">Press Refresh after allowing access.</span>
            </p>
        </v-flex>
        <v-flex xs6 sm6 md6 class="mb-5">
            <img class="enable-explanation" src="../../../images/chrome-camera-reset.gif" alt="">
        </v-flex>
    </v-layout>

    <v-layout v-else column align-center justify-center class="error-permisson-wrap">
        <v-flex class="mb-4">
            <inputCableImage class="input-icon"></inputCableImage>
        </v-flex>
        <v-flex class="mb-5">
            <!--<h3>Unable to access device.</h3>-->
            <p class="font-weight-bold">It looks like one of your devices is in use by another application, or is
                disabled in your system settings.</p>
            <ol class="numbered pl-3">
                <li>Make sure your camera and microphone are enabled in your system settings</li>
                <li>Close any other tabs or applications, and reconnect.</li>
            </ol>

        </v-flex>
    </v-layout>
</template>

<script>
    import inputCableImage from '../../../images/input.svg';

    export default {
        name: "notAllowed",
        components: {inputCableImage},
        props: {
            isNotFound: {
                type: Boolean,
                default: false,
                required: true
            },
        },
    };
</script>

<style scoped lang="less">
    .error-permisson-wrap {
        /*rtl:begin:ignore*/
        direction: ltr;
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
        /*rtl:end:ignore*/
    }

</style>
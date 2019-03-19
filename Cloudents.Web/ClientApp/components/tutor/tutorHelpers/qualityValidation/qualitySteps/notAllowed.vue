<template>
    <v-container class="error-permisson-wrap">
        <v-layout v-if="!isNotFound" >
            <v-flex xs12 sm8 md8>
                <img style="width: 100%;" src="../images/chrome-camera-reset.gif" alt="">
            </v-flex>
            <v-flex class="ml-2" xs12 sm4 md4>
                <h4>Allow browser permissions.</h4>
                <p>
                    To communicate with the other participant, allow your web browser to access the camera and microphone.
                </p>
                <p>
                    You will have to refresh your browser after allowing access.
                </p>
            </v-flex>
        </v-layout>
        <v-layout v-else>
            <v-flex>
                <h3>Unable to access device.</h3>
                <p>It looks like one of your devices is in use by another application, or is disabled in your system settings.</p>
                <p>Make sure your camera and microphone are enabled in your system settings, close any other tabs or applications, and reconnect.</p>
            </v-flex>
        </v-layout>
    </v-container>

</template>

<script>
    export default {
        name: "notAllowed",
        props: {
            isNotFound: {
                type: Boolean,
                default: false,
                required: true
            },
        },
    }
</script>

<style scoped lang="less">
    .error-permisson-wrap{
        img{
            width: 100%;
            height: auto;
        }
    }

</style>
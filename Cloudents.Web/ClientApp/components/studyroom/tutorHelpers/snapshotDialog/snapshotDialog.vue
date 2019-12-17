<template>
    <div class="snapshot-dialog-wrap pb-3">
        <v-layout row class="pt-3">
            <v-flex xs12 class="text-xs-right px-3">
                <v-icon class="caption cursor-pointer" @click="closeDialog()">sbf-close</v-icon>
            </v-flex>
        </v-layout>
        <v-layout column align-center>
            <v-flex xs12 class="pt-12">
                <span class="subheading font-weight-bold" v-language:inner="'tutor_take_snapshot_title'"></span>
            </v-flex>
            <v-flex xs12 style="text-align: center;" class="pt-2">
                <span v-language:inner="'tutor_take_snapshot_message'"></span>
            </v-flex>
            <v-flex v-show="!noCameraError" xs12 style="text-align: center;" class="pt-2">
               <video autoplay="true" id="videoElement"></video>
               <canvas id="snapshot" width=800 height=600 style="display:none;"></canvas>
            </v-flex>
            <v-flex v-show="noCameraError" xs12 style="text-align: center;" class="pt-2">
               <span v-language:inner="'tutor_take_snapshot_error'"></span>
            </v-flex>
            <v-flex xs12 class="pt-4">
                <v-btn class="accept-consent-btn elevation-0 align-center justify-center" @click="takeSnapshot()">
                    <span class="text-capitalize" v-language:inner="'tutor_take_snapshot_btn'"></span>
                </v-btn>
            </v-flex>
        </v-layout>
    </div>
</template>

<script>
    import { mapActions } from 'vuex';
    import studyRoomRecordingService from '../../studyRoomRecordingService';
    import {LanguageService} from "../../../../services/language/languageService";

    export default {
        data() {
            return {
                noCameraError: false,
            };
        },
        methods: {
            ...mapActions(['setSnapshotDialog']),
            drawImageToCanvas(){
                let context = document.getElementById("snapshot").getContext('2d');
                let player = document.getElementById("videoElement");
                context.drawImage(player, 0, 0, 800, 600);
            },
            downloadImg(){
                let a = document.createElement("a");
                document.body.appendChild(a);
                a.style = "display: none";
                let context = document.getElementById("snapshot").getContext('2d');
                let url = context.canvas.toDataURL("image/png");
                a.href = url;
                a.download = 'img.png';
                a.click();
            },
            takeSnapshot() {
                this.drawImageToCanvas();
                this.downloadImg();
            },
            closeDialog() {
                this.setSnapshotDialog(false);
            },
            async getUserMedia(){
                try{
                    let userMedia = await navigator.mediaDevices.getUserMedia({video:true});
                    return userMedia;
                }catch(err){
                    return null;
                }
                }
            },
        async mounted(){
            let video = document.querySelector("#videoElement");
            let stream = await this.getUserMedia();
            if(!!stream){
                video.srcObject = stream;
            }else{
                this.noCameraError = true;
            }
        }
    };
</script>

<style lang="less">
    @import '../../../../styles/mixin.less';

    .pt-12 {
        padding-top: 12px;
    }

    .snapshot-dialog-wrap {
        @BtnBackground: #ffc739;
        background: @color-white;
        border-radius: 4px;
        box-shadow: 0 3px 6px 0 rgba(0, 0, 0, 0.16);
        width: 100%;
        padding: 0 5px;
        .accept-consent-btn {
            display: flex;
            height: 48px;
            color: @color-white;
            background-color: @BtnBackground!important;
            border-radius: 4px;
        }
    }

</style>
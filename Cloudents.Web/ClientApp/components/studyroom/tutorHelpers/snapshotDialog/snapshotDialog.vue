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
               <video autoplay="true" id="videoElement" style="width:640px; height:480px"></video>
               <div id="snapshot-container" :style="{width: width +'px', height: height+'px', display:'none'}"></div>
            </v-flex>
            <v-flex v-show="noCameraError" xs12 style="text-align: center;" class="pt-2">
               <span v-language:inner="'tutor_take_snapshot_error'"></span>
            </v-flex>
            <v-flex xs12 class="pt-4">
                <!-- <input type="text" v-model="width">
                <input type="text" v-model="height"> -->
                <!-- <input type="text" v-model="scale"> -->
            </v-flex>
            <v-flex xs12 class="pt-4">
                <v-btn class="accept-consent-btn elevation-0 align-center justify-center" @click="takeSnapshot()">
                    <span class="text-capitalize">{{snapshotBtnText}}</span>
                    <span class="text-capitalize" v-show="timerCountdown">&nbsp;({{timerCountdown}})</span>
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
                width: 800,
                height: 600,
                scale: 1,
                snapshotBtnText: LanguageService.getValueByKey('tutor_take_snapshot_btn'),
                timerCountdown: null,
            };
        },
        methods: {
            ...mapActions(['setSnapshotDialog']),
            drawImageToCanvas(){
                let oldCanvas = document.getElementById("snapshot");
                if(oldCanvas){
                    oldCanvas.remove();
                }
                let canvasContainer = document.getElementById("snapshot-container");
                let canvasElm = document.createElement('canvas');
                canvasElm.width = this.width * Number(this.scale);
                canvasElm.height = this.height * Number(this.scale);
                canvasElm.id = "snapshot";
                canvasContainer.appendChild(canvasElm);
                let context = canvasElm.getContext('2d');
                let player = document.getElementById("videoElement");
                let hRatio = canvasElm.width / 640;
                let vRatio = canvasElm.height / 480;
                let ratio  = Math.min ( hRatio, vRatio );
                context.drawImage(player, 0, 0, 640, 480, 0, 0, 640*ratio, 480*ratio);
            },
            downloadImg(blob){
                let a = document.createElement("a");
                document.body.appendChild(a);
                a.style = "display: none";
                let url = URL.createObjectURL(blob)
                a.href = url;
                a.download = 'img.png';
                a.click();
            },
            getUrlFromBlob(){
                let context = document.getElementById("snapshot").getContext('2d');
                let blob = context.canvas.toBlob(this.downloadImg, "image/png");
            },
            startInterval() {
                let timeleft = 3;
                this.timerCountdown = timeleft;
                let downloadTimer = setInterval(()=>{
                timeleft--;
                this.timerCountdown = timeleft;
                if(timeleft <= 0){
                    clearInterval(downloadTimer);
                    this.timerCountdown = null;
                    this.drawImageToCanvas();
                    this.getUrlFromBlob();
                }
                },1000);
            },
            takeSnapshot() {
                this.startInterval();                     
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
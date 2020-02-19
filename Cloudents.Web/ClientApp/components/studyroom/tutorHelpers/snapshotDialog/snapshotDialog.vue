<template>
    <div class="snapshot_dialog_wrap pb-6">
        <v-layout class="pt-4">
            <v-flex xs12 class="text-right px-4">
                <v-icon class="body-2 cursor-pointer" @click="closeDialog()">sbf-close</v-icon>
            </v-flex>
        </v-layout>
        <v-layout wrap align-center>
            <v-flex xs12 class="pt-3 text-center">
                <span class="snapshot_dialog_wrap_title font-weight-bold" v-language:inner="'tutor_take_snapshot_title'"></span>
            </v-flex>
            <v-flex xs12 class="pt-2 text-center">
                <span v-language:inner="'tutor_take_snapshot_message'"></span>
            </v-flex>
            <v-flex class="snapshot_video_container pt-2 text-center" v-show="!noCameraError" id="videoElementContainer" xs12>
               <video autoplay="true" id="videoElement" style="width:640px; height:480px; display:none;"></video>
               <div id="snapshot-container" :style="{width: width +'px', height: height+'px', display:'none'}"></div>
            </v-flex>
            <v-flex v-show="noCameraError" xs12 class="snapshot_video_container snapshot_no_camera_error pt-2 text-center">
               <img :src="require(`./img/noCamera-${lang}.png`)" alt="">
            </v-flex>
            <!-- <v-flex xs12 class="pt-6"> -->
                <!-- <input type="text" v-model="width">
                <input type="text" v-model="height"> -->
                <!-- <input type="text" v-model="scale"> -->
            <!-- </v-flex> -->
            <v-flex xs12 class="pt-6">
                <v-btn :disabled="noCameraError" class="snapshot_accept_consent_btn elevation-0 align-center justify-center" @click="takeSnapshot()">
                    <span class="text-capitalize">{{snapshotBtnText}}</span>
                    <!-- <span class="text-capitalize" v-show="timerCountdown">&nbsp;({{timerCountdown}})</span> -->
                </v-btn>
            </v-flex>
        </v-layout>
    </div>
</template>

<script>
    import { mapActions, mapGetters } from 'vuex';
    // import studyRoomRecordingService from '../../studyRoomRecordingService';
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
                lang: global.lang,
                audio: null,
            };
        },
        computed:{
            ...mapGetters(['getLocalVideoTrack']),
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
                let player = document.querySelectorAll("#videoElementContainer video");
                player = player[player.length-1];
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
                context.canvas.toBlob(this.downloadImg, "image/png");
            },
            // startInterval() {
            //     let timeleft = 3;
            //     this.timerCountdown = timeleft;
            //     let downloadTimer = setInterval(()=>{
            //     timeleft--;
            //     this.timerCountdown = timeleft;
            //     if(timeleft <= 0){
            //         clearInterval(downloadTimer);
            //         this.timerCountdown = null;
            //         this.drawImageToCanvas();
            //         this.getUrlFromBlob();
            //     }
            //     },1000);
            // },
            playSound(){
                this.audio.play();
            },
            takeSnapshot() {
                // this.startInterval(); 
                this.playSound();
                this.drawImageToCanvas();
                this.getUrlFromBlob();  
                this.closeDialog();                  
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
            this.audio = new Audio(require('./sound/Shutter.wav'));
            let videoContainer = document.querySelector("#videoElementContainer");
            let videoElm = document.querySelector('#videoElement');
            if(this.getLocalVideoTrack && !this.getLocalVideoTrack.isStopped){
                videoContainer.appendChild(this.getLocalVideoTrack.attach());
            }else{
                //incase no local video or localvideo disconnected;
                videoElm.style.display = '';
                let stream = await this.getUserMedia();
                if(!!stream){
                    videoElm.srcObject = stream;
                }else{
                    this.noCameraError = true;
                }
            }
        }
    };
</script>

<style lang="less">
    @import '../../../../styles/mixin.less';

    // .pt-12 {
    //     padding-top: 12px;
    // }
.studyroom-snapshot-dialog{
    background: @color-white;
        border-radius: 4px;
        box-shadow: 0 3px 6px 0 rgba(0, 0, 0, 0.16);
    .snapshot_dialog_wrap {
        @BtnBackground: #ffc739;
        border-radius: 4px;
        width: 100%;
        padding: 0 5px;
        .snapshot_dialog_wrap_title{
            font-size: 16px;
        }
        .snapshot_video_container{
           // min-width:640px; 
          //  min-height:480px;
          video {
               display: block;
  max-width:~"calc(100vw - 100px)";
  max-height:~"calc(100vh - 300px)";
  width: auto;
  height: auto;
  margin: 0 auto;
          }
        }
        .snapshot_accept_consent_btn {
            display: flex;
            height: 48px;
            color: @color-white;
            background-color: @BtnBackground!important;
            border-radius: 4px;
            margin: 0 auto;
        }
        .snapshot_no_camera_error{
            text-align: center;
            //min-width: 640px;
            //min-height: 480px;
            justify-content: center;
            align-items: center;
            display: flex;
            img {
                        display: block;
  max-width:~"calc(100vw - 100px)";
  max-height:~"calc(100vh - 300px)";
  width: auto;
  height: auto;
  margin: 0 auto;
            }
        }
    }
}
    

</style>
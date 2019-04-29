<template>
   <v-layout align-center justify-center column class="audio-out-container mt-5 py-3">
       <v-flex class="mb-3">
           <speakerImage class="speaker-icon"></speakerImage>
       </v-flex>
       <v-flex class="mt-3">
           <h4 class="auidio-out-title subheading">Audio Output quality test</h4>
       </v-flex>
       <v-flex class="mt-3 mb-4">
           <img class="eq-image" src="../../../images/eq.gif" alt="">
       </v-flex>
        <div class="audio-input-validation">
        </div>
   </v-layout>
</template>

<script>
    import speakerImage from '../../../images/speaker.svg'
    export default {
        name: "audioOutputValidation",
        components: {speakerImage},
        data() {
            return {
                soundUrl: `https://zboxstorage.blob.core.windows.net/zboxhelp/new/music-check.mp3`,
                audio: null
            }
        },
        methods: {
            playTestSound() {
                this.audio = new Audio(`${this.soundUrl}`);
                this.audio.play()
            },
            stopSound(){
                this.audio.pause();
            }
        },
        created(){
            this.playTestSound()
        },
        beforeDestroy(){
            this.stopSound()
        }
    }
</script>

<style lang="less">
.audio-out-container{
    .auidio-out-title{
        font-weight: 600;
        color: rgba(0, 0, 0, 0.87);
    }
    .speaker-icon{
        height: 40px;
        width: 40px;
        fill: #a8a8a8;
    }
}
</style>
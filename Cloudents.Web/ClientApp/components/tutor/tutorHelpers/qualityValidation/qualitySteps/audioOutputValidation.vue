<template>
    <div>
        <h4>Audio Output quality test</h4>
        <div class="audio-input-validation">
        </div>
    </div>
</template>

<script>
    export default {
        name: "audioOutputValidation",
        data() {
            return {
                soundUrl: 'https://blue-online-static.wyzantcdn.com//static/dist/assets/12_short_working-solutions_0066.mp3',
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

<style scoped>

</style>
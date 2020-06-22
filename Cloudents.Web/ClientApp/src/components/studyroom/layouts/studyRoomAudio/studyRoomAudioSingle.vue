<template>
   <div></div>
</template>

<script>
export default {
   data() {
      return {
         audioTrack:null,
      }
   },
   props:{
      participantAudio:{
         type:Object,
         required:true
      }
   },
   methods: {
      handleAudioTrack(){
         if(this.participantAudio.id == this.$store.getters.accountUser.id){
            return; //user dont need his audio only the remote need
         } 
         if(this.audioTrack){
            return;
         }else{
            this.audioTrack = this.participantAudio.audio;
            let self = this;
            this.$nextTick(()=>{
               let previewContainer = this.$el;
               let audioTag = previewContainer.querySelector("audio");
               if (audioTag){
                  previewContainer.removeChild(audioTag)
               }
               previewContainer.appendChild(self.participantAudio.audio.attach());
            })
         }
      },
   },
   mounted() {
      this.handleAudioTrack()
   },
   destroyed() {
      if(this.audioTrack){
         this.audioTrack.detach().forEach((detachedElement) => {
            detachedElement.remove();
         });
      }
   },
}
</script>
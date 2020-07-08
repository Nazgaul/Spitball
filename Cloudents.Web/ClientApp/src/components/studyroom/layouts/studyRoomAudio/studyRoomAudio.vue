<template>
  <div style="display: none;" class="studyRoomAudio" v-if="roomParticipantsAudio">
    <studyRoomAudioSingle 
      v-for="participantAudio in roomParticipantsAudio" 
      :participantAudio="participantAudio" 
      :key="participantAudio.audio.sid"/>
  </div>
</template>

<script>
import studyRoomAudioSingle from './studyRoomAudioSingle.vue';
export default {
  components:{studyRoomAudioSingle},
  computed:{
    roomParticipantsAudio(){
      if(this.$store.getters.getRoomParticipantsAudio?.length){
        return this.$store.getters.getRoomParticipantsAudio.filter(p=>p.id != this.$store.getters.accountUser?.id)
      }else{
        return null
      }
    }
  }
}
</script>
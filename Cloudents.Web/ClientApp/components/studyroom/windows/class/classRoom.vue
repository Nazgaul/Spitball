<template>
   <div class="classRoom">
      <template v-if="roomParticipants">
         <userPreview 
               v-for="participant in roomParticipants" 
               :key="Object.values(participant)[0].id" :participant="Object.values(participant)[0]"
               class="ma-2 classRoomCards"/>
      </template>
   </div>
</template>

<script>
import userPreview from '../../layouts/userPreview/userPreview.vue'
export default {
   components:{
      userPreview
   },
   computed: {
      roomParticipants(){
         if(this.$store.getters.getRoomParticipants){
            let participants = Object.entries(this.$store.getters.getRoomParticipants).map((e) => ( { [e[0]]: e[1] } ));
            let participantIdx;
            let currentParticipant = participants.find((participant,index)=>{
               participantIdx = index;
               return Object.values(participant)[0].id == this.$store.getters.accountUser.id
            })
            if(currentParticipant){
               participants.splice(participantIdx,1)
               participants.unshift(currentParticipant)
            }
            return participants
         }else{
            return null
         }
      }
   }
}
</script>

<style lang="less">
.classRoom{
   background-color: #212123;
   .classRoomCards{
      float: left;
      width: 236px !important;
      height: 149px !important;
   }
}
</style>
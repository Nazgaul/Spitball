<template>
   <v-slide-y-transition>
      <v-footer
         app
         color="#212123"
         inset
         fixed
         :height="footerExtend ? 124 : 0"
         class="pa-0 pl-2 studyRoomFooter"
         :class="{'py-3':footerExtend}"
      >
         <button class="pb-1 collapseBtnFooter" @click="footerExtend = !footerExtend">
            <v-icon v-if="footerExtend">sbf-arrow-down</v-icon>
            <v-icon v-else>sbf-arrow-up</v-icon>
         </button>
         <v-slide-group v-show="footerExtend"
            class="pa-0"
            active-class="success"
            show-arrows
            color="#fff"
            prev-icon="sbf-arrow-left-carousel"
            next-icon="sbf-arrow-right-carousel">
            <template v-if="roomParticipants">
               <v-slide-item  v-for="participant in roomParticipants" :key="Object.values(participant)[0].id">
                  <userPreview :participant="Object.values(participant)[0]" class="classRoomCards mx-1" />
               </v-slide-item>
            </template>
         </v-slide-group>
      </v-footer>
   </v-slide-y-transition>
</template>

<script>
import userPreview from '../userPreview/userPreview.vue';

export default {
   data() {
      return {
         footerExtend:true,
      }
   },
   components:{userPreview},
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
   },
   watch: {
      footerExtend:{
         immediate:true,
         handler(newVal){
            this.$store.commit('setStudyRoomFooterState',newVal)
         }
      }
   },
}
</script>

<style lang="less">
   .studyRoomFooter {
      .collapseBtnFooter{
         position: absolute;
         top: -22px;
         right: 0/*rtl:ignore */;
         width: 30px;
         height: 30px;
         border-radius: 8px;
         box-shadow: 0 2px 10px 0 rgba(0, 0, 0, 0.21);
         outline: none;
         background-color: #212123;
      }
      .classRoomCards{
         width: 154px;
         height: 100px;
      }
      .sbf {
         color: #fff;
      }
   }
</style>
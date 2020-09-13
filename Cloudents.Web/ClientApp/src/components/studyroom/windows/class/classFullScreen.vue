<template>
   <div class="classFullScreen">
      <div id="tutorVideoLayout">
         <tutorFullScreen class="tutorFullScreen_p" :style="`height:480;width:${480 * getRatio()}`"/>
      </div>
      <v-footer :height="124" inset app fixed color="#212123" class="classFooter pa-0 py-3 ps-2">
         <v-slide-group
            class="pa-0"
            active-class="success"
            show-arrows
            color="#fff"
            prev-icon="sbf-arrow-left-carousel"
            next-icon="sbf-arrow-right-carousel">
            <template v-if="roomParticipants" >
               <v-slide-item v-for="participant in roomParticipants" :key="Object.values(participant)[0].id">
                  <userPreview :participant="Object.values(participant)[0]" class="classRoomCards mx-1"/>
               </v-slide-item>
            </template>
         </v-slide-group>
      </v-footer>
   </div>
</template>

<script>
import {initLayoutContainer} from 'video-layout';
import tutorFullScreen from '../../layouts/userPreview/tutorFullScreen.vue';
import userPreview from '../../layouts/userPreview/userPreview.vue'
import { mapGetters } from 'vuex';
export default {
   data() {
      return {
         layout: undefined,
         ratios: [4/3, 3/4, 16/9],
         layoutEl: undefined,
      }
   },
   components:{
      userPreview,
      tutorFullScreen
   },
   watch: {
      getStudyRoomDrawerState:{
         handler(){
            let self = this;
            setTimeout(()=>{
               self.layout();
            },500)
         }
      },
   },
   computed: {
      ...mapGetters(['getStudyRoomDrawerState']),
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
   methods: {
      updateLayoutValues(){
         this.layout = initLayoutContainer(this.layoutEl, {
         maxRatio: 3/4,    
         minRatio: 9/16,   
         fixedRatio: false,  
         bigPercentage: 0.8,
         bigFixedRatio: false,
         bigMaxRatio: 3/2,
         bigMinRatio: 9/16,
         bigFirst: true,
         alignItems: 'center',
         smallMaxWidth: 'Infinity',
         bigMaxHeight: 'Infinity',
         smallMaxHeight: 'Infinity',
         bigAlignItems: 'center',
         bigMaxWidth: 'Infinity',
         smallAlignItems: 'center',
         animate: true,
         }).layout;
      },
      getRatio(){
         return this.ratios[Math.round(Math.random() * (this.ratios.length - 1))]
      },
   },
   mounted() {
      this.layoutEl = document.getElementById("tutorVideoLayout");
      this.updateLayoutValues();
      this.layout();
   },
}
</script>

<style lang="less">
.classFullScreen{
   background-color: #212123;
   display: block; //Just to remove cache
   position: relative;
   .v-footer{
      justify-content: center;
   }
   #tutorVideoLayout {
      position: absolute;
      top:0;
      left: 0;
      right: 0;
      bottom: 0;
      background: black;
      .tutorFullScreen_p {
         display: flex;
         align-items: center;
         justify-content: center;
         transition-property: all;
         transition-duration: 0.5s;
      }
   }
   .classFooter{
      .classRoomCards{
         width: 154px;
         height: 100px;
      }
      .sbf {
         color: #fff;
      }
   }
}
</style>
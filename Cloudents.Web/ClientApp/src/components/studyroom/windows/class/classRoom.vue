<template>
   <div class="classRoom">
     <div id="layout">
      <template v-if="roomParticipants">
         <userPreviewZoom class="userPreviewZoomTEST" v-for="participant in roomParticipants" :key="Object.values(participant)[0].id"
                :participant="Object.values(participant)[0]"
                :style="`height:480;width:${480 * getRatio()}`"
                />

      </template>

     </div>
      <!-- <template v-if="roomParticipants"> -->
         <!-- <userPreviewZoom 
               v-for="participant in roomParticipants" 
               :key="Object.values(participant)[0].id" :participant="Object.values(participant)[0]"
               class="ma-2 classRoomCards"/> -->
      <!-- </template> -->
   </div>
</template>

<script>
import {initLayoutContainer} from 'video-layout';

// import userPreview from '../../layouts/userPreview/userPreview.vue';
import userPreviewZoom from '../../layouts/userPreview/userPreviewZoom.vue';
import { mapGetters } from 'vuex';
export default {
   components:{
      // userPreview
      userPreviewZoom
   },
   data() {
      return {
         layout: undefined,
         ratios: [4/3, 3/4, 16/9],
         layoutEl: undefined,
      }
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
   watch: {
      getStudyRoomDrawerState:{
         handler(){
         let self = this;
         setTimeout(()=>{
            self.layout();
         },500)
         // this.$nextTick(()=>{
         //    self.layout();
         // })
            // if(newVal){
            // }else{
            //    this.layout();

            // }
         }
      },
      roomParticipants:{
         deep:true,
         immediate:true,
         handler(){
         let self = this;
         this.$nextTick(()=>{
            self.layout();
         })
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
      this.layoutEl = document.getElementById("layout");
      this.updateLayoutValues();
      this.layout();
   },
}
</script>

<style lang="less">
.classRoom{
   background-color: #212123;
   position: relative;
   #layout {
      position: absolute;
      // top: 140px;
      top:0;
      left: 0;
      right: 0;
      bottom: 0;
      .userPreviewZoom {
         // background-color: #000;
         display: inline-block;
         // border: 1px solid #fff;
         transition-property: all;
         transition-duration: 0.5s;
         // display: flex;
         // align-items: center;
         // justify-content: center;
         // flex-direction: column;
      }
   }
   // .classRoomCards{
   //    float: left;
   //    width: 236px;
   //    height: 149px;
   // }
}
</style>
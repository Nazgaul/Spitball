<template>
   <div class="shareScreenWindow">
      <div class="shareScreenVideoContainer">
         <template v-if="!isRoomTutor">
            <div id="shareScreenWindowVideo"></div>
            <div class="presentCard studentPresentWaiting" v-if="!isTutorSharing">
               <img src="./image/studentStart.png" alt="">
               <div class="text">{{$t('studyRoom_start_present_student')}}</div>
            </div>
         </template>
         <div class="presentCard tutorStartPresenting" v-if="isRoomTutor">
            <template v-if="!isShareScreen"> 
               <img src="./image/start.png" alt="">
               <div class="text">{{$t('studyRoom_start_present_text')}}</div>
               <v-btn class="tutorStartPresentingBtn white--text text-truncate" 
                     @click="startShareScreen"
                     color="#4c59ff" rounded depressed>
                        {{$t('studyRoom_start_present')}}
               </v-btn>
            </template>
            <template v-else> 
               <img src="./image/stop.png" alt="">
               <div class="text">{{$t('studyRoom_stop_present_text')}}</div>
               <v-btn class="tutorStartPresentingBtn white--text text-truncate"
                     @click="stopShareScreen" 
                     color="#e84749" rounded depressed>
                     <div class="endPresentIcon"></div>
                        {{$t('studyRoom_stop_present')}}
               </v-btn>
            </template>
         </div>
      </div>
   </div>
</template>

<script>
import { mapGetters } from 'vuex';
export default {
   data() {
      return {
         isTutorSharing:false,
      }
   },
   watch: {
      tutorScreenTrack:{
         immediate:true,
         deep:true,
         handler(track){
            if(this.isRoomTutor) return;
            if(track){
               let self = this
               this.$nextTick(()=>{
                  const localMediaContainer = document.getElementById('shareScreenWindowVideo');
                  let videoTag = localMediaContainer.querySelector("video");
                  if (videoTag) {localMediaContainer.removeChild(videoTag)}
                  localMediaContainer.appendChild(track.attach());
                  self.isTutorSharing = true
               })
            }else{
               this.isTutorSharing = false
            }
         }
      }
   },
   computed: {
      ...mapGetters(['getRoomTutorParticipant']),
      tutorScreenTrack(){
         return this.getRoomTutorParticipant?.screen;
      },
      isRoomTutor(){
         return this.$store.getters.getRoomIsTutor
      },
      isShareScreen(){
         return this.$store.getters.getIsShareScreen
      }
   },
   methods: {
      startShareScreen(){
         this.$store.dispatch('updateShareScreen',true)
      },
      stopShareScreen(){
         this.$store.dispatch('updateShareScreen',false)
      },
   },
}
</script>

<style lang="less">
.shareScreenWindow{
   background-color: #f5f5f5;
   .shareScreenVideoContainer{
      width: 100%;
      height: 100%;
      max-height: ~"calc(100vh - 186px)"; // 124px footer + 62px header
      overflow: hidden;
      position: relative;
      display: flex;
      justify-content: center;
      .presentCard{
         margin-top: 140px;
         width: 540px;
         height: 264px;
         border-radius: 6px;
         box-shadow: 0 2px 12px 0 rgba(0, 0, 0, 0.1);
         padding: 34px 0;
         display: flex;
         flex-direction: column;
         align-items: center;
         
         &.studentPresentWaiting{
            padding: 52px 0 42px;
            background-color: #4c59ff;
            img{
               width: 90px;
               margin-bottom: 24px;
            }
            .text{
               font-size: 22px;
               font-weight: 600;
               color: #ffffff;
               text-align: center;
               padding: 0 50px;
            }
         }
         &.tutorStartPresenting{
            background: white;
            img{
               width: 90px;
               margin-bottom: 16px;
            }
            .text{
               font-size: 22px;
               font-weight: 600;
               color: #43425d;
               padding-bottom: 28px;
            }
            .tutorStartPresentingBtn{
               height: 42px;
               min-width: 120px;
               padding: 0 38px;
               .endPresentIcon{
                  width: 13px;
                  height: 13px;
                  background: white;
                  border-radius: 3px;
                  margin-right: 8px;
               }
            }
         }
      }
      #shareScreenWindowVideo{
         height: ~"calc(100vh - 186px)"; // 124px footer + 62px header

         video {
            width: 100%;
            height: 100%;
            object-fit: cover;
            object-position: center;
         }
           video::-webkit-media-controls-enclosure {
              display: none !important;
           }
      }
   }
}
</style>
<template>
   <!-- :fullscreen="$vuetify.breakpoint.xsOnly" -->
   <v-dialog
    :value="true"
    persistent
    :maxWidth="'500'"
    :content-class="'mobilePermissionDialog'">
      <template v-if="!isShowMeHow">
         <div class="mb-5 mt-3 content">
            {{$t('spitballAccess')}}
            </div>
         <v-btn @click="onShowMeHow" block x-large color="#4c59ff" depressed class="white--text mb-5 btnText">
            {{$t('showMe')}}
         </v-btn>
         <v-btn @click="onClose" block x-large color="#4c59ff" depressed class="white--text mb-5 btnText">
            {{$t('knowhow')}}
         </v-btn>
         <v-btn @click="onClose" color="#4c59ff" block x-large depressed class="white--text btnText">
            {{$t('continue_tracks')}}
         </v-btn>
      </template>

      <v-card v-if="isShowMeHow" class="elevation-0">
         <video width="100%" playsinline @loadeddata="playVideo" ref="permissionDialogVideo" 
         loop autoplay muted class="dialogPermissionVideo mb-2" 
         :src="getPermissionBlockedVideo()"></video>
         <v-btn @click="onClose" rounded color="#4c59ff" depressed class="white--text">
            {{$t('got_it')}}
         </v-btn>
      </v-card>
    </v-dialog> 
</template>

<script>
export default {
   data() {
      return {
         isShowMeHow:false,
      }
   },
   methods: {
      getPermissionBlockedVideo(){
         // eslint-disable-next-line no-undef
         let isSafari = navigator.userAgent.search("Safari") >= 0 && navigator.userAgent.search("Chrome") < 0;
         if(isSafari){
            return require('./ISO_VIDEO.mp4').default
         }else{
            return require('./ANDROID_VIDEO.mp4').default
         }
      },
      playVideo(){
         let playPromise = this.$refs.permissionDialogVideo.play()
         let self = this
         
         if (playPromise !== undefined) {
               playPromise.then(() => {}).catch(error => {
                  self.$appInsights.trackException(error)
               });
         }
      },
      onShowMeHow(){
         this.isShowMeHow = true
      },
      onClose(){
         this.$emit('onClose')
      }
   },
}
</script>

<style lang="less">
.mobilePermissionDialog{
   background: white;
   padding: 14px 30px;
   text-align: center;
   .content{
      font-size: 20px;
      font-weight: 600;
      color: #43425d;
   }
   .btnText{
      font-size: 16px;
      font-weight: 600;
      white-space: initial;
      .v-btn__content{
         flex: auto;
      }
   }
   .dialogPermissionVideo{
      border: 1px solid black;
   }
}
</style>
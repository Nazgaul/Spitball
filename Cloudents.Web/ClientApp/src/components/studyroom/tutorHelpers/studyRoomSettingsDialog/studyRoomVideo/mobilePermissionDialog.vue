<template>
   <v-dialog
    :value="true"
    persistent
    :maxWidth="'296'"
    :content-class="'mobilePermissionDialog'">
      <v-icon @click="onClose" size="12" class="closeIcon">sbf-close</v-icon>
      <template v-if="!isShowMeHow">
         <div class="mb-6 content">
            {{$t('spitballAccess')}}
            </div>
         <v-btn @click="onShowMeHow" rounded block height="44" color="#4c59ff" depressed class="white--text mb-3 btnText">
            {{$t('showMe')}}
         </v-btn>
         <v-btn @click="onClose" rounded block height="44" outlined color="#4c59ff" depressed class="mb-3 btnText">
            {{$t('knowhow')}}
         </v-btn>
         <v-btn @click="onClose" color="#4c59ff" rounded block height="44" outlined depressed class="btnText">
            {{$t('continue_tracks')}}
         </v-btn>
      </template>

      <div v-if="isShowMeHow" class="elevation-0 videoCard">
         <video  playsinline @loadeddata="playVideo" ref="permissionDialogVideo" 
         loop autoplay muted class="dialogPermissionVideo mb-2" 
         :src="getPermissionBlockedVideo()"></video>
         <v-btn @click="onClose" rounded color="#4c59ff" depressed class="white--text">
            {{$t('got_it')}}
         </v-btn>
      </div>
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
   border-radius: 6px;
   padding: 16px;
   padding-top: 34px;
   text-align: center;
   position: relative;
   width: 100%;
  box-shadow: 0 13px 21px 0 rgba(0, 0, 0, 0.51);
   .closeIcon{
      position: absolute;
      width: auto;
      right: 10px;
      top: 10px;
   }
   .content{
      color: #43425d;
      font-size: 18px;
      font-weight: 600;
      line-height: 1.44;
   }
   .btnText{
      white-space: initial;
      font-size: 14px;
      font-weight: 600;
      .v-btn__content{
         flex: auto;
      }
   }
   .videoCard{
      .dialogPermissionVideo{
         max-width: 100%;
         border: 1px solid black;
      }
      video{
         width: 100%;
         max-height: calc(~"100vh - 250px");
      }
   }
}
</style>
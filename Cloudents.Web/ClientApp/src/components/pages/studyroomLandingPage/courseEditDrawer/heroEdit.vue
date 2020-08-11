<template>
   <v-expansion-panel class="editSection mb-4 elevation-0 rounded heroEdit">
      <v-expansion-panel-header class="pa-3">
         <div class="editHeader d-flex justify-space-between align-center">
            {{$t('hero')}}
            <!-- <v-btn height="18" width="18" color="#4c59ff" fab x-small depressed>
               <v-icon size="10" color="white">sbf-v</v-icon>
            </v-btn> -->
         </div>
         </v-expansion-panel-header>
      <v-expansion-panel-content>
         <v-textarea class="textInputs" auto-grow color="#4c59ff" rows="1" :value="courseName">
            <template v-slot:label>
                  <div class="inputLabel">
                     Course Name
                  </div>
            </template>
         </v-textarea>
         <v-textarea class="textInputs mt-3" auto-grow color="#4c59ff" rows="1" :value="courseDescription">
            <template v-slot:label>
                  <div class="inputLabel">
                     Course Description
                  </div>
            </template>
         </v-textarea>
         <div class="courseImage mb-8">
            <div class="courseImage_title mb-3">Image or a video</div>
            <label class="liveImageWrap d-flex flex-column">
               <uploadImage v-show="isLoaded" :fromLiveSession="true" @setLiveImage="handleLiveImage" class="editLiveImage"/>
               <div class="noDefaultImage" v-if="!$route.params.id && !previewImage && !image">
                  <v-icon size="40" color="#bdc0d1">sbf-plus-sign</v-icon>
               </div>
               <template v-else>
                  <v-skeleton-loader v-if="!isLoaded" height="130" width="200" type="image"></v-skeleton-loader>
                  <img v-show="isLoaded" @load="loaded" class="liveImage" :src="previewImage || $proccessImageUrl(image, 200, 130)" width="200" height="130" alt="">
               </template>
               <div class="recommendedImage mt-1">{{$t('img_res')}}</div>
            </label>
         </div>
         <v-text-field class="textInputs mb-2" color="#4c59ff" :value="courseBtnText" autocomplete="off" >
            <template v-slot:label>
                  <div class="inputLabel">
                     Button
                  </div>
            </template>
         </v-text-field>
      </v-expansion-panel-content>
   </v-expansion-panel>
</template>

<script>
import uploadImage from '../../../new_profile/profileHelpers/profileBio/bioParts/uploadImage/uploadImage.vue';

export default {
   data() {
      return {
         isLoaded: false,
         previewImage: null,
         newLiveImage: null,
      }
   },
   components:{
      uploadImage
   },
   computed: {
      courseName(){
         return this.$store.getters.getCourseDetails?.name
      },
      courseDescription(){
         return this.$store.getters.getCourseDetails?.description
      },
      image() {
         return this.$store.getters.getCourseDetails?.image;
      },
      courseBtnText(){
         return 'Buy Now!'
      }
   },
   methods: {
      handleLiveImage(previewImage) {
         if(previewImage) {
            this.isLoaded = false
            let formData = new FormData();
            formData.append("file", previewImage[0]);
            let self = this
            this.$store.dispatch('updateLiveImage', formData).then(({data}) => {
               self.isLoaded = false
               self.previewImage = window.URL.createObjectURL(previewImage[0])
               self.newLiveImage = data.fileName
               self.$store.commit('setCourseCoverImage', self.newLiveImage)
            })
         }
      },
      loaded() {
         this.isLoaded = true
      }
   }
}
</script>
<style lang="less" src="./editCards.less"></style>
<style lang="less">
.heroEdit{
   .courseImage{
      .courseImage_title{
         font-size: 14px;
         line-height: 1.64;
         color: #43425d;
      }
         .liveImageWrap {
             cursor: pointer;
             max-width: fit-content;
             text-align: center;
             position: relative;
             .editLiveImage {
                 position: absolute;
                 text-align: center;
                 border-radius: 3px;
                 background-color: rgba(0,0,0,.6);
                 z-index: 1;
                 top: 1px;
                 left: 1px;
                 padding: 6px;
             }
             .liveImage {
                 border: solid 1px #c6cdda;
                 border-radius: 4px;
             }
             .recommendedImage {
                 font-size: 12px;
                 color: #b4babd;
             }
             .noDefaultImage {
                 display: flex;
                 justify-content: center;
                 width: 200px;
                 height: 130px;
                 border-radius: 6px;
                 background-color: #f0f4f8
             }
         }
   } 
}
</style>
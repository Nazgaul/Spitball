<template>
   <v-expansion-panel @click="goTo" class="editSection mb-4 elevation-0 rounded heroEdit">
      <v-expansion-panel-header class="pa-3">
         <div class="editHeader d-flex justify-space-between align-center">
            {{$t('hero')}}
            <!-- <v-btn height="18" width="18" color="#4c59ff" fab x-small depressed>
               <v-icon size="10" color="white">sbf-v</v-icon>
            </v-btn> -->
         </div>
         </v-expansion-panel-header>
      <v-expansion-panel-content>
         <v-textarea class="textInputs" auto-grow color="#4c59ff" rows="1" v-model="courseName" :rules="[rules.required]">
            <template v-slot:label>
               <div class="inputLabel" v-t="'course_name'"/>
            </template>
         </v-textarea>
         <v-textarea class="textInputs mt-3" auto-grow color="#4c59ff" rows="1"  v-model="courseDescription">
            <template v-slot:label>
               <div class="inputLabel" v-t="'course_description'"/>
            </template>
         </v-textarea>
         <div class="courseImage mb-2">
            <div class="courseImage_title mb-3" v-t="'course_imgVideo'"/>
            <label class="liveImageWrap d-flex flex-column">
               <uploadImage v-show="isLoaded" :fromLiveSession="true" @setLiveImage="handleLiveImage" class="editLiveImage"/>
               <v-skeleton-loader v-if="!isLoaded" height="130" width="200" type="image"></v-skeleton-loader>
               <img v-show="isLoaded" @load="loaded" class="liveImage" :src="courseImage" width="200" height="130" alt="">
               <div class="recommendedImage mt-1">{{$t('img_res')}}</div>
            </label>
         </div>
         <v-text-field class="textInputs mb-2" color="#4c59ff" v-model="courseBtnText" autocomplete="off" >
            <template v-slot:label>
               <div class="inputLabel" v-t="'course_btn'"/>
            </template>
         </v-text-field>
      </v-expansion-panel-content>
   </v-expansion-panel>
</template>

<script>
import uploadImage from '../../../new_profile/profileHelpers/profileBio/bioParts/uploadImage/uploadImage.vue';
import { validationRules } from '../../../../services/utilities/formValidationRules.js'

export default {
   data() {
      return {
         heroModal:true,
         isLoaded: false,
         rules: {
            required: (val) => validationRules.required(val),
         }
      }
   },
   components:{
      uploadImage
   },
   computed: {
      courseName:{
         get(){
            return this.$store.getters.getCourseNamePreview;
         },
         set(val){
            this.$store.commit('setEditedDetailsByType',{
               type:'name',
               val
            })
         }
      },
      courseDescription:{
         get(){
            return this.$store.getters.getCourseDescriptionPreview;
         },
         set(val){
            this.$store.commit('setEditedDetailsByType',{
               type:'description',
               val
            })
         }
      },
      courseImage(){
         let img = this.$store.getters.getCourseImagePreview;
         if(img && img.includes('blob')){
            return img;
         }else{
            return this.$proccessImageUrl(img, 200, 130)
         }
      },
      courseBtnText:{
         get(){
            if(this.$store.getters.getCourseButtonPreview) {
               return this.$store.getters.getCourseButtonPreview;
            }
            else{
               return this.$store.getters.getCoursePrice?.coursePrice?.amount? this.$t('save_spot') : this.$t('free_enroll')
            }
         },
         set(val){
            this.$store.commit('setEditedDetailsByType',{
               type:'heroButton',
               val
            })
         }
      },
   },
   methods: {
      goTo(e){
         if(!e.currentTarget.classList.toString().includes('--active')){
            this.$vuetify.goTo('#courseInfoSection',{
               duration: 1000,
               offset: 10,
               easing:'easeInOutCubic',
            })
         }
      },
      handleLiveImage(previewImage) {
         if(previewImage) {
            this.isLoaded = false
            let formData = new FormData();
            formData.append("file", previewImage[0]);
            let self = this
            this.$store.dispatch('updateLiveImage', formData).then(({data}) => {
               self.isLoaded = false
               self.$store.commit('setEditedDetailsByType',{
                  type:'image',
                  val: data.fileName
               })
               self.$store.commit('setEditedDetailsByType',{
                  type:'previewImage',
                  val: window.URL.createObjectURL(previewImage[0])
               })
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
         }
   } 
}
</style>
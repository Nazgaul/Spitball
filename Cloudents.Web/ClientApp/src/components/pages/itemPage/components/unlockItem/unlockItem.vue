<template>
  <div class="unlockItem">
     <div v-if="isDocument" class="unlockItem_document">
         <div class="unlockItem_document_container">
            <div class="unlockItem_document_title" v-t="'documentPage_unlock_title'"/>
            <div class="unlockItem_document_subtitle">{{$t('documentPage_unlock_subtitle',[docLength])}}</div>
            <v-btn
               class="unlockItem_document_btn white--text"
               @click="openPurchaseDialog"
               :loading="isLoading"
               depressed
               height="44"
               rounded
               color="#4c59ff"
            >
               <span>{{$t('documentPage_unlock_document_btn')}}</span>
            </v-btn>
            <img class="unlockItem_document_img" src="./lockdoc.png" alt="">
         </div>
     </div>
      <div v-else class="unlockItem_video">
         <div class="unlockItem_video_container">
            <div class="unlockItem_video_title" v-t="'documentPage_unlock_title'"></div>
            <div class="unlockItem_video_subtitle" v-t="'documentPage_unlock_video_subtitle'"></div>

            <v-btn
               class="unlockItem_video_btn white--text"
               @click="openPurchaseDialog"
               :loading="isLoading"
               depressed
               height="38"
               rounded
               color="#4c59ff"
            >
               <span>{{$t('documentPage_unlock_video_btn')}}</span>
            </v-btn>
            <img class="unlockItem_video_img" src="./lockvid.png" alt="">
         </div>
      </div>
   </div>
</template>

<script>
import { mapGetters } from 'vuex';

export default {
   name:'unlockItem',
   props:{
      type:{
         type: String,
         required: true
      },
      docLength:{
         type: Number,
         required: false
      }
   },
   computed: {
      ...mapGetters([
         'accountUser',
         'getBtnLoading',
         'getDocumentDetails',
      ]),
      isDocument(){
         return this.type === 'Document';
      },
      isLoading() {
         return !(this.type && !this.getBtnLoading);
      },
   },
   methods: {
      openPurchaseDialog(){
         if(!!this.accountUser) {
            let courseId = this.getDocumentDetails.courseId;
            this.$store.dispatch('updateEnrollCourse',courseId)
         } else {
            this.$store.commit('setComponent', 'register')
         }
      }
   },

}
</script>

<style lang="less">
@import '../../../../../styles/mixin';

.unlockItem{
   background: #ffffff8a;
   width: 100%;
   position: absolute;
   z-index: 5;
   height: calc(~"100% - 40px");
      // height: 100%;
   
   @media (max-width: @screen-xss) {
      // height: 100%;
   }


   .unlockItem_document{
      width: 100%;
      height: 100%;
      padding: 32px;
      // padding-bottom: 60px;
      @media (max-width: @screen-xs) {
         padding: 10px;
      }
      .unlockItem_document_container{
         width: 100%;
         height: 100%;
         text-align: center;
         color: #4d4b69;
         display: flex;
         flex-direction: column;
         align-items: center;
         justify-content: center;
         .unlockItem_document_title{
               font-size: 20px;
               font-weight: bold;
             padding: 50px 20px 8px;
             height: auto !important;
               text-shadow: 2px 2px 2px white;
               @media (max-width: @screen-xs) {
                  font-size: 18px;
                  padding: 0 20px;
               }
         }
         .unlockItem_document_subtitle{
               font-size: 18px;
               text-shadow: 2px 2px 2px white;
               font-weight: 600;
               @media (max-width: @screen-xs) {
                  font-size: 14px;
                  padding-top: 6px;
               }
         }
         .unlockItem_document_btn{
               margin: 24px 0; 
               height: 44px;
               text-transform: initial;
               font-weight: 600;
               font-size: 20px;
               max-width: 270px;
               width: 100%;
               @media (max-width: @screen-xs) {
                  max-width: 240px;
                  margin: 10px 0 0;
                  font-size: 16px;
                  height: 36px;
               }
         }
         .unlockItem_document_img{
            height: auto;
            width: inherit;
            object-fit: contain;
            padding: 0 20px;
            max-width: 500px;
            @media (max-width: @screen-xs) {
               height: 60%;
            }
         }
      }
   }
   .unlockItem_video{
      width: 100%;
      height: 100%;
      padding: 20px;
      background: white;
      @media (max-width: @screen-xs) {
         padding: 10px;
      }
      .unlockItem_video_container{
         width: 100%;
         height: 100%;
         text-align: center;
         color: #4d4b69;
         display: flex;
         flex-direction: column;
         align-items: center;
         justify-content: center;
         .unlockItem_video_title{
            // padding: 0 10px;
            font-size: 20px;
            font-weight: bold;
            height: auto !important;
            text-shadow: 2px 2px 2px white;
            @media (max-width: @screen-xs) {
               font-size: 16px;
               padding: 0 10px;
            }
            @media (max-width: @screen-xss) {
               line-height: 1.2;
            }


         }
         .unlockItem_video_subtitle{
               padding-top: 8px;
               font-size: 18px;
               text-shadow: 2px 2px 2px white;
               font-weight: 600;
               @media (max-width: @screen-xs) {
                  font-size: 14px;
                  padding-top: 6px;
               }
               @media (max-width: @screen-xss) {
                  padding-top: 4px;
                  padding-bottom: 2px;
               }
         }
         .unlockItem_video_btn{
            margin-top: 24px;
            height: 38px;
            text-transform: initial;
            font-weight: 600;
            font-size: 18px;
            max-width: 270px;
            width: 100%;
            padding-bottom: 4px;
            @media (max-width: @screen-xs) {
               max-width: 240px;
               margin-top: 10px;
               font-size: 16px;
               height: 36px;
            }
            @media (max-width: @screen-xss) {
               margin-top: 2px;
            }

         }
         .unlockItem_video_img{
            width: inherit;
            object-fit: contain;
            height: 30%;
            max-width: 500px;
            margin-top: 40px;
            @media (max-width: @screen-xs) {
               display: none;
            }
         }
      }
   }
}

</style>
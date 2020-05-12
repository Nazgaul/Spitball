<template>
   <v-slide-y-transition>
      <v-footer
         app
         color="#212123"
         inset
         fixed
         :height="footerExtend ? 124 : 12"
         class="pa-0 studyRoomFooter"
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
            <v-slide-item v-for="participant in roomParticipants" :key="participant.id">
               <userPreview :participant="participant" class="ma-2"/>
            </v-slide-item>
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
         return this.$store.getters.getRoomParticipants
      }
   },
   watch: {
      footerExtend:{
         immediate:true,
         handler(newVal){
            this.$emit('footerExtendChanged',newVal)
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
         right: 46px;
         width: 30px;
         height: 30px;
         border-radius: 8px;
         box-shadow: 0 2px 10px 0 rgba(0, 0, 0, 0.21);
         outline: none;
         background-color: #212123;
      }
      .sbf {
         color: #fff;
      }
   }
</style>
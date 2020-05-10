<template>
   <v-slide-y-transition>
      <v-footer
         app
         color="#212123"
         inset
         fixed
         :height="footerExtend ? 124 : 0"
         class="pa-0 studyRoomFooter"
      >
         <v-btn icon class="collapseIcon" :ripple="false" @click="footerExtend = !footerExtend" color="#fff">
            <v-icon v-if="footerExtend">sbf-arrow-down</v-icon>
            <v-icon v-else>sbf-arrow-up</v-icon>
         </v-btn>
         <v-slide-group
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
      .collapseIcon {
         position: absolute;
         top: -24px;
         right: 60px;
         background: #212123;
         border-radius: 0%; //vuetify override
         border-top-right-radius: 8px;
         border-top-left-radius: 8px;
         box-shadow: 0 2px 10px 0 rgba(0, 0, 0, 0.21);
      }
      .sbf {
         color: #fff;
      }
   }
</style>
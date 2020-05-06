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
         <v-btn icon class="collapseIcon" @click="footerExtend = !footerExtend" color="#fff">
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
}
</script>

<style lang="less">
   .studyRoomFooter {
      .collapseIcon {
         position: absolute;
         top: -30px;
         right: 60px;
         background: #212123;
         border-radius: 0%; //vuetify override
      }
      .sbf {
         color: #fff;
      }
   }
</style>
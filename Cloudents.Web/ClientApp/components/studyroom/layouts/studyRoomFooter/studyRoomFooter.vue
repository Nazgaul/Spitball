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
            v-model="model"
            class="pa-0"
            active-class="success"
            show-arrows
            color="#fff"
            prev-icon="sbf-arrow-left-carousel"
            next-icon="sbf-arrow-right-carousel">
            <v-slide-item v-for="n in 15" :key="n">
               <v-card :color="'grey lighten-1'" class="ma-2" height="100" width="154">
               </v-card>
            </v-slide-item>
         </v-slide-group>
      </v-footer>
   </v-slide-y-transition>
</template>

<script>
export default {
   data() {
      return {
         footerExtend:true,
         model: null,
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
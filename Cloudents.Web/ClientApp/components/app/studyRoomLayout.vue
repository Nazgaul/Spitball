<template>
  <div class="studyRoom">
    <v-navigation-drawer app right clipped></v-navigation-drawer>

    <v-app-bar app clipped-right color="#4c59ff" class="header">
      <logoComponent></logoComponent>

      <div class="roundShape mr-2"></div>
      <v-toolbar-title class="white--text">Live</v-toolbar-title>

      <v-btn>Class</v-btn>
      <v-btn>Whiteboard</v-btn>
      <v-btn>Share Screen</v-btn>
      <v-btn>Text Editor</v-btn>
      <v-btn>Code Editor</v-btn>
      <v-spacer></v-spacer>
      <v-btn>Mute All</v-btn>
      <v-btn>End</v-btn>
        <v-btn icon>
            <v-icon>sbf-3-dot</v-icon>
        </v-btn>
      <!-- -->
    </v-app-bar>
    <v-content>
      <!-- Provides the application the proper gutter -->
      <v-container fluid>
        <div>Hi Ram 2</div>
        <v-btn @click="footerExtend = !footerExtend" color="green">Click Me</v-btn>
        <v-btn @click="activeWindow = (activeWindow + 1)" color="green">Click Me2</v-btn>

        <v-window v-model="activeWindow">
          <v-window-item :key="1">Idan</v-window-item>
          <v-window-item :key="2">Maor</v-window-item>
        </v-window>
        <!-- If using vue-router -->
        <!-- <router-view></router-view> -->
      </v-container>
    </v-content>

    <v-expand-transition>
      <v-footer app color="#FF0000" inset fixed height="124" v-if="footerExtend" class="pa-0">
        <v-slide-group
          v-model="model"
          class="pa-0"
          active-class="success"
          show-arrows
          prev-icon="sbf-arrow-left-carousel"
          next-icon="sbf-arrow-right-carousel"
        >
          <v-slide-item v-for="n in 15" :key="n" v-slot:default="{ active, toggle }">
            <v-card
              :color="active ? undefined : 'grey lighten-1'"
              class="ma-2"
              height="100"
              width="154"
              @click="toggle"
            >
              <!-- <v-row
            class="fill-height"
            align="center"
            justify="center"
          >
            <v-scale-transition>
              <v-icon
                v-if="active"
                color="white"
                size="48"
                v-text="'mdi-close-circle-outline'"
              ></v-icon>
            </v-scale-transition>
              </v-row>-->
            </v-card>
          </v-slide-item>
        </v-slide-group>
      </v-footer>
    </v-expand-transition>
  </div>
</template>

<script>
import logoComponent from "./logo/logo.vue";

export default {
  components: {
    logoComponent
  },
  data: () => ({
    model: null,
    footerExtend: true,
    activeWindow: 1
  })
};
</script>

<style lang="less">
.studyRoom {
    .header{
        .logo {
            fill:#fff;
        }
    }
  .roundShape {
    width: 8px;
    height: 8px;
    background-color: #fff;
    border-radius: 50%;
  }
}
</style>
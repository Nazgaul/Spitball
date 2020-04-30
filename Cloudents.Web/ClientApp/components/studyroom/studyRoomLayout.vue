<template>
  <div class="studyRoom">
    <v-navigation-drawer app right clipped class="drawer" :width="drawerExtend ? 300 : 0">
      <v-btn
        icon
        class="collapseIcon"
        @click="drawerExtend = !drawerExtend"
        color="#fff"
        rounded="false"
      >
        <v-icon v-if="drawerExtend">sbf-arrow-right-carousel</v-icon>
        <v-icon v-else>sbf-arrow-left-carousel</v-icon>
      </v-btn>
    </v-navigation-drawer>

    <v-app-bar app clipped-right color="#4c59ff" class="header">
      <logoComponent></logoComponent>

      <div class="roundShape mr-2"></div>
      <v-toolbar-title class="white--text">Live</v-toolbar-title>

      <v-btn depressed class="ma-2" @click="setClass()">Class</v-btn>
      <v-btn depressed class="ma-2" @click="setWhiteboard()">Whiteboard</v-btn>
      <v-btn depressed class="ma-2" @click="setShareScreen()">Share Screen</v-btn>
      <v-btn depressed class="ma-2" @click="setTextEditor()">Text Editor</v-btn>
      <v-btn depressed class="ma-2" @click="setCodeEditor()">Code Editor</v-btn>
      <v-spacer></v-spacer>
      <v-btn depressed class="ma-2" @click="MuteAll()">Mute All</v-btn>
      <v-btn rounded depressed class="ma-2" @click="EndSeesion()">End</v-btn>
      <v-btn icon>
        <v-icon>sbf-3-dot</v-icon>
      </v-btn>
      <!-- -->
    </v-app-bar>
    <v-content>
      <!-- Provides the application the proper gutter -->
      <!-- <v-container fluid class="pa-0"> -->
      <component style="height:100%" :is="activeWindow"></component>
      <!-- <v-window v-model="activeWindow">
          <v-window-item :key="1">Class</v-window-item>
          <v-window-item :key="2">White board</v-window-item>
          <v-window-item :key="3">setShareScreen</v-window-item>
          <v-window-item :key="4">setStext editorareScreen</v-window-item>
          <v-window-item :key="5">code editor</v-window-item>
      </v-window>-->
      <!-- If using vue-router -->
      <!-- <router-view></router-view> -->
      <!-- </v-container> -->
    </v-content>

    <v-slide-y-transition>
      <v-footer
        app
        color="#212123"
        inset
        fixed
        :height="footerExtend ? 124 : 0"
        class="pa-0 footer"
      >
        <v-btn
          icon
          class="collapseIcon"
          @click="footerExtend = !footerExtend"
          color="#fff"
        >
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
    </v-slide-y-transition>
  </div>
</template>

<script>
import logoComponent from "../app/logo/logo.vue";
const canvas = () => import("./windows/canvas/canvas.vue");

export default {
  components: {
    logoComponent,
    canvas
  },
  data: () => ({
    model: null,
    footerExtend: true,
    drawerExtend: true,
    activeWindow: canvas
  }),
  methods: {
    setClass() {
      this.activeWindow = 1;
    },
    setWhiteboard() {
      this.activeWindow = 2;
    },
    setShareScreen() {},
    setTextEditor() {
      this.activeWindow = 4;
    },
    setCodeEditor() {
      this.activeWindow = 5;
    }
  }
};
</script>

<style lang="less">
.studyRoom {
  .header {
    .logo {
      fill: #fff;
    }
  }
  .roundShape {
    width: 8px;
    height: 8px;
    background-color: #fff;
    border-radius: 50%;
  }
  .drawer {
    overflow: initial; // to let the collapse btn to show
   // position: relative;
    .collapseIcon {
      position: absolute;
      top: 20px;
      left: -35px;
      background: #212123;
      border-radius: 0%; //vuetify override
    }
  }

  .footer {
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
}
</style>
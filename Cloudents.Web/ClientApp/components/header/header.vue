<template>
  <component is="slot">
      <search-item v-model="showDialog" :type="type"></search-item>
      <v-toolbar app clipped-left fixed :height="isMobileSize? 48 : 72" :extended="isMobileSize" class="header">
        <v-toolbar-title :style="$vuetify.breakpoint.smAndUp ? 'width: 230px; min-width: 230px' : 'min-width: 72px'">
            <router-link class="logo-link" :to="{name:'home'}">
                <logo class="logo"></logo>
            </router-link>
        </v-toolbar-title>
        <form @submit.prevent="submit" :slot="isMobileSize? 'extension' : 'default'" :class="isMobileSize? 'ml-2 mr-2' : 'default'">
            <v-text-field  light solo class="search-b" placeholder="Ask me anything" v-model="qFilter" prepend-icon="sbf-search" :append-icon="voiceAppend"  :append-icon-cb="mic"></v-text-field>
        </form>
        <div class="settings-wrapper d-flex align-center">
            <v-menu bottom left>
                <v-btn icon slot="activator" dark>
                    <v-icon>sbf-3-dot</v-icon>
                </v-btn>
                <v-list>
                    <v-list-tile @click="$_currentClick(item)" v-for="(item,index) in settingMenu">
                        <v-list-tile-content>
                            <v-list-tile-title>{{item.id==='university'&&getUniversityName?getUniversityName:item.name}}</v-list-tile-title>
                        </v-list-tile-content>
                    </v-list-tile>
                </v-list>
            </v-menu>
        </div>
    </v-toolbar></component>
</template>
<script src="./header.js"></script>
<style src="./header.less" lang="less"></style>



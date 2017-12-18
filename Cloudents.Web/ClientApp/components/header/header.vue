<template>
        <v-toolbar app flat clipped-left fixed :height="isMobileSize? 48 : 72" :extended="isMobileSize&&!showSingleLine" class="header">
          <slot :name="`${$route.name}Mobile`">
              <v-toolbar-title :style="$vuetify.breakpoint.smAndUp ? 'width: 230px; min-width: 230px' : 'min-width: 72px'">
                <router-link class="logo-link" :to="{name:'home'}">
                    <logo class="logo"></logo>
                </router-link>
            </v-toolbar-title></slot>
         <slot :name="`${$route.name}SecondLineMobile`" :slot="isMobileSize? 'extension' : 'default'" v-if="!showSingleLine">
             <form @submit.prevent="submit" :class="isMobileSize? 'ml-2 mr-2' : 'default'">
                <v-text-field light solo class="search-b" placeholder="Ask me anything" v-model="qFilter"  prepend-icon="sbf-search" :append-icon="voiceAppend" :append-icon-cb="$_voiceDetection"></v-text-field>
            </form></slot>

            <div class="settings-wrapper d-flex align-center" v-if="showMoreOptions">
                <v-menu bottom left>
                    <v-btn icon slot="activator" dark>
                        <v-icon>sbf-3-dot</v-icon>
                    </v-btn>
                    <v-list>
                        <v-list-tile @click="$_currentClick(item)" v-for="(item,index) in settingMenu" :key="index" :id="item.id">
                            <v-list-tile-content>
                                <v-list-tile-title>{{item.id==='university'&&getUniversityName?getUniversityName:item.name}}</v-list-tile-title>
                            </v-list-tile-content>
                        </v-list-tile>
                    </v-list>
                </v-menu>
            </div>
        </v-toolbar>
</template>
<script src="./header.js"></script>
<style src="./header.less" lang="less"></style>



<template>

    <v-toolbar app fixed height="60" extended class="header">
        <v-toolbar-title class="ma-0">
            <router-link class="logo-link" :to="{name:'home'}">
                <logo class="logo"></logo>
            </router-link>
        </v-toolbar-title>
        <v-toolbar-items>
            <form @submit.prevent="submit" :class="isMobileSize? 'ml-2 mr-2' : 'default'">
                <v-text-field type="search" light solo class="search-b" placeholder="Ask me anything" v-model="qFilter" prepend-icon="sbf-search" :append-icon="voiceAppend" :append-icon-cb="$_voiceDetection"></v-text-field>
            </form>
            <!--v-if="showMoreOptions"-->
            <div class="settings-wrapper d-flex align-center">
                <v-menu bottom left>
                    <v-btn icon slot="activator">
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
        </v-toolbar-items>
        <template slot="extension">
            <div class="gap ma-0"></div>
            <!--<nav-bar></nav-bar>-->
            <v-tabs :value="currentSelection" :scrollable="false">
                <v-tabs-bar>
                    <v-tabs-item v-for="tab in verticals" :key="tab.id" :href="tab.id" :id="tab.id" @click="$_updateType(tab.id)" :class="['spitball-text-'+tab.id,tab.id==currentSelection?'tabs__item--active':'']"
                                 class="mr-4 vertical">
                        {{tab.name}}
                    </v-tabs-item>
                    <v-tabs-slider :color="`color-${currentSelection}`"></v-tabs-slider>
                </v-tabs-bar>
            </v-tabs>
        </template>
        <!--<div slot="extension">
            <v-layout column>
                <router-link class="logo-link" :to="{name:'home'}">
                    <logo class="logo"></logo>
                </router-link>
                <router-link class="logo-link" :to="{name:'home'}">
                    <logo class="logo"></logo>
                </router-link>
            </v-layout>
        </div>-->

    </v-toolbar>


    <!--<v-toolbar app flat clipped-left fixed :height="isMobileSize? 48 : 72" :extended="isMobileSize&&!showSingleLine" class="header">
      <slot :name="`${$route.name}Mobile`">
          <v-toolbar-title :style="$vuetify.breakpoint.smAndUp ? 'width: 230px; min-width: 230px' : 'min-width: 72px'">
            <router-link class="logo-link" :to="{name:'home'}">
                <logo class="logo"></logo>
            </router-link>
        </v-toolbar-title></slot>
     <slot :name="`${$route.name}SecondLineMobile`" :slot="isMobileSize? 'extension' : 'default'" v-if="!showSingleLine">
         <form @submit.prevent="submit" :class="isMobileSize? 'ml-2 mr-2' : 'default'">
            <v-text-field type="search" light solo class="search-b" placeholder="Ask me anything" v-model="qFilter"  prepend-icon="sbf-search" :append-icon="voiceAppend" :append-icon-cb="$_voiceDetection"></v-text-field>
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
    </v-toolbar>-->
</template>
<script src="./header.js"></script>
<style src="./header.less" lang="less"></style>



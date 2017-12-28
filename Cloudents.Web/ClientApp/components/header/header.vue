<template>
    <v-toolbar app fixed :height="$vuetify.breakpoint.mdAndUp ? 120 : 152" class="header">
        <v-layout column class="header-elements">
            <v-flex class="line">
                <v-layout row>
                    <v-toolbar-title class="ma-0">
                        <router-link class="logo-link" :to="{name:'home'}">
                            <logo class="logo"></logo>
                        </router-link>
                    </v-toolbar-title>
                    <v-toolbar-items>
                        <form v-if="$vuetify.breakpoint.mdAndUp" @submit.prevent="submit">
                            <v-text-field type="search" light solo class="search-b" placeholder="Ask me anything" v-model="qFilter" prepend-icon="sbf-search" :append-icon="voiceAppend" :append-icon-cb="$_voiceDetection"></v-text-field>
                        </form>
                        <v-spacer v-if="$vuetify.breakpoint.smAndDown"></v-spacer>
                        <div class="settings-wrapper d-flex align-center">
                            <v-menu bottom left>
                                <v-btn icon slot="activator">
                                    <v-icon>sbf-3-dot</v-icon>
                                </v-btn>
                                <v-list class="settings-list">
                                    <v-list-tile @click="$_currentClick(item)" v-for="(item,index) in settingMenu" :key="index" :id="item.id">
                                        <v-list-tile-content>
                                            <v-list-tile-title>{{item.id==='university'&&getUniversityName?getUniversityName:item.name}}</v-list-tile-title>
                                        </v-list-tile-content>
                                    </v-list-tile>
                                </v-list>
                            </v-menu>
                        </div>
                    </v-toolbar-items>
                </v-layout>
            </v-flex>
            <v-flex v-if="$vuetify.breakpoint.smAndDown" class="line">
                <form @submit.prevent="submit">
                    <v-text-field type="search" light solo class="search-b" placeholder="Ask me anything" v-model="qFilter" prepend-icon="sbf-search" :append-icon="voiceAppend" :append-icon-cb="$_voiceDetection"></v-text-field>
                </form>
            </v-flex>
            <v-flex class="line">
                <v-layout row>
                    <div class="gap ma-0" v-if="$vuetify.breakpoint.mdAndUp"></div>
                    <!--<nav-bar></nav-bar>-->
                    <v-tabs class="verticals-bar" :value="currentSelection" :scrollable="false">
                        <v-tabs-bar>
                            <v-tabs-item v-for="tab in verticals" :key="tab.id" :href="tab.id" :id="tab.id" @click="$_updateType(tab.id)" :class="['spitball-text-'+tab.id,tab.id==currentSelection?'tabs__item--active':'']"
                                         class="mr-4 vertical">
                                {{tab.name}}
                            </v-tabs-item>
                            <v-tabs-slider :color="`color-${currentSelection}`"></v-tabs-slider>
                        </v-tabs-bar>
                    </v-tabs>
                </v-layout>
            </v-flex>

        </v-layout>
    </v-toolbar>
</template>
<script src="./header.js"></script>
<style src="./header.less" lang="less"></style>



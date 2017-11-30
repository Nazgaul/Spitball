<template>
    <v-toolbar app clipped-left fixed :height="isMobileSize? 48 : 72" :extended="isMobileSize" class="header">
        <!--class="ml-0 pl-3"-->
        <!--<v-icon name="search"></v-icon>-->
        <v-toolbar-title :style="$vuetify.breakpoint.smAndUp ? 'width: 230px; min-width: 230px' : 'min-width: 72px'">
            <router-link class="logo-link" :to="{name:'home'}">
                <logo class="logo"></logo>
            </router-link>
        </v-toolbar-title>
        <form @submit.prevent="submit" :slot="isMobileSize? 'extension' : 'default'" :class="isMobileSize? 'ml-2 mr-2' : 'default'">
            <v-text-field  light solo class="search-b" placeholder="Ask me anything" v-model="qFilter" prepend-icon="sbf-search" :append-icon="voiceAppend"  :append-icon-cb="mic"></v-text-field>
        </form>
        <div class="settings-wrapper d-flex align-center">
            <router-link :to="{name:'setting'}" class="settings">
                <settings-icon></settings-icon>
            </router-link>
        </div>
    </v-toolbar>
</template>
<script src="./header.js"></script>
<style src="./header.less" lang="less"></style>



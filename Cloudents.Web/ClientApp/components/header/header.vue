<template>
    <v-toolbar app clipped-left fixed :height="$vuetify.breakpoint.smAndDown? 48 : 72" :extended="$vuetify.breakpoint.smAndDown" class="header">
        <!--class="ml-0 pl-3"-->
        <!--<v-icon name="search"></v-icon>-->
        <v-toolbar-title :style="$vuetify.breakpoint.smAndUp ? 'width: 230px; min-width: 230px' : 'min-width: 72px'" :class="$vuetify.breakpoint.smAndUp? 'pl-4' : ''">
            <router-link :to="{name:'home'}">
                <logo class="logo"></logo>
            </router-link>
        </v-toolbar-title>
        <form @submit.prevent="submit" :slot="$vuetify.breakpoint.smAndDown? 'extension' : 'default'">
            <v-text-field  light solo class="search-b" placeholder="Ask me anything" v-model="qFilter"></v-text-field>
        </form>
        <div class="d-flex align-center" style="margin-left: auto">
            <router-link :to="{name:'setting'}" class="settings">
                <settings-icon></settings-icon>
            </router-link>
        </div>
    </v-toolbar>
</template>
<script src="./header.js"></script>
<style src="./header.less" lang="less"></style>



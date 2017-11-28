<template>
    <v-toolbar app clipped-left fixed :height="$vuetify.breakpoint.smAndDown? 48 : 72" :extended="$vuetify.breakpoint.smAndDown" class="header">
        <!--class="ml-0 pl-3"-->
        <v-icon name="search"></v-icon>
        <v-toolbar-title :style="$vuetify.breakpoint.smAndUp ? 'width: 230px; min-width: 230px' : 'min-width: 72px'" :class="$vuetify.breakpoint.smAndUp? 'pl-4' : ''">
            <logo class="logo"></logo>
        </v-toolbar-title>
        <v-text-field :slot="$vuetify.breakpoint.smAndDown? 'extension' : 'default'" light solo class="search-b"  placeholder="Ask me anything" v-model="qFilter"></v-text-field>
        <div class="d-flex align-center" style="margin-left: auto">
            <router-link :to="{name:'setting'}" class="settings">
                <settings-icon></settings-icon>
            </router-link>
        </div>
    </v-toolbar>

    <!--<v-toolbar app clipped-left fixed class="header" >
        <v-layout row fill-height class="first-header ma-0">
            <!--@click.stop="primaryDrawer.model = !primaryDrawer.model"
            <v-toolbar-side-icon class="hidden-md-and-up" @click.stop="menuToggle" ref="menuIcon">
                <hamburger></hamburger>
            </v-toolbar-side-icon>
            <router-link :to="{name:'home'}" class="logo-link pt-2">
                <logo class="logo"></logo>
            </router-link>
            <div class="line"></div>
            <div class="icon">
                <component :class="'fill-'+name" class="item" v-bind:is="name+'Header'"></component>
            </div>
            <div class="line"></div>
            <form @submit.prevent="submit" class="box-header-search ml-3">
                <button type="submit" class="mr-2 pt-2"><search-icon></search-icon></button>
                <input name="search" type="text" placeholder="Ask me anything" required
                       v-model="qFilter" ref="search">
            </form>

            <!--<div class="line"></div>
            <div class="notification pt-2">
                <v-badge right overlap>
                    <span slot="badge">1</span>
                    <notification-icon></notification-icon>
                </v-badge>
            </div>
        </v-layout>
    </v-toolbar>-->
</template>
<script src="./header.js"></script>
<style src="./header.less" lang="less"></style>



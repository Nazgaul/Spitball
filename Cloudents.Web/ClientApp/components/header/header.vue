<template>
    <v-toolbar app clipped-left fixed class="header" >
        <v-layout row fill-height class="first-header ma-0">
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
                <input name="search" type="text" :placeholder="placeholders[name]" required
                       v-model="qFilter" ref="search">
            </form>

            <div class="line"></div>
            <div class="notification pt-2">
                <v-badge right overlap>
                    <span slot="badge">1</span>
                    <notification-icon></notification-icon>
                </v-badge>
            </div>
        </v-layout>
    </v-toolbar>
</template>
<script src="./header.js"></script>
<style src="./header.less" lang="less"></style>



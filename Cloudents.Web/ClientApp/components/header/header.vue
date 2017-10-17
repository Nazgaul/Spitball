<template>
    <v-toolbar app clipped-left fixed class="header" >
        <v-layout row fill-height class="first-header ma-0">
            <router-link :to="{name:'home'}" class="logo-link">
                <logo class="logo"></logo>
            </router-link>
            <div class="line"></div>
            <div class="icon">
                <component :class="'fill-'+$route.name" class="item" v-bind:is="$route.name+'Header'"></component>
            </div>
            <div class="line"></div>
            <form @submit.prevent="submit" class="box-header-search ml-2">
                <button type="submit" class="mr-2"><search-icon></search-icon></button>
                <input name="search" type="text" :placeholder="placeholders[$route.name]"
                       v-model="qFilter" ref="search" @focus="focus">
            </form>

            <div class="line"></div>
            <div class="notification">
                <v-badge right overlap>
                    <span slot="badge">1</span>
                    <notification-icon></notification-icon>
                </v-badge>
            </div>
        </v-layout>
        <div :slot="isOptions?'extension':''" v-show="isOptions">
            <search-type class="verticals"  :values="names" :model="'searchTypes'" :value="$route.name" @click="changeType"></search-type>
        </div>
    </v-toolbar>
</template>
<script src="./header.js"></script>
<style src="./header.less" lang="less"></style>



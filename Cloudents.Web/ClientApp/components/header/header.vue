<template>
    <v-toolbar app clipped-left fixed>
        <!--<v-toolbar clipped-left prominent class="white border">-->
        <!--tag="v-toolbar-title"-->
        <v-layout row fill-height class=" ml-0">
            <router-link :to="{name:'home'}" class="logo-link">
                <logo class="logo"></logo>
            </router-link>
            <div class="line"></div>
            <div class="icon">
                <component :class="'bg-'+$route.name" class="item" v-bind:is="$route.name+'Header'"></component>
            </div>
            <div class="line"></div>
            <!--<v-text-field
                        prepend-icon="searchIcon"  :placeholder="placeholders[$route.name]" :prepend-icon-cb="searchIcon"
                          solo v-model="qFilter" @focus="focus" hide-details single-line>
                <label slot="label">hi</label>
            </v-text-field>-->

            <form @submit.prevent="submit" class="box-header-search ml-2">
                <button type="submit" class="mr-2"><search-icon></search-icon></button>
                <input name="search" type="text" :placeholder="placeholders[$route.name]" v-model="qFilter" @focus="focus">
            </form>

            <div class="line"></div>
            <div class="notification">
                <v-badge right overlap>
                    <span slot="badge">10</span>
                    <notification-icon></notification-icon>
                </v-badge>
            </div>



            <!--<v-flex :slot="isOptions?'extension':''" v-show="isOptions">
                <search-type class="searchTypes header offset-xs2" :values="names" :model="'searchTypes'" :value="$route.name" @click="changeType"></search-type>
            </v-flex>-->
        </v-layout>
    </v-toolbar>
</template>
<script src="./header.js"></script>
<style src="./header.less" lang="less" scoped></style>

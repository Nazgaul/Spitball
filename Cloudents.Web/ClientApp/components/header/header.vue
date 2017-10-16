<template>
    <v-toolbar app clipped-left fixed color="#ffdcdc">
        <!--<v-toolbar clipped-left prominent class="white border">-->
        <!--tag="v-toolbar-title"-->
        <router-link  :to="{name:'home'}" class="logo-link ml-0">
            <logo class="logo"></logo>
            <!--<picture>
                <source media="(min-width: 960px)" srcset="/images/sb-logo.svg">
                <img src="/Images/spitb-logo.svg" alt="logo">
            </picture>-->
        </router-link>
        <hr />
        <div class="icon"><component :class="$route.name" class="item" v-bind:is="$route.name+'Header'"></component></div>
        <div class="box-header-search">
            <form @submit.prevent="submit">
                <button type="submit"><img src="/Images/search-icon.svg" alt=""></button>
                <input name="search" type="text" :placeholder="placeholders[$route.name]" v-model="qFilter" @focus="focus">
            </form>
        </div>
        <v-flex :slot="isOptions?'extension':''" v-show="isOptions">
            <search-type class="searchTypes header offset-xs2" :values="names" :model="'searchTypes'" :value="$route.name" @click="changeType"></search-type>
        </v-flex>
        

    </v-toolbar>
</template>
<script src="./header.js"></script>
<style src="./header.less" lang="less" scoped></style>

<template>
    <div class="search-input">
        <span class="search-back-arrow" @click="goBackFromSearch()" v-show="isSearchActive">
            <v-icon :class="{'rtl': isRtl}">sbf-arrow-right</v-icon>
        </span>
        <div class="search-b-wrapper">
        <!--<div class="search-b-wrapper" v-scroll="onScroll">-->
            <v-text-field class="search-b" type="text" solo
                          :class="{'white-background': showSuggestions}"
                          @keyup.enter="search()" autocomplete="off" @keyup.down="arrowNavigation(1)"
                          @keyup.up="arrowNavigation(-1)"
                          name="q"
                          id="transcript"
                          clearable
                          clear-icon="sbf-close"
                          v-model="msg" @input="changeMsg"
                          :placeholder="placeholder"
                          prepend-icon="sbf-search"
                          :hide-on-scroll="isHome?hideOnScroll:false">

            </v-text-field>
            <div class="menu-toggler" v-show="showSuggestions" @click="closeSuggestions"></div>
                <v-list class="search-menu" v-click-outside="outsideClick" v-show="showSuggestions">
                    <template v-for="(item, index) in suggestList">
                        <v-list-tile class="suggestion" @click="selectos(item)" :key="index">
                            <v-list-tile-action hidden-xs-only>
                                <component :is="item.icon"></component>
                            </v-list-tile-action>
                            <v-list-tile-content>
                                <v-list-tile-title>{{item.name}}</v-list-tile-title>
                            </v-list-tile-content>
                        </v-list-tile>
                    </template>
                </v-list>
        </div>
        <slot name="searchBtn" :search="search"></slot>
    </div>
</template>

<script src="./searchInput.js"></script>
<style src="./searchInput.less" lang="less"></style>

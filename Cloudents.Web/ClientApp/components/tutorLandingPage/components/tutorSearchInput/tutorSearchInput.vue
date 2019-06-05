<template>
    <div class="tutor-search-input">
        <span class="search-back-arrow" @click="goBackFromSearch()" v-show="isSearchActive">
            <v-icon :class="{'rtl': isRtl}">sbf-arrow-right</v-icon>
        </span>
        <div class="search-b-wrapper">
            <v-text-field class="search-b" type="text" solo
                          :class="{'white-background': showSuggestions}"
                          @keyup.enter="search()" autocomplete="off"
                          id="tutorSearchInput"
                          clearable
                          clear-icon="sbf-close"
                          v-model="msg" 
                          @input="changeMsg"
                          :placeholder="placeholder"
                          prepend-icon="sbf-search">

            </v-text-field>
            <div class="menu-toggler" v-show="showSuggestions" @click="closeSuggestions"></div>
                <v-list class="search-menu" v-click-outside="outsideClick" v-show="showSuggestions">
                    <template v-for="(item, index) in suggest">
                        <v-list-tile class="suggestion" @click="selectors(item)" :key="index">
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

<script src="./tutorSearchInput.js"></script>
<style src="./tutorSearchInput.less" lang="less"></style>

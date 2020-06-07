<template>
    <div class="tutor-search-input">
        <div class="search-b-wrapper">
            <v-text-field class="search-b" type="text" solo
                          :class="{'white-background': showSuggestions}"
                          @keyup.enter="search()" autocomplete="off" 
                          @keydown.down="arrowNavigation(1)"
                          @keydown.up="arrowNavigation(-1)"
                          id="tutorSearchInput"
                          clearable
                          clear-icon="sbf-close"
                          v-model="msg" 
                          @input="changeMsg"
                          :placeholder="placeholder"
                          prepend-icon="sbf-search">
            </v-text-field>
            <!-- <div class="menu-toggler" v-show="showSuggestions" @click="closeSuggestions"></div> -->
                <v-list class="search-menu" v-show="showSuggestions">
                    <template v-for="(item, index) in suggests">
                        <v-list-item class="suggestion" @click="selectors(item)" :key="index" :class="{'list__tile--highlighted': index === focusedIndex}">
                            <v-list-item-content>
                                <v-list-item-title>{{item.text}}</v-list-item-title>
                            </v-list-item-content>
                        </v-list-item>
                    </template>
                </v-list>
        </div>
        <slot name="searchBtn" :search="search"></slot>
    </div>
</template>

<script src="./tutorSearchInput.js"></script>
<style src="./tutorSearchInput.less" lang="less"></style>

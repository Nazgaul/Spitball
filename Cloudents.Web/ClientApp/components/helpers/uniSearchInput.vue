<template>
    <div class="search-input">
        <div class="search-b-wrapper">
            <v-text-field class="search-b" type="search" solo
                          @keyup.enter="search" autocomplete="off" @keyup.down="arrowNavigation(1)"
                          @keyup.up="arrowNavigation(-1)"
                          required name="q"
                          id="transcript"
                          v-model.trim="msg" :placeholder="placeholder"
                          :prepend-icon="prependIcon" @click="openSuggestions"></v-text-field>
            <!--<input type="checkbox" id="toggler"/>-->
            <div class="menu-toggler" v-show="showSuggestions" @click="closeSuggestions"></div>
                <v-list class="search-menu" v-show="showSuggestions && suggestList.length">
                    <v-subheader v-if="!msg.length || (focusedIndex >= 0 && !originalMsg.length)" v-language:inner>uniSearchinput_universities_near_you</v-subheader>
                    <template v-for="(item, index) in suggestList">
                        <v-list-tile class="suggestion" @click="selectos({item:item,index})"
                                     :class="{'list__tile--highlighted': index === focusedIndex}"
                                     :key="index">
                            <v-list-tile-action hidden-xs-only>
                                <img class="suggestion-image" :src="item.image"/>
                            </v-list-tile-action>
                            <v-list-tile-content>
                                <v-list-tile-title v-html="highlightSearch(item)"></v-list-tile-title>
                            </v-list-tile-content>
                        </v-list-tile>
                    </template>
                </v-list>
        </div>
        <slot name="searchBtn" :search="search"></slot>
    </div>
</template>

<script src="./uniSearchInput.js"></script>
<style src="./searchInput.less" lang="less"></style>

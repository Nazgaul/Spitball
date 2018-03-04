<template>
    <!--<v-menu :allow-overflow="true" offset-y full-width content-class="search-menu" :value="menuOpen"-->
    <!--class="search-b-wrapper">-->
    <!--<span slot="activator">-->
    <!--<v-text-field class="search-b" type="search" solo-->
    <!--@keyup.enter="search" autocomplete="off"-->
    <!--required name="q"-->
    <!--:class="{'record':isRecording}"-->
    <!--id="transcript"-->
    <!--v-model.trim="msg" :placeholder="placeholder"-->
    <!--prepend-icon="sbf-search" :append-icon="voiceAppend"-->
    <!--:append-icon-cb="$_voiceDetection"></v-text-field>-->
    <!--</span>-->
    <!--<v-list>-->
    <!--<v-subheader v-if="!msg.length">Some things you can ask me:</v-subheader>-->
    <!--<template v-for="(item, index) in suggestList">-->
    <!--&lt;!&ndash;{{item.type}}&ndash;&gt;-->
    <!--<v-list-tile class="suggestion" :class="`type-${item.type}`" @click="selectos({item:item,index})" :key="index">-->
    <!--<v-list-tile-action hidden-xs-only>-->
    <!--<history-icon v-if="item.type==='History'"></history-icon>-->
    <!--<v-icon v-else>sbf-search</v-icon>-->
    <!--</v-list-tile-action>-->
    <!--<v-list-tile-content>-->
    <!--<v-list-tile-title v-html="highlightSearch(item)"></v-list-tile-title>-->
    <!--</v-list-tile-content>-->
    <!--</v-list-tile>-->
    <!--</template>-->
    <!--</v-list>-->
    <!--</v-menu>-->
    <div class="search-b-wrapper">
        <v-text-field class="search-b" type="search" solo
                      @keyup.enter="search" autocomplete="off"
                      required name="q"
                      :class="{'record':isRecording}"
                      id="transcript"
                      v-model.trim="msg" :placeholder="placeholder"
                      prepend-icon="sbf-search" :append-icon="voiceAppend"
                      :append-icon-cb="$_voiceDetection" @click="showSuggestions"></v-text-field>
        <input type="checkbox" id="toggler"/>
        <label class="menu-toggler" for="toggler"></label>
        <v-list class="search-menu">
            <v-subheader v-if="!msg.length">Some things you can ask me:</v-subheader>
            <template v-for="(item, index) in suggestList">
                <!--{{item.type}}-->
                <v-list-tile class="suggestion" :class="`type-${item.type}`" @click="selectos({item:item,index})"
                             :key="index">
                    <v-list-tile-action hidden-xs-only>
                        <history-icon v-if="item.type==='History'"></history-icon>
                        <v-icon v-else>sbf-search</v-icon>
                    </v-list-tile-action>
                    <v-list-tile-content>
                        <v-list-tile-title v-html="highlightSearch(item)"></v-list-tile-title>
                    </v-list-tile-content>
                </v-list-tile>
            </template>
        </v-list>

    </div>
</template>

<script src="./searchInput.js"></script>
<style src="./searchInput.less" lang="less"></style>

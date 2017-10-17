
<template v-once>
    <div>
        <div :class="{overlay:isOverlay}" @click="begone()"></div>
        <app-header ref="header" :overlay.sync="isOverlay" :isOverlay="isOverlay"></app-header>
        <app-aside></app-aside>
        <main>
            <v-content>
                <v-container fluid>
                    <v-layout>
                        <v-flex offset-xs1>
                            <div class="loader" v-if="isLoading">
                                <v-progress-circular indeterminate v-bind:size="50" color="amber"></v-progress-circular>
                            </div>
                            <router-view v-show="!isLoading"></router-view>
                        </v-flex>
                    </v-layout>
                </v-container>
            </v-content>
        </main>
    </div>
</template>
<script src="./pageApp.js"></script>
<style lang="less">
    @import "../../mixin.less";

    .overlay {
        top: 128px;
        background: rgba(0,0,0,0.5);
        pointer-events:all;
    }

    .loader {
        .inTheMiddle();
    }
</style>

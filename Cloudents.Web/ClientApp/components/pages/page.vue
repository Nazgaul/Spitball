
<template v-once>
    <div>
        <div :class="{overlay:isOverlay}"></div>
        <app-header ref="header" :overlay.sync="isOverlay"></app-header>
        <app-aside></app-aside>
        <!--<app-header ref="header"></app-header>-->
        <main>
            <v-content>
                <v-container fluid>
                    <div v-if="isLoading">Loading.....</div>
                    <router-view v-show="!isLoading"></router-view>
                </v-container>
            </v-content>
        </main>

        <!--<section>
            <input id="menu-checkbox" type="checkbox" class="hide_641" />

            <div class="content">
                <div v-if="isLoading">Loading.....</div>
                <router-view v-show="!isLoading"></router-view>
            </div>
        </section>-->
    </div>
</template>
<script src="./pageApp.js"></script>
<!--<style src="./page.less" lang="less" scoped></style>-->

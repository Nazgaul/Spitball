
<template v-once>
    <div>
        <div :class="{overlay:isOverlay}"></div>
        <app-header ref="header" :overlay.sync="isOverlay"></app-header>
        <section>
            <input id="menu-checkbox" type="checkbox" class="hide_641" />
            <app-aside></app-aside>
            <div class="content">
                <div v-if="isLoading">Loading.....</div>
                <router-view v-show="!isLoading"></router-view>
            </div>
        </section>
    </div>
</template>
<script src="./pageApp.js"></script>
<style src="./page.less" lang="less" scoped></style>


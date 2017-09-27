
<template v-once>
    <div>
        <!--<input v-model="isOpen" placeholder="is open main" />-->
        <div class="overlay"></div>
        <app-header ref="header" :isOpen="isOpen" :section.sync="section" :section="section" ></app-header>
        <section>
            <input id="menu-checkbox" type="checkbox" class="hide_641" />
            <app-aside :isOpen="isOpen" :section.sync="section"></app-aside>
            <div class="content">
                <router-link :to="{name:$route.name,query:{a:'yiaft'}}">yifatttttt</router-link>
                <div v-if="isLoading">Loading.....</div>
                <page-result v-else><router-view></router-view></page-result>
            </div>
        </section>
    </div>
</template>
<script src="./pageApp.js"></script>
<style src="./page.less" lang="less" scoped></style>


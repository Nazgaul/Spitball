
<template v-once>
    <div>
        <!--<input v-model="isOpen" placeholder="is open main" />-->
        <div class="overlay"></div>
        <app-header ref="header" :result.sync="result" :isOpen="isOpen" :section.sync="section" :section="section" ></app-header>
        <section>
            <input id="menu-checkbox" type="checkbox" class="hide_641" />
            <app-aside :isOpen="isOpen" :section.sync="section"></app-aside>
            <div class="content">
                <router-link :to="{name:$route.name,query:{a:'yiaft'}}">yifatttttt</router-link>
                <router-view :items="result"></router-view>
            </div>
        </section>
    </div>
</template>
<script src="./pageApp.js"></script>
<style src="./page.less" lang="less" scoped></style>


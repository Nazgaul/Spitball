
<template v-once>
    <div>
        <!--<input v-model="isOpen" placeholder="is open main" />-->
        <div class="overlay"></div>
        <app-header :isOpen="isOpen" :section.sync="section" :section="section"></app-header>
        <section>
            <input id="menu-checkbox" type="checkbox" class="hide_641" />
            <app-aside :isOpen="isOpen" :section.sync="section"></app-aside>
            <div class="content">
                <router-view></router-view>
            </div>
        </section>
    </div>
</template>
<script src="./sections.js"></script>
<style src="./sections.less" lang="less" scoped></style>


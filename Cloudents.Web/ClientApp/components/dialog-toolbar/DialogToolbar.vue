<template>
    <v-toolbar app fixed class="dialog-toolbar elevation-1" :height="height" :extended="extended">
        <v-btn icon class="back" @click="backAction ? backAction() : $router.go(-1)">
            <i class="sbf icon sbf-chevron-down"></i>
        </v-btn>
        <v-toolbar-title class="toolbar-title">{{toolbarTitle}}</v-toolbar-title>
        <v-spacer></v-spacer>
        <slot name="rightElement"></slot>
        <template slot="extension">
            <slot name="extension"></slot>
        </template>
    </v-toolbar>

</template>

<script>
    export default {
        props: {
            height: String,
            toolbarTitle: String,
            backAction: Function,
            extended: Boolean
        }
    };
</script>

<style lang="less" src="./DialogToolbar.less"></style>
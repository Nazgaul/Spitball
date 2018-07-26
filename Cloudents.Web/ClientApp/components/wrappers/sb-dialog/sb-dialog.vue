<template>
    <v-dialog  v-model="show" :fullscreen="$vuetify.breakpoint.xs" max-width="720px"  scrollable :content-class="contentClass">
     <slot></slot>
    </v-dialog>
</template>

<script src="./sb-dialog.js"></script>
<style src="./sb-dialog.less" lang="less"></style>

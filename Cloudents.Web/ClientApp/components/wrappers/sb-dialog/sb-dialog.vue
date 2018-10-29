<template>
    <v-dialog transition="scale-transition" v-model="show" :fullscreen="OverlayActive" :full-width="fullWidth" :max-width="fullWidth ? '1280px' : '720px'" scrollable :content-class="contentClass" :persistent="isPersistent">
     <slot></slot>
    </v-dialog>
</template>

<script src="./sb-dialog.js"></script>
<style src="./sb-dialog.less" lang="less"></style>

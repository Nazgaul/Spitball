<template>
    <v-dialog
            v-model="show"
            v-if="show"
            class="sb-dialog-back"
            :origin="'bottom center'"
            :transition="transitionAnimation"
            :fullscreen="OverlayActive"
            :max-width="fullWidth ? '1280px' : maxWidth"
            scrollable
            :content-class="contentClass"
            :persistent="isPersistent">
        <slot></slot>
    </v-dialog>
</template>

<script src="./sb-dialog.js"></script>
<style src="./sb-dialog.less" lang="less"></style>

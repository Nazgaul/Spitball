<template>
    <v-dialog
              transition="scale-transition"
              v-model="show"
              :full-width="fullWidth"
              :fullscreen="$vuetify.breakpoint.xs"
              :max-width="!fullWidth ? '720px' : '1280px'"
               scrollable
              :content-class="contentClass">
     <slot></slot>
    </v-dialog>
</template>

<script src="./sb-dialog.js"></script>
<style src="./sb-dialog.less" lang="less"></style>

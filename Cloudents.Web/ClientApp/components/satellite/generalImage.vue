<template functional>
<div class="s-main-page">
        <div class="image-section">
            <div class="image hidden-xs-only" :style="'background-image: url('+ require(`./img/${props.header}`)+')'"></div>
        <img :src="require(`./img/${props.icon}`)" class="h-icon d-block">
        </div>
        <div class="wrapper">
            <slot name="default"></slot>
        </div>
</div>
</template>
<style src="./satellite-share.less" lang="less"></style>
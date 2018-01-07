<template functional>
<div class="s-main-page">
        <div class="image-section">
        <img :src="require(`./img/${props.header}`)" alt="header" />
        <img :src="require(`./img/${props.icon}`)" class="h-icon d-block">
        </div>
        <div class="wrapper">
            <slot name="default"></slot>
        </div>
</div>
</template>
<style src="./satellite-share.less" lang="less"></style>
<template>
    <div class="homePage">
      
        <homeVideo />
        <homeQuote />
        <homeRegister />
        <homeBoxes />

    </div>
</template>

<script>
const homeVideo = () => import('./homeVideo.vue')
const homeQuote = () => import('./homeQuote.vue')
const homeRegister = () => import('./homeRegister.vue')
const homeBoxes = () => import('./homeBoxes.vue')

export default {
    components: { homeVideo, homeQuote, homeRegister, homeBoxes }
};
</script>

<style lang="less">
  .homePage { background: #fff; }
</style>
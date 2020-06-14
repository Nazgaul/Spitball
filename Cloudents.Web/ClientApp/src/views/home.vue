<template>
    <div class="homePage">
      
        <homeVideo />
        <homeQuote />
        <homeRegister />
        <homeBoxes />

    </div>
</template>

<script>
const homeVideo = () => import('../components/home/homeVideo.vue')
const homeQuote = () => import('../components/home/homeQuote.vue')
const homeRegister = () => import('../components/home/homeRegister.vue')
const homeBoxes = () => import('../components/home/homeBoxes.vue')

export default {
    components: { homeVideo, homeQuote, homeRegister, homeBoxes }
};
</script>

<style lang="less">
  .homePage { background: #fff; }
</style>
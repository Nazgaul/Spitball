<template>
    <div class="soon-container-overlay">
        <div class="soon-img-container">
            <img v-if="isRtl" src="./img/soonPic.png" alt="" />
            <img v-else src="./img/soon-Pic-english.png" alt="" />
        </div>
    </div>
</template>

<script>
export default {
    data(){
        return{
            isRtl: global.isRtl
        }
    }
}
</script>

<style lang="less">
    .soon-container-overlay{
        position: absolute;
        width: 100%;
        height: 100%;
        z-index: 101;
        background-color: rgba(0, 0, 0, 0.95);
        display: flex;
        justify-content: center;
        margin: 0 auto;
        .soon-img-container{
            pointer-events: none;
            position: fixed;
            top: 190px;
        }
    }
</style>

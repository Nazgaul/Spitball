<template>
    <div class="player-wrapper">
        <div class="player">
            <div class="arrow"></div>
        </div>
    </div>
</template>


<script>
    export default {
        props: { item: {type:Object,required:true}}
    }
</script>
<style scoped>

    .loading-skeleton .player-wrapper {
        width: 100%;
        max-width: 504px;
        height: 283px;
        display: flex;
        align-items: center;
        justify-content: center;
        background: #eeeeee;
        border: 8px solid white;
    }

    .loading-skeleton .player {
        width: 78px;
        height: 55px;
        background: white;
        border-radius: 16px;
        border-radius: 16px;
        display: flex;
        align-items: center;
        justify-content: center;
    }

    .loading-skeleton .arrow {
        width: 0;
        height: 0;
        border-style: solid;
        border-width: 11px 0 11px 23px;
        border-color: transparent transparent transparent #eeeeee;
    }
    @media only screen and (max-device-width : 600px) {
        .loading-skeleton .player-wrapper {
            width: 360px;
            height: 202px;
        }
    }
</style>
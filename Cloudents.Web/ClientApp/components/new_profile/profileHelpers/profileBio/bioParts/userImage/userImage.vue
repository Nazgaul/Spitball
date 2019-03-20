<template>
    <div class="user-image-wrap">
        <img class="user-picture" style="height: 240px; width: 214px;"
              src="../../../../images/placeholder.png">
        <div class="bottom-section">
            <user-rating :rating="4.5" class="px-4" ></user-rating>
        </div>
    </div>

</template>

<script>
    import userRating from '../userRating.vue'
    export default {
        components: {userRating},
        name: "userImage"
    }
</script>

<style lang="less">
    .user-image-wrap{
        position: relative;
        height: 240px;
        max-width: 214px;
        .user-picture{

        }
        .bottom-section{
            display: flex;
            align-items: center;
            justify-content: center;
            position: absolute;
            bottom: 0;
            right: 0;
            left: 0;
            padding: 18px 0;
            background-color: rgba(31, 31, 45, 0.73);

        }

    }

</style>
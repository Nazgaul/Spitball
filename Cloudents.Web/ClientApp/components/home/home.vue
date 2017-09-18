<template v-once>
    <div class="page">
        <form v-on:submit.prevent="search">
            <input v-model.trim="msg" :placeholder="placeholder" />
            <input type="submit" value="search" />
        </form>
        <vertical-collection :changeCallback="changeSection"></vertical-collection>
    </div>
</template>

<style src="./home.less" lang="less" scoped></style>
<script src="./home.js"></script>

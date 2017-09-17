<template v-once>
    <div class="page">
        <form v-on:submit.prevent="search">
            <input v-model.trim="msg" />
            <input type="submit" value="search" />
        </form>
        <div>{{result}}</div>
        <vertical-collection></vertical-collection>
    </div>
</template>

<style src="./home.less" lang="less" scoped></style>
<script src="./home.js"></script>

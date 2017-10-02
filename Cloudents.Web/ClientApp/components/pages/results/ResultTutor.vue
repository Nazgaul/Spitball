<template>
    <div>
        <div v-for="item in items">{{item.name}}</div>
    </div>
</template>
<script>
    export default {
        computed: {
            items: function () { return this.$store.getters.items }
        }
    }
</script>
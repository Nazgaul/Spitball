<template>
    <div>
        <v-text-field v-model="accountNum" readonly></v-text-field>
        <button @click="copy">Copy number</button>
    </div>
</template>

<script>
    export default {
        computed: {
            accountNum: function () {
                return 'accountNum123456'
            }
        },
        methods: {
            copy(){

            }
        }
    }
</script>

<style scoped>

</style>
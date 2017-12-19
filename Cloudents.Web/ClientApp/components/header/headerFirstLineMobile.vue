<template>
    <div>
        <v-btn @click="$router.go(-1)">back</v-btn>
        <span>{{name}}</span>
    </div>
</template>

<script>
    export default {
        name: "header-first-line-mobile",
        props:{name:{type:String}}
    }
</script>